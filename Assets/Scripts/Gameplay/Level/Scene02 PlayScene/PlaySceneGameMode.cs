using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Mathlife.ProjectL.Gameplay.ObjectBase;
using Mathlife.ProjectL.Gameplay.Play;
using Mathlife.ProjectL.Gameplay.UI;
using Sirenix.Utilities;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Mathlife.ProjectL.Gameplay
{
    public class PlaySceneGameMode : GameMode<PlaySceneGameMode>
    {
        protected override SingletonLifeTime LifeTime => SingletonLifeTime.Scene;

        // Alias
        private ArtyRosterState ArtyRosterState => GameState.Inst.artyRosterState;
        private BattleState BattleState => GameState.Inst.battleState;

        // Inspector
#if UNITY_EDITOR
        [SerializeField]
        private List<ArtyPreset> developPlayers = new();

        [SerializeField]
        private List<Enemy> developEnemys = new();

        [SerializeField]
        private AssetReferenceT<StageGameData> developStageGameDataRef = null;
#endif

        // Field
        public ArtyController turnOwner { get; private set; }
        private bool developMode = false;
        private StageGameData stageGameData;
        private readonly List<ArtyController> battlers = new();

        private readonly List<ArtyController> aliveBattlers = new();

        public ImmutableList<ArtyController> AlivePlayers => aliveBattlers
            .Where(battler => battler.IsPlayer)
            .ToImmutableList();

        public override async UniTask InitializeScene(IProgress<float> progress)
        {
            // 0. 게임 모드 공통 초기화 로직 수행
            await base.InitializeScene(progress);
            progress.Report(0.1f);

            // 1. 모든 UI 닫아놓기
            MainCanvas.Inst.DeactivateAllPresenters();
            progress.Report(0.05f);

            // 2. 모드 세팅
#if UNITY_EDITOR
            developMode = BattleState.stageGameData == false;
#else
            developMode = false;
#endif

            stageGameData = BattleState.stageGameData;
#if UNITY_EDITOR
            if (developMode)
            {
                stageGameData = await developStageGameDataRef.LoadAssetAsync();
            }
#endif

            // 3. 맵 생성
            LoadingScreenManager.Inst.SetMessage("맵을 생성하는 중...");

            float start = 0.05f;
            float end = 0.95f;
            IProgress<float> mapLoadingProgress =
                Progress.Create<float>(value => progress.Report(Mathf.Lerp(start, end, value)));

            Sprite mapSprite = stageGameData.mapSprite;
            await DestructibleTerrain.Inst.GenerateTerrain(mapSprite, mapLoadingProgress);
            progress.Report(0.95f);

            // 3. 플레이어 및 적 준비
            LoadingScreenManager.Inst.SetMessage("화포를 생성하는 중...");

            battlers.Clear();

            void InstantiateBattler(int spawnIndex, ArtyModel arty, Enemy enemy)
            {
                GameObject inst = Instantiate(arty.BattlerPrefab);
                inst.transform.position = stageGameData.spawnPositions[spawnIndex];
                var battler = inst.GetComponent<ArtyController>();
                battler.Setup(arty, enemy);
                battlers.Add(battler);
            }

            if (developMode)
            {
#if UNITY_EDITOR
                for (int i = 0; i < Mathf.Min(developPlayers.Count, 3); ++i)
                {
                    var player = developPlayers[i];
                    var arty = new ArtyModel(player.arty, player.level, 0L);
                    arty.Equip(EMechPartType.Barrel, player.barrel?.id ?? -1);
                    arty.Equip(EMechPartType.Armor, player.armor?.id ?? -1);
                    arty.Equip(EMechPartType.Engine, player.engine?.id ?? -1);

                    InstantiateBattler(i, arty, null);
                }

                for (int i = 0; i < Mathf.Min(developEnemys.Count, 3); ++i)
                {
                    var enemy = developEnemys[i];
                    var arty = new ArtyModel(enemy.artyGameData, enemy.level, 0L);

                    InstantiateBattler(i + 3, arty, enemy);
                }
#endif
            }
            else
            {
                for (int i = 0; i < 3; ++i)
                {
                    ArtyModel arty = ArtyRosterState.Battery[i];

                    if (arty == null)
                        continue;

                    InstantiateBattler(i, arty, null);
                }

                for (int i = 0; i < 3; ++i)
                {
                    Enemy enemy = BattleState.stageGameData.enemyList.ElementAtOrDefault(i);

                    if (enemy == null)
                        continue;

                    ArtyModel arty = new ArtyModel(enemy.artyGameData, enemy.level, 0L);

                    InstantiateBattler(i + 3, arty, enemy);
                }
            }

            progress.Report(0.98f);

            // 4. HUD 준비
            LoadingScreenManager.Inst.SetMessage("HUD를 준비하는 중...");
            Presenter.Find<GaugeHUD>().Activate();
            Presenter.Find<MoveHUD>().Activate();
            Presenter.Find<ItemHUD>().Activate();
            progress.Report(0.99f);

            // 5. 딜레이
            await UniTask.Delay(100);
            progress.Report(1.0f);

            // 6. BGM 재생
            AudioManager.Inst.PlayBGM(stageGameData.bgm);

            // 7. 배틀 루프 시작
            BattleLoop().Forget();
        }

        private async UniTaskVoid BattleLoop()
        {
            aliveBattlers.Clear();
            aliveBattlers.AddRange(battlers.Where(bat => bat));

            int turn = 0;
            int index = 0;

            int playerCount = 0;
            int enemyCount = 0;

            const int turnDelayMilliSeconds = 1000;
            while (true)
            {
                turnOwner = aliveBattlers[index];
                turnOwner.StartTurn(turn);
                await UniTask.WaitWhile(turnOwner, battler => battler.HasTurn);
                turnOwner.EndTurn();

                await UniTask.Delay(turnDelayMilliSeconds);

                aliveBattlers.ForEach(DestroyOuter);
                await UniTask.NextFrame(); // 삭제 처리 대기

                aliveBattlers.RemoveAll(IsDead);

                playerCount = aliveBattlers.Count(battler => battler.IsPlayer);
                enemyCount = aliveBattlers.Count - playerCount;

                if (playerCount == 0 || enemyCount == 0)
                    break;

                index = (index + 1) % aliveBattlers.Count;
                ++turn;
            }

            FinishBattle(playerCount, enemyCount);
            return;

            void DestroyOuter(ArtyController battler)
            {
                if (DestructibleTerrain.Inst.InFairArea(battler.transform.position) == false)
                {
                    MyDebug.Log($"배틀러 {battler.Model.DisplayName}(Lv. {battler.Model.levelRx.Value}) 낙사로 인한 삭제 처리");
                    Destroy(battler);
                }
            }

            bool IsDead(ArtyController battler)
            {
                if (battler == null)
                {
                    MyDebug.Log($"Dead Check Type 1: Component has been destroyed.");
                }
                else if (battler.gameObject == null)
                {
                    MyDebug.Log($"Dead Check Type 2: GameObject has been destroyed.");
                }
                else if (battler.CurrentHp <= 0)
                {
                    MyDebug.Log($"Dead Check Type 3: CurrentHp <= 0.");
                }

                return battler == null || battler.gameObject == null || battler.CurrentHp <= 0;
            }
        }

        public override UniTask ClearScene(IProgress<float> progress)
        {
            base.ClearScene(progress);

            foreach (var battler in battlers)
            {
                if (battler != null)
                {
                    Destroy(battler.gameObject);
                }
            }

            progress.Report(1f);
            return UniTask.CompletedTask;
        }

        private void FinishBattle(int playerCount, int enemyCount)
        {
            var popup = Presenter.Find<BattleResultPopup>();
            popup.Setup(enemyCount == 0, stageGameData);
            popup.OpenWithAnimation().Forget();
        }
    }
}