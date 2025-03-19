using System;
using Mathlife.ProjectL.Utils;
using UniRx;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Mathlife.ProjectL.Gameplay.UI
{
    public class HomePage : Page
    {
        public override string PageName => "홈";
        
        HomePageMenuBar menuBar;

        public override void Initialize()
        {
            base.Initialize();
            
            menuBar = transform.FindRecursive<HomePageMenuBar>();
        }
        
        protected override void OnOpen()
        {
            // Overlay
            // TODO: UserInfo 구현 및 Open
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