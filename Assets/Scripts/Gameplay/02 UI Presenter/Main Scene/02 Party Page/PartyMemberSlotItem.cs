using Mathlife.ProjectL.Utils;
using System;
using TMPro;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using VContainer;

namespace Mathlife.ProjectL.Gameplay
{
    public class PartyMemberSlotItem : Presenter<int>
    {
        [Inject] GameDataDB m_gameDataDB;
        [Inject] CharacterRepository m_characterRepository;
        [Inject] PartyPage m_partyPage;

        CanvasGroup m_canvasGroup;
        ObservableBeginDragTrigger m_beginDragTrigger;
        ObservableDragTrigger m_dragTrigger;
        ObservableEndDragTrigger m_endDragTrigger;
        ObservablePointerClickTrigger m_pointerClickTrigger;

        [SerializeField] Image m_portraitImage;
        [SerializeField] TMP_Text m_nameText;
        [SerializeField] TMP_Text m_levelText;
        [SerializeField] CanvasGroup m_selectionOverlayCanvasGroup;

        int m_slotIndex;

        CharacterModel m_character = null;
        IDisposable m_characterSubscription = null;

        [SerializeField] RectTransform m_dragItemParent;
        PartyMemberSlotDragItem m_dragItem = null;

        void Awake()
        {
            m_canvasGroup = GetComponent<CanvasGroup>();
            m_beginDragTrigger = GetComponent<ObservableBeginDragTrigger>();
            m_dragTrigger = GetComponent<ObservableDragTrigger>();
            m_endDragTrigger = GetComponent<ObservableEndDragTrigger>();
            m_pointerClickTrigger = GetComponent<ObservablePointerClickTrigger>();
        }

        protected override void Store(int slotIndex)
        {
            m_slotIndex = slotIndex;
        }

        protected override void SubscribeDataChange()
        {
            m_characterRepository.party
                .ObserveEveryValueChanged(party => party[m_slotIndex])
                .Subscribe(OnSlotMemberChange)
                .AddTo(gameObject);

            m_partyPage.selectedSlotIndex
                .SubscribeChangeEvent(OnSelectedSlotIndexChange)
                .AddTo(gameObject);
        }

        protected override void SubscribeUserInteractions()
        {
            m_beginDragTrigger
                .OnBeginDragAsObservable()
                .Subscribe(OnBeginDrag)
                .AddTo(gameObject);

            m_dragTrigger
                .OnDragAsObservable()
                .Subscribe(OnDrag)
                .AddTo(gameObject);

            m_endDragTrigger
                .OnEndDragAsObservable()
                .Subscribe(OnEndDrag)
                .AddTo(gameObject);

            m_pointerClickTrigger
                .OnPointerClickAsObservable()
                .Subscribe(OnPointerClick)
                .AddTo(gameObject);
        }

        protected override void InitializeView()
        {
            UpdateView();
        }

        // 데이터 구독 메서드
        void OnSlotMemberChange(CharacterModel character)
        {
            // i번 슬롯 캐릭터 구독
            m_character = character;

            if (m_characterSubscription != null)
                m_characterSubscription.Dispose();

            if (m_character != null)
            {
                m_characterSubscription = character.SubscribeLevelChangeEvent(UpdateLevelText);
            }

            UpdateView();
        }

        void OnSelectedSlotIndexChange(int selectedSlotIndex)
        {
            // This character is selected.
            if (selectedSlotIndex == m_slotIndex)
            {
                m_selectionOverlayCanvasGroup.Show();
            }
            // Another character is selected.
            else
            {
                m_selectionOverlayCanvasGroup.Hide();
            }
        }

        // 유저 상호 작용
        public CharacterModel GetCharacterModel()
        {
            return m_character;
        }

        void OnBeginDrag(PointerEventData eventData)
        {
            m_dragItem = m_gameDataDB.Instantiate<PartyMemberSlotDragItem>(EPrefabId.PartyMemberSlotDragItem, m_dragItemParent);
            m_dragItem.Initialize(m_character);
            m_dragItem.transform.position = transform.position;

            m_canvasGroup.HideWithAlpha(0.25f);

            m_partyPage.isDraggingSlotItem.SetState(true);
        }

        void OnDrag(PointerEventData eventData)
        {
            m_dragItem.GetComponent<RectTransform>().position = eventData.position;
        }

        void OnEndDrag(PointerEventData eventData)
        {
            m_partyPage.isDraggingSlotItem.SetState(false);

            Destroy(m_dragItem.gameObject);
            m_canvasGroup.Show();
        }

        void OnPointerClick(PointerEventData eventData)
        {
            m_partyPage.selectedSlotIndex.SetState(m_slotIndex);
        }

        // 뷰 업데이트
        public void UpdateView()
        {
            if (m_character == null)
            {
                m_canvasGroup.Hide();
                return;
            }

            m_canvasGroup.Show();
            m_portraitImage.sprite = m_character.portrait;
            m_levelText.text = m_character.level.ToString();
            m_nameText.text = m_character.displayName;
        }

        void UpdateLevelText(int value)
        {
            m_levelText.text = value.ToString();
        }
    }
}