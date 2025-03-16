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

        public override void Open()
        {
            base.Open();

            // Overlay
            // TODO: UserInfo 구현 및 Open
            Find<CurrencyBar>().Activate();
            
            // 뷰 초기화
            menuBar.Draw();
        }

        public override void Close()
        {
            base.Close();
            
            // 뷰 정리
            menuBar.Clear();
        }
    }
}