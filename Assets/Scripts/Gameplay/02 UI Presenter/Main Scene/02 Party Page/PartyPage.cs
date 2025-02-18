using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine.UI;
using Mathlife.ProjectL.Utils;
using UniRx;
using UniRx.Triggers;
using VContainer;
using UnityEngine.EventSystems;
using System;
using UnityEngine;

namespace Mathlife.ProjectL.Gameplay
{
    public class PartyPage : Page
    {
        public override EPageId pageId => EPageId.TeamPage;

        [Inject] MainSceneManager m_mainSceneManager;
        [Inject] CharacterRepository m_characterRepository;

        [SerializeField] SimpleActionButton m_navigateBackBar;
        [SerializeField] List<CharacterSlotPresenter> m_slots;
        [SerializeField] PartyPageSelectedCharacterView m_selectedCharacterView;
        [SerializeField] PartyMemberChangeModal m_partyMemberChangeModal;
        [SerializeField] PartyValidationModal m_partyValidationModal;

        // 상태 - 선택한 캐릭터
        ReactiveProperty<CharacterModel> m_selectedCharacter = new(null);
        public CharacterModel selectedCharacter { get => m_selectedCharacter.Value; set => m_selectedCharacter.Value = value; }

        public IDisposable SubscribeSelectedCharacterChangeEvent(Action<CharacterModel> action)
        {
            return m_selectedCharacter.Subscribe(action);
        }

        // 상태 - 드래그 상태
        BoolReactiveProperty m_isDraggingMemberCard = new(false);
        public bool isDraggingMemberCard
        {
            get => m_isDraggingMemberCard.Value;
            set => m_isDraggingMemberCard.Value = value;
        }
        public CompositeDisposable SubscribeDragMemberCardEvent(Action beginDragAction, Action endDragAction)
        {
            CompositeDisposable subscriptions = new();

            IDisposable beginDragSubscription = m_isDraggingMemberCard
                .Where(v => v)
                .Subscribe(_ => beginDragAction?.Invoke());

            IDisposable endDragSubscription = m_isDraggingMemberCard
                .Where(v => !v)
                .Subscribe(_ => endDragAction?.Invoke());

            subscriptions.Add(beginDragSubscription);
            subscriptions.Add(endDragSubscription);
            return subscriptions;
        }

        // 초기화
        public override void Initialize()
        {
            InitializeChildren();

            Close();
        }

        protected override void InitializeChildren()
        {
            m_navigateBackBar.Initialize(OnClickBackButton);

            for (int i = 0; i < Constants.TeamMemberMaxCount; ++i)
            {
                m_slots[i].Initialize();
            }

            SubscribeSelectedCharacterChangeEvent(selected => m_selectedCharacterView.Render(selected))
                .AddTo(gameObject);

            m_selectedCharacterView.BindEvents(OnClickDetailInfoButton, OnClickPartyMemberChangeButton);
            //m_characterSelectionModal.Initialize();
            m_partyValidationModal.Initialize();
        }

        // 유저 상호 작용
        public override void Open()
        {
            m_selectedCharacter = null;
            m_selectedCharacterView.Render(null);

            base.Open();
        }

        async void OnClickBackButton()
        {
            if (m_characterRepository.party.Validate())
                m_mainSceneManager.NavigateBack();
            else
                await m_partyValidationModal.Show();
        }

        void OnClickDetailInfoButton()
        {
            m_mainSceneManager.Navigate(EPageId.CharacterPage);
        }

        void OnClickPartyMemberChangeButton()
        {

        }

        protected override void SubscribeDataChange()
        {
            throw new NotImplementedException();
        }

        protected override void SubscribeUserInteractions()
        {
            throw new NotImplementedException();
        }

        protected override void InitializeView()
        {
            throw new NotImplementedException();
        }
    }
}