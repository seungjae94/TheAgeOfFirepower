using System;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Mathlife.ProjectL.Gameplay
{
    public class HomePage : Page
    {
        [SerializeField] Button m_partyButton;
        [SerializeField] Button m_inventoryButton;
        [SerializeField] Button m_shopButton;
        [SerializeField] Button m_battleButton;

        public override EPageId pageId => EPageId.HomePage;

        public override void Initialize()
        {
            m_partyButton.OnClickAsObservable()
                .Subscribe(_ => m_worldSceneManager.Navigate(EPageId.TeamPage));

            m_inventoryButton.OnClickAsObservable()
                .Subscribe(_ => m_worldSceneManager.Navigate(EPageId.InventoryPage));

            m_shopButton.OnClickAsObservable()
                .Subscribe(_ => m_worldSceneManager.Navigate(EPageId.ShopPage));

            m_battleButton.OnClickAsObservable()
                .Subscribe(_ => m_worldSceneManager.Navigate(EPageId.StageSelectionPage));
        }

        protected override void InitializeChildren()
        {
        }
    }
}
