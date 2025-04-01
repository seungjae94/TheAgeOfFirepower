using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Mathlife.ProjectL.Gameplay.ObjectBase;
using Mathlife.ProjectL.Gameplay.Play;
using Mathlife.ProjectL.Gameplay.UI;
using Sirenix.Utilities;
using UnityEngine;

namespace Mathlife.ProjectL.Gameplay
{
    public class PlaySceneGameMode : GameMode<PlaySceneGameMode>
    {
        protected override SingletonLifeTime LifeTime => SingletonLifeTime.Scene;

        // Alias
        private ArtyRosterState ArtyRosterState => GameState.Inst.artyRosterState;
        private BattleState BattleState => GameState.Inst.battleState;

        // Depedency
        [SerializeField]
        private List<ArtyController> artyControllers = new();

#if UNITY_EDITOR
        [SerializeField]
        private List<DevelopArtyData> developArtyList = new();

        [SerializeField]
        private Sprite developMapSprite;
#endif

        public override async UniTask InitializeScene(IProgress<float> progress)
        {
            // 0. 게임 모드 공통 초기화 로직 수행
            await base.InitializeScene(progress);
            progress.Report(0.1f);

            // 1. 모든 UI 닫아놓기
            PlayCanvas.Inst.DeactivateAllPresenters();
            progress.Report(0.2f);

            // 2. 모드 세팅
            bool developMode = (BattleState.StageGameData == null);

            // 3. 맵 생성
            Sprite mapSprite = developMode ? developMapSprite : BattleState.StageGameData.mapSprite; 
            DestructibleTerrain.Inst.GenerateTerrain(mapSprite);
            await UniTask.NextFrame();
            progress.Report(0.6f);
            
            // 3. 플레이어 및 적 준비
            Debug.Assert(artyControllers.Count == 6);
            if (developMode)
            {
                Debug.Assert(developArtyList.Count == 6);
                
                for (int i = 0; i < 6; ++i)
                {
                    var arty = developArtyList[i];
                    bool artyEmpty = (arty == null) || (arty.artyGameData == null);
                    artyControllers[i].gameObject.SetActive(!artyEmpty);

                    if (artyEmpty)
                        continue;
                        
                    artyControllers[i].Setup(arty);
                }
            }
            else
            {
                for (int i = 0; i < 3; ++i)
                {
                    ArtyModel playerArty = ArtyRosterState.Battery[i];
                    
                    artyControllers[i].gameObject.SetActive(playerArty != null);

                    if (playerArty == null)
                        continue;
                    
                    artyControllers[i].Setup(playerArty);
                }

                for (int i = 0; i < 3; ++i)
                {
                    Enemy enemy = BattleState.StageGameData.enemyList.ElementAtOrDefault(i);
                    
                    artyControllers[i + 3].gameObject.SetActive(enemy != null);

                    if (enemy == null)
                        continue;
                    
                    artyControllers[i + 3].Setup(enemy);
                }
            }
            progress.Report(0.7f);

            // 4. HUD 준비
            //Presenter.Find<HomePage>().Open();
            progress.Report(0.8f);

            // 5. 딜레이
            await UniTask.Delay(100);
            progress.Report(1.0f);

            // 6. 배틀 루프 시작
            //BattleLoop().Forget();
        }

        private async UniTaskVoid BattleLoop()
        {
            Debug.Assert(artyControllers.Count == 6);

            int turn = 0;
            while (true)
            {
                //await turn.Logic();

                //TurnBase turn = CreateTurn(/*The fighter of this order*/);
                // //await turn.BeginTurn();
                //
                // while (true)
                // {
                //     Decision decision = await turn.MakeDecision();
                //     await turn.DispatchDecision(decision);
                //
                //     if (decision.Type == EDecisionType.Fire 
                //         || decision.Type == EDecisionType.Skip)
                //         break;
                // }
                //
                // TurnResult turnResult = await turn.EndTurn();
                //
                // if (turnResult.battleFinished)
                //     break;

                ++turn;
            }

            FinishBattle().Forget();
        }

        private async UniTaskVoid FinishBattle()
        {
        }
    }
}