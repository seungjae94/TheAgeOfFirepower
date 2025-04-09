using System;
using Cysharp.Threading.Tasks;
using Mathlife.ProjectL.Gameplay.ObjectBase;
using Mathlife.ProjectL.Gameplay.UI;

namespace Mathlife.ProjectL.Gameplay
{
    public class LobbySceneGameMode : GameMode<LobbySceneGameMode>
    {
        protected override SingletonLifeTime LifeTime => SingletonLifeTime.Scene;
        
        public override async UniTask InitializeScene(IProgress<float> progress)
        {
            // 0. 게임 모드 공통 초기화 로직 수행
            await base.InitializeScene(progress);
            progress.Report(0.1f);
            
            // 1. 모든 UI 닫아놓기
            LobbyCanvas.Inst.DeactivateAllPresenters();
            progress.Report(0.3f);
            
            // 2. 홈 페이지 열기
            Presenter.Find<HomePage>().Open();
            if (GameManager.Inst.PrevSceneName == SceneNames.PlayScene)
            {
                Presenter.Find<StageSelectionPage>().Open();
            }
            progress.Report(0.6f);
            
            // 3. 딜레이
            await UniTask.Delay(100);
            progress.Report(1.0f);
        }
    }
}
