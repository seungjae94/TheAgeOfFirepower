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

        public override EPageId pageId => EPageId.HomePage;

        public override void Initialize()
        {
            m_partyButton.OnClickAsObservable()
                .Subscribe(_ => m_worldSceneManager.Navigate(EPageId.TeamPage));

            m_inventoryButton.OnClickAsObservable()
                .Subscribe(_ => m_worldSceneManager.Navigate(EPageId.InventoryPage));
        }

        protected override void InitializeChildren()
        {
        }
    }
}
