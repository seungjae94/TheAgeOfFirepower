using Mathlife.ProjectL.Utils;
using UnityEngine;
using UnityEngine.Serialization;

namespace Mathlife.ProjectL.Gameplay.UI
{
    public class HomePage : Page
    {
        public override string PageName => "홈";
        
        // TODO: private UserProfile profile;
        private HomePageMenuBar menuBar;

        public override void Initialize()
        {
            base.Initialize();
            
            menuBar = transform.FindRecursive<HomePageMenuBar>();
        }
        
        protected override void OnOpen()
        {
            // Overlay
            Find<CurrencyBar>().Activate();
            
            // 뷰 초기화
            menuBar.Draw();
        }
        
        protected override void OnClose()
        {
            // 뷰 정리
            menuBar.Clear();
        }
    }
}