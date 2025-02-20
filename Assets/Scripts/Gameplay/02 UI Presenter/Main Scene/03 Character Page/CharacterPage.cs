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

        [Inject] MainSceneManager m_mainSceneManager;
        [Inject] CharacterRepository m_characterRepository;

        [SerializeField] Button m_navBackButton;
        [SerializeField] CharacterSelectionFlex m_flex;

        public State<CharacterModel> selectedCharacter { get; private set; } = new();

        SortedCharacterListSubscription m_sortedCharacterListChangeSubs;
        
        protected override void InitializeChildren()
        {
            m_flex.Initialize();
        }

        protected override void SubscribeDataChange()
        {
            m_sortedCharacterListChangeSubs = m_characterRepository
                .SubscribeSortedCharacterList(UpdateView);
        }

        protected override void SubscribeUserInteractions()
        {
            m_navBackButton.OnClickAsObservable()
                .Subscribe(_ => m_mainSceneManager.NavigateBack())
                .AddTo(gameObject);
        }

        // 유저 상호작용
        void OnClickFlexItem(CharacterModel character)
        {
            selectedCharacter.SetState(character);
            m_mainSceneManager.Navigate(EPageId.CharacterDetailPage);
        }

        // 뷰 업데이트
        void UpdateView()
        {
            UpdateFlex();
        }

        void UpdateFlex()
        {
            var itemDatas = m_characterRepository
                .GetSortedList()
                .ToList();

            m_flex.Draw(itemDatas, OnClickFlexItem);
        }
    }
}
