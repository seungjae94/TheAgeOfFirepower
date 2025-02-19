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

        [SerializeField] SimpleActionButton m_navigateBackButton;
        [SerializeField] List<PartyMemberSlot> m_slots;
        [SerializeField] PartySelectedCharacter m_selectedCharacterView;
        [SerializeField] PartyMemberChangeModal m_partyMemberChangeModal;
        [SerializeField] PartyValidationModal m_partyValidationModal;

        // 상태 - 선택한 캐릭터
        public State<CharacterModel> selectedCharacter { get; private set; } = new();

        // 상태 - 드래그 상태
        BoolReactiveProperty m_isDraggingPartyMemberSlotItem = new(false);
        public bool isDraggingMemberSlotItem
        {
            get => m_isDraggingPartyMemberSlotItem.Value;
            set => m_isDraggingPartyMemberSlotItem.Value = value;
        }
        public CompositeDisposable SubscribePartyMemberSlotItemDragEvent(Action beginDragAction, Action endDragAction)
        {
            CompositeDisposable subscriptions = new();

            IDisposable beginDragSubscription = m_isDraggingPartyMemberSlotItem
                .Where(v => v)
                .Subscribe(_ => beginDragAction?.Invoke());

            IDisposable endDragSubscription = m_isDraggingPartyMemberSlotItem
                .Where(v => !v)
                .Subscribe(_ => endDragAction?.Invoke());

            subscriptions.Add(beginDragSubscription);
            subscriptions.Add(endDragSubscription);
            return subscriptions;
        }

        // 초기화
        protected override void InitializeChildren()
        {
            m_navigateBackButton.Initialize(OnClickBackButton);

            for (int i = 0; i < m_slots.Count; ++i)
            {
                m_slots[i].Initialize();
            }

            m_selectedCharacterView.Initialize();
            //m_characterSelectionModal.Initialize();
            m_partyValidationModal.Initialize();
        }

        protected override void SubscribeDataChange()
        {
        }

        protected override void SubscribeUserInteractions()
        {
        }

        protected override void InitializeView()
        {
        }

        // 유저 상호 작용
        public override void Open()
        {
            selectedCharacter.SetState(null);
            base.Open();
        }

        async void OnClickBackButton()
        {
            if (m_characterRepository.party.Validate())
                m_mainSceneManager.NavigateBack();
            else
                await m_partyValidationModal.Show();
        }

        public void OnClickDetailInfoButton(Unit _)
        {
            m_mainSceneManager.Navigate(EPageId.CharacterPage);
        }

        public void OnClickPartyMemberChangeButton(Unit _)
        {
            Debug.Log("파티 멤버 교체 버튼 눌렀음");
        }
    }
}