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
        private List<TestArtyModel> developPlayers = new();
        
        [SerializeField]
        private List<TestArtyModel> developEnemys = new();

        [SerializeField]
        private AssetReferenceT<StageGameData> developStageGameDataRef = null;
#endif

        // Field
        public ArtyController turnOwner { get; private set; }
        private bool developMode = false;
        private readonly List<ArtyController> battlers = new();

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

            void InstantiateBattler(bool isPlayer, ArtyModel arty, int spawnIndex)
            {
                GameObject inst = Instantiate(arty.BattlerPrefab);
                inst.transform.position = new Vector3(stageGameData.spawnXs[spawnIndex], 15f, 0f);
                var battler = inst.GetComponent<ArtyController>();
                battler.Setup(isPlayer, arty);
                battlers.Add(battler);
            }
            
            if (developMode)
            {
                for (int i = 0; i < Mathf.Min(developPlayers.Count, 3); ++i)
                {
                    var player = developPlayers[i];
                    var arty = new ArtyModel(player.artyGameData, player.level, 0L);
                    
                    InstantiateBattler(true, arty, i);
                }
                
                for (int i = 0; i < Mathf.Min(developEnemys.Count, 3); ++i)
                {
                    var enemy = developEnemys[i];
                    var arty = new ArtyModel(enemy.artyGameData, enemy.level, 0L);
                    
                    InstantiateBattler(false, arty, i + 3);
                }
            }
            else
            {
                for (int i = 0; i < 3; ++i)
                {
                    ArtyModel arty = ArtyRosterState.Battery[i];

                    if (arty == null)
                        continue;

                    InstantiateBattler(true, arty, i);
                }

                for (int i = 0; i < 3; ++i)
                {
                    Enemy enemy = BattleState.StageGameData.enemyList.ElementAtOrDefault(i);

                    if (enemy == null)
                        continue;
                    
                    ArtyModel arty = new ArtyModel(enemy.artyGameData, enemy.level, 0L);
                    
                    InstantiateBattler(false, arty, i + 3);
                }
            }

            progress.Report(0.7f);

            // 4. HUD 준비
            Presenter.Find<FireHUD>().Activate();
            Presenter.Find<MoveHUD>().Activate();
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