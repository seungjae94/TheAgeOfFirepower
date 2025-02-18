using System;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace Mathlife.ProjectL.Gameplay
{
    public class HomePage : Page
    {
        [Inject] MainSceneManager m_mainSceneManager;

        [SerializeField] Button m_partyButton;
        [SerializeField] Button m_inventoryButton;
        [SerializeField] Button m_shopButton;
        [SerializeField] Button m_battleButton;

        public override EPageId pageId => EPageId.HomePage;

        public override void Initialize()
        {
            base.Initialize();
            Open();
        }

        protected override void InitializeChildren()
        {
        }

        protected override void InitializeView()
        {
        }

        protected override void SubscribeDataChange()
        {
        }

        protected override void SubscribeUserInteractions()
        {
            m_partyButton.OnClickAsObservable()
                .Subscribe(_ => m_mainSceneManager.Navigate(EPageId.TeamPage));

            m_inventoryButton.OnClickAsObservable()
                .Subscribe(_ => m_mainSceneManager.Navigate(EPageId.InventoryPage));

            m_shopButton.OnClickAsObservable()
                .Subscribe(_ => m_mainSceneManager.Navigate(EPageId.ShopPage));

            m_battleButton.OnClickAsObservable()
                .Subscribe(_ => m_mainSceneManager.Navigate(EPageId.StageSelectionPage));
        }
    }
}
