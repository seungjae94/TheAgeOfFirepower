using Mathlife.ProjectL.Utils;
using System;
using TMPro;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;
using VContainer;

namespace Mathlife.ProjectL.Gameplay
{
    public class PartyMemberSlotItem : Presenter<int>
    {
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

        [SerializeField] RectTransform m_dragItemParent;

        int m_slotIndex;

        CharacterModel m_character = null;
        IDisposable m_characterSubscription = null;
        DragCharacterCardView m_dragCard = null;

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

            m_partyPage.selectedCharacter
                .SubscribeChangeEvent(OnSelectedCharacterChange)
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
            if (m_character == null)
            {
                Hide();
                return;
            }

            Show();
            UpdateView();
        }

        // 데이터 구독 메서드
        void OnSlotMemberChange(CharacterModel character)
        {
            // i번 슬롯 캐릭터 구독
            m_character = character;

            if (m_characterSubscription != null)
                m_characterSubscription.Dispose();

            if (m_character == null)
            {
                Hide();
                return;
            }

            m_characterSubscription = character.SubscribeLevelChangeEvent(OnLevelChanged);

            UpdateView();
        }

        void OnSelectedCharacterChange(CharacterModel selectedCharacter)
        {
            // This character is selected.
            if (m_character == selectedCharacter)
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
        public void Hide()
        {
            m_canvasGroup.Hide();
        }

        public void Show()
        {
            m_canvasGroup.Show();
        }

        public void UpdateView()
        {
            m_portraitImage.sprite = m_character.portrait;
            m_levelText.text = m_character.level.ToString();
            m_nameText.text = m_character.displayName;
        }

        public CharacterModel GetCharacterModel()
        {
            return m_character;
        }

        void OnBeginDrag(PointerEventData eventData)
        {
            //m_dragCard = InstantiateWithInjection<DragCharacterCardPresenter>(EPrefabId.DragCharacterCard, m_dragArea);
            //m_dragCard.Initialize(m_character);
            //m_dragCard.transform.position = transform.position;

            //m_canvasGroup.alpha = 0.25f;
            //m_canvasGroup.blocksRaycasts = false;
            //m_canvasGroup.interactable = false;

            //if (m_characterRepository.team.Contains(m_character))
            //    m_worldSceneManager.GetPage<PartyPage>().isDraggingMemberCard = true;
        }

        void OnDrag(PointerEventData eventData)
        {
            m_dragCard.GetComponent<RectTransform>().position = eventData.position;
        }

        void OnEndDrag(PointerEventData eventData)
        {
            //if (m_worldSceneManager.GetPage<PartyPage>().isDraggingMemberCard)
            //    m_worldSceneManager.GetPage<PartyPage>().isDraggingMemberCard = false;

            //Destroy(m_dragCard.gameObject);
            //m_canvasGroup.alpha = 1.0f;
            //m_canvasGroup.blocksRaycasts = true;
            //m_canvasGroup.interactable = true;
        }

        void OnPointerClick(PointerEventData eventData)
        {
            m_partyPage.selectedCharacter.SetState(m_character);
        }

        void OnLevelChanged(int value)
        {
            m_levelText.text = value.ToString();
        }
    }
}