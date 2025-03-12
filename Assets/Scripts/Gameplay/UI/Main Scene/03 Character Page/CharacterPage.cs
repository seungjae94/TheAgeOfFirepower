using System.Linq;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace Mathlife.ProjectL.Gameplay
{
    public class CharacterPage : Page
    {
        public override EPageId pageId => EPageId.CharacterPage;

        [Inject] LobbySceneGameMode lobbySceneGameMode;
        [Inject] CharacterRosterState characterRosterState;

        [SerializeField] Button m_navBackButton;
        [SerializeField] CharacterSelectionFlex m_flex;

        public readonly ReactiveProperty<CharacterModel> selectedCharacterRx = new();

        SortedCharacterListSubscription m_sortedCharacterListChangeSubs;
        
        protected override void InitializeChildren()
        {
            m_flex.Initialize();
        }

        protected override void SubscribeDataChange()
        {
            m_sortedCharacterListChangeSubs = characterRosterState
                .SubscribeSortedCharacterList(UpdateView);
        }

        protected override void SubscribeUserInteractions()
        {
            m_navBackButton.OnClickAsObservable()
                .Subscribe(_ => lobbySceneGameMode.NavigateBack())
                .AddTo(gameObject);
        }

        // 유저 상호작용
        void OnClickFlexItem(CharacterModel character)
        {
            selectedCharacterRx.Value = character;
            lobbySceneGameMode.Navigate(EPageId.CharacterDetailPage);
        }

        // 뷰 업데이트
        void UpdateView()
        {
            UpdateFlex();
        }

        void UpdateFlex()
        {
            var itemDatas = characterRosterState
                .GetSortedList()
                .ToList();

            m_flex.Draw(itemDatas, OnClickFlexItem);
        }
    }
}
