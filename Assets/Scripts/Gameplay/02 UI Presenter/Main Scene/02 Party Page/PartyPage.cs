using System.Collections.Generic;
using UniRx;
using VContainer;
using System;
using UnityEngine;
using Cysharp.Threading.Tasks;

namespace Mathlife.ProjectL.Gameplay
{
    public class PartyPage : Page
    {
        public override EPageId pageId => EPageId.TeamPage;

        [Inject] MainSceneManager m_mainSceneManager;
        [Inject] CharacterRepository m_characterRepository;

        [SerializeField] SimpleActionButton m_navigateBackButton;
        [SerializeField] Transform m_partySlotsParent;
        [SerializeField] PartySelectedCharacter m_selectedCharacterView;
        [SerializeField] PartyMemberChangeModal m_partyMemberChangeModal;
        [SerializeField] PartyValidationModal m_partyValidationModal;

        List<PartyMemberSlot> m_partySlots = new();

        // �� ���� ���� ���
        public PartyMemberChangeModal partyMemberChangeModal => m_partyMemberChangeModal;


        // ���� - ������ ĳ����
        public State<CharacterModel> selectedCharacter { get; private set; } = new();
        
        public int GetSelectedSlotIndex()
        {
            if (selectedCharacter.GetState() == null)
                return -1;

            return m_characterRepository.party.IndexOf(selectedCharacter.GetState());
        }

        public void ChangePartyMemberAtSelectedSlot(CharacterModel character)
        {
            int selectedSlotIndex = GetSelectedSlotIndex();
            m_characterRepository.party.Add(selectedSlotIndex, character);
        }

        // ���� - �巡�� ����
        public State<bool> isDraggingSlotItem { get; private set; } = new();

        // �ʱ�ȭ
        protected override void Awake()
        {
            base.Awake();

            foreach(Transform m_partySlotTrans in m_partySlotsParent)
            {
                m_partySlots.Add(m_partySlotTrans.GetComponent<PartyMemberSlot>());
            }
        }

        protected override void InitializeChildren()
        {
            m_navigateBackButton.Initialize(OnClickBackButton);

            for (int i = 0; i < m_partySlots.Count; ++i)
            {
                m_partySlots[i].Initialize();
            }

            m_selectedCharacterView.Initialize();
            m_partyMemberChangeModal.Initialize();
            m_partyValidationModal.Initialize();
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

        public async void OnClickPartyMemberChangeButton(Unit _)
        {
            await m_partyMemberChangeModal.Show();
        }
    }
}