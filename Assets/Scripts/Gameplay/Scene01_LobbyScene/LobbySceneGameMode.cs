using System;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Mathlife.ProjectL.Gameplay
{
    public class LobbySceneGameMode : GameMode<LobbySceneGameMode>
    {
        protected override SingletonLifeTime LifeTime => SingletonLifeTime.Scene;
        
        PageNavigator m_pageNavigator = new();

        protected override async UniTask InitializeScene(IProgress<float> progress)
        {
            // 1. UI Loading
            foreach (var presenter in FindObjectsByType<PresenterBase>(FindObjectsSortMode.None))
            {
                //LifetimeScope.Find<MainSceneLifetimeScope>().Container.Inject(presenter);
            }
            //m_loadingScreen.SetProgress(0.25f);
            
            // 로딩 테스트
            await UniTask.Delay(100);
            progress.Report(0.1f);
            
            await UniTask.Delay(100);
            progress.Report(0.2f);
            
            await UniTask.Delay(100);
            progress.Report(0.3f);
            
            await UniTask.Delay(100);
            progress.Report(0.4f);
            
            await UniTask.Delay(100);
            progress.Report(0.5f);
            
            await UniTask.Delay(500);
            progress.Report(0.75f);
            
            await UniTask.Delay(500);
            progress.Report(1.0f);

            //Page[] pages = FindObjectsByType<Page>(FindObjectsSortMode.None);
            //m_pageNavigator.AddPages(pages);
            //m_loadingScreen.SetProgress(0.5f);
            //
            //// 3. 로딩 스크린 숨기기
            //await UniTask.Delay(100);
            //m_loadingScreen.Hide();
        }

        public void NavigateHome()
        {
            m_pageNavigator.Home();
        }

        public void NavigateBack()
        {
            m_pageNavigator.Back();
        }

        public void Navigate(EPageId pageId)
        {
            m_pageNavigator.Navigate(pageId);
        }

        public Page GetPreviousPage()
        {
            return m_pageNavigator.GetPreviousPage();
        }
    }
}
