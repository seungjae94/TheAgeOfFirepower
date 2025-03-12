using System;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace Mathlife.ProjectL.Gameplay
{
    public class HomePage : Page
    {
        [Inject] LobbySceneGameMode lobbySceneGameMode;

        [SerializeField] Button m_partyButton;
        [SerializeField] Button m_characterButton;
        [SerializeField] Button m_inventoryButton;
        [SerializeField] Button m_shopButton;
        [SerializeField] Button m_battleButton;

        public override EPageId pageId => EPageId.HomePage;

        public override void Initialize()
        {
            base.Initialize();
            Open();
        }

        protected override void SubscribeUserInteractions()
        {
            m_partyButton.OnClickAsObservable()
                .Subscribe(_ => lobbySceneGameMode.Navigate(EPageId.PartyPage))
                .AddTo(gameObject);

            m_characterButton.OnClickAsObservable()
                .Subscribe(_ => lobbySceneGameMode.Navigate(EPageId.CharacterPage))
                .AddTo(gameObject);

            m_inventoryButton.OnClickAsObservable()
                .Subscribe(_ => lobbySceneGameMode.Navigate(EPageId.InventoryPage))
                .AddTo(gameObject);

            m_shopButton.OnClickAsObservable()
                .Subscribe(_ => lobbySceneGameMode.Navigate(EPageId.ShopPage))
                .AddTo(gameObject);

            m_battleButton.OnClickAsObservable()
                .Subscribe(_ => lobbySceneGameMode.Navigate(EPageId.StageSelectionPage))
                .AddTo(gameObject);
        }
    }
}
