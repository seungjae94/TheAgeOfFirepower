using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using VContainer;

namespace Mathlife.ProjectL.Gameplay
{
    public class ShopPage : Page
    {
        public override EPageId pageId => EPageId.ShopPage;

        [SerializeField] NavigateBackBarView m_navigateBackBar;
        [SerializeField] IngameCurrencyBarPresenter m_ingameCurrencyBar;

        // 초기화
        public override void Initialize()
        {
            InitializeChildren();
            Close();
        }

        protected override void InitializeChildren()
        {
            m_navigateBackBar.Initialize(OnClickNavigateBackButton);
            m_ingameCurrencyBar.Initilize();
        }

        // 유저 상호작용
        void OnClickNavigateBackButton()
        {
            m_worldSceneManager.NavigateBack();
        }
    }
}
