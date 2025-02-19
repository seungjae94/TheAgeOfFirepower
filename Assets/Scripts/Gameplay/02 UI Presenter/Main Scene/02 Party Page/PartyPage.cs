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

        // ���� - ������ ĳ����
        public State<CharacterModel> selectedCharacter { get; private set; } = new();

        // ���� - �巡�� ����
        public State<bool> isDraggingSlotItem { get; private set; } = new();

        public IDisposable SubscribePartyMemberSlotItemDragEvent(Action beginDragAction, Action endDragAction)
        {
            return isDraggingSlotItem.GetProperty()
                .DistinctUntilChanged()
                .Subscribe(isDragging =>
                {
                    if (isDragging)
                        beginDragAction();
                    else
                        endDragAction();
                });
        }

        // �ʱ�ȭ
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

        // ���� ��ȣ �ۿ�
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
            Debug.Log("��Ƽ ��� ��ü ��ư ������");
        }
    }
}