using Cysharp.Threading.Tasks;
using Mathlife.ProjectL.Utils;
using UnityEngine;
using UnityEngine.Serialization;

namespace Mathlife.ProjectL.Gameplay.UI
{
    public class HomePage : Page
    {
        public override string PageName => "홈";
        
        [SerializeField]
        private HomePageUserProfile profile;
        
        [SerializeField]
        private HomePageMenuBar menuBar;
        
        protected override void OnOpen()
        {
            // Overlay
            Find<TopBar>().Activate();
            
            // 뷰 초기화
            profile.Draw();
            menuBar.Draw();
        }
        
        protected override void OnClose()
        {
            // 뷰 정리
            profile.Clear();
            menuBar.Clear();
        }
    }
}