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
        private readonly List<ArtyController> battlers = new();

        public ImmutableList<ArtyController> AlivePlayers => battlers
            .Where(battler => battler.IsPlayer)
            .ToImmutableList();
        
        public override async UniTask InitializeScene(IProgress<float> progress)
        {
            // 0. 게임 모드 공통 초기화 로직 수행
            await base.InitializeScene(progress);
            progress.Report(0.1f);

            // 1. 모든 UI 닫아놓기
            PlayCanvas.Inst.DeactivateAllPresenters();
            progress.Report(0.2f);

            // 2. 모드 세팅
            developMode = BattleState.StageGameData == false;

            StageGameData stageGameData = BattleState.StageGameData;
            if (developMode)
            {
                stageGameData = await developStageGameDataRef.LoadAssetAsync();
            }

            // 3. 맵 생성
            Sprite mapSprite = stageGameData.mapSprite;
            DestructibleTerrain.Inst.GenerateTerrain(mapSprite);
            await UniTask.NextFrame();
            await UniTask.NextFrame();
            progress.Report(0.6f);

            // 3. 플레이어 및 적 준비
            battlers.Clear();

            void InstantiateBattler(int spawnIndex, ArtyModel arty, Enemy enemy)
            {
                GameObject inst = Instantiate(arty.BattlerPrefab);
                inst.transform.position = new Vector3(stageGameData.spawnXs[spawnIndex], 15f, 0f);
                var battler = inst.GetComponent<ArtyController>();
                battler.Setup(arty, enemy);
                battlers.Add(battler);
            }

            if (developMode)
            {
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
                    Enemy enemy = BattleState.StageGameData.enemyList.ElementAtOrDefault(i);

                    if (enemy == null)
                        continue;

                    ArtyModel arty = new ArtyModel(enemy.artyGameData, enemy.level, 0L);

                    InstantiateBattler(i + 3, arty, enemy);
                }
            }

            progress.Report(0.7f);

            // 4. HUD 준비
            Presenter.Find<FireHUD>().Activate();
            Presenter.Find<MoveHUD>().Activate();
            Presenter.Find<FuelHUD>().Activate();
            progress.Report(0.8f);

            // 5. 딜레이
            await UniTask.Delay(100);
            progress.Report(1.0f);

            // 6. 배틀 루프 시작
            BattleLoop().Forget();
        }

        private async UniTaskVoid BattleLoop()
        {
            List<ArtyController> aliveBattlers = battlers
                .Where(bat => bat)
                .ToList();

            int turn = 0;
            int index = 0;
            const int turnDelayMilliSeconds = 1500;
            while (true)
            {
                turnOwner = battlers[index];
                turnOwner.StartTurn(turn);
                await UniTask.WaitWhile(turnOwner, battler => battler.HasTurn);
                turnOwner.EndTurn();
                
                // TODO: 턴 결과 집계
                aliveBattlers.RemoveAll(battler => !battler);
                index = (index + 1) % battlers.Count;
                ++turn;

                await UniTask.Delay(turnDelayMilliSeconds);
            }

            FinishBattle().Forget();
        }

        private async UniTaskVoid FinishBattle()
        {
        }
    }
}