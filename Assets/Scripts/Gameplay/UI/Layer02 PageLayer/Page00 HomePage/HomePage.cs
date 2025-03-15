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
        HomePageMenuBar menuBar;

        private void Awake()
        {
            menuBar = transform.FindRecursive<HomePageMenuBar>();
        }

        public override void Open()
        {
            base.Open();

            // Page Overlay
            // TODO: UserInfo 구현 및 Open
            Find<CurrencyBar>().Open();
            
            // View 초기화
            menuBar.Initialize();
        }
        
        private void Start()
        {
        }
    }
}