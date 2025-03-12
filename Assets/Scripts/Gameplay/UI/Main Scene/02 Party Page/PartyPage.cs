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

        [Inject] LobbySceneGameMode lobbySceneGameMode;
        [Inject] CharacterRosterState characterRosterState;

        [SerializeField] Button m_navBackButton;
        [SerializeField] Transform m_partySlotsParent;
        [SerializeField] PartySelectedCharacter m_selectedCharacterView;
        [SerializeField] PartyMemberChangeModal m_partyMemberChangeModal;
        [SerializeField] PartyValidationModal m_partyValidationModal;

        List<PartyMemberSlot> m_partySlots = new();

        // �� ���� ���� ���
        public PartyMemberChangeModal partyMemberChangeModal => m_partyMemberChangeModal;


        // ���� - ������ ĳ����
        //public State<int> selectedSlotIndex { get; private set; } = new();

        public readonly ReactiveProperty<int> selectedSlotIndexRx = new(-1);
        //public int SelectedSlotIndex { get => selectedSlotIndexRxProp.Value; set => selectedSlotIndexRxProp.Value = value; }
        //public IObservable<int> SelectedSlotIndexRxProp => selectedSlotIndexRxProp;
        
        public bool IsSelectedSlotIndexInRange()
        {
            return selectedSlotIndexRx.Value >= 0 && selectedSlotIndexRx.Value < m_partySlots.Count;
        }

        public CharacterModel GetSelectedCharacter()
        {
            if (IsSelectedSlotIndexInRange() == false)
                return null;

            return characterRosterState.party[selectedSlotIndexRx.Value];
        }

        // ���� - �巡�� ����
        public readonly ReactiveProperty<bool> isDraggingSlotItemRx = new(false);

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
            selectedSlotIndexRx.Value = -1;
            base.Open();
        }

        async void OnClickBackButton()
        {
            if (characterRosterState.party.Validate())
                lobbySceneGameMode.NavigateBack();
            else
                await m_partyValidationModal.Show();
        }

        public void OnClickDetailInfoButton(Unit _)
        {
            lobbySceneGameMode.Navigate(EPageId.CharacterDetailPage);
        }

        public async void OnClickPartyMemberChangeButton(Unit _)
        {
            await m_partyMemberChangeModal.Show();
        }
    }
}