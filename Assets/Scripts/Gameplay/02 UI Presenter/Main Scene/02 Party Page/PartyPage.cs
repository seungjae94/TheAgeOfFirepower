using System.Collections.Generic;
using UniRx;
using VContainer;
using System;
using UnityEngine;
using Cysharp.Threading.Tasks;
using UnityEngine.UI;

namespace Mathlife.ProjectL.Gameplay
{
    public class PartyPage : Page
    {
        public override EPageId pageId => EPageId.PartyPage;

        [Inject] MainSceneManager m_mainSceneManager;
        [Inject] CharacterRepository m_characterRepository;

        [SerializeField] Button m_navBackButton;
        [SerializeField] Transform m_partySlotsParent;
        [SerializeField] PartySelectedCharacter m_selectedCharacterView;
        [SerializeField] PartyMemberChangeModal m_partyMemberChangeModal;
        [SerializeField] PartyValidationModal m_partyValidationModal;

        List<PartyMemberSlot> m_partySlots = new();

        // �� ���� ���� ���
        public PartyMemberChangeModal partyMemberChangeModal => m_partyMemberChangeModal;


        // ���� - ������ ĳ����
        public State<int> selectedSlotIndex { get; private set; } = new();

        public bool IsSelectedSlotIndexInRange()
        {
            return selectedSlotIndex.GetState() >= 0 && selectedSlotIndex.GetState() < m_partySlots.Count;
        }

        public CharacterModel GetSelectedCharacter()
        {
            if (IsSelectedSlotIndexInRange() == false)
                return null;

            return m_characterRepository.party[selectedSlotIndex.GetState()];
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
            for (int i = 0; i < m_partySlots.Count; ++i)
            {
                m_partySlots[i].Initialize();
            }

            m_selectedCharacterView.Initialize();
            m_partyMemberChangeModal.Initialize();
            m_partyValidationModal.Initialize();
        }

        protected override void SubscribeUserInteractions()
        {
            m_navBackButton.OnClickAsObservable()
                 .Subscribe(_ => OnClickBackButton())
                 .AddTo(gameObject);
        }

        // ���� ��ȣ �ۿ�
        public override void Open()
        {
            selectedSlotIndex.SetState(-1);
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
            m_mainSceneManager.Navigate(EPageId.CharacterDetailPage);
        }

        public async void OnClickPartyMemberChangeButton(Unit _)
        {
            await m_partyMemberChangeModal.Show();
        }
    }
}