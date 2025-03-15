using Mathlife.ProjectL.Utils;
using System;
using TMPro;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Mathlife.ProjectL.Gameplay.UI
{
    public class BatteryPageArtySlotItem : MonoBehaviour, IView
    {
        // Alias
        BatteryPage BatteryPage => Presenter.Find<BatteryPage>();
        ArtyRosterState ArtyRosterState => GameState.Inst.artyRosterState;

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

        ArtyModel arty = null;
        IDisposable m_characterSubscription = null;

        [SerializeField] RectTransform m_dragItemParent;
        BatteryPageArtySlotDragItem m_dragItem = null;

        public void Initialize()
        {
            m_canvasGroup = GetComponent<CanvasGroup>();
            m_beginDragTrigger = GetComponent<ObservableBeginDragTrigger>();
            m_dragTrigger = GetComponent<ObservableDragTrigger>();
            m_endDragTrigger = GetComponent<ObservableEndDragTrigger>();
            m_pointerClickTrigger = GetComponent<ObservablePointerClickTrigger>();
        }
        
        public void Setup(int slotIndex)
        {
            m_slotIndex = slotIndex;
        }
        
        public void Draw()
        {
            // 모델 구독
            ArtyRosterState.Battery
                .ObserveEveryValueChanged(party => party[m_slotIndex])
                .Subscribe(OnSlotMemberChange)
                .AddTo(gameObject);

            BatteryPage.selectedSlotIndexRx
                .Subscribe(OnSelectedSlotIndexChange)
                .AddTo(gameObject);
            
            // 이벤트 구독
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
            
            // 뷰 초기화
            UpdateView();
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public void Open()
        {
            
        }

        // 모델 구독 콜백
        void OnSlotMemberChange(ArtyModel arty)
        {
            this.arty = arty;

            if (m_characterSubscription != null)
                m_characterSubscription.Dispose();

            if (this.arty != null)
            {
                m_characterSubscription = arty.levelRx
                    .Subscribe(UpdateLevelText);
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

        // 이벤트 구독 콜백
        public ArtyModel GetCharacterModel()
        {
            return arty;
        }

        void OnBeginDrag(PointerEventData eventData)
        {
            m_dragItem.gameObject.SetActive(true);
            //m_dragItem.Initialize(m_character);
            m_dragItem.transform.position = transform.position;

            m_canvasGroup.HideWithAlpha(0.25f);

            BatteryPage.isDraggingSlotItemRx.Value = true;
        }

        void OnDrag(PointerEventData eventData)
        {
            m_dragItem.GetComponent<RectTransform>().position = eventData.position;
        }

        void OnEndDrag(PointerEventData eventData)
        {
            BatteryPage.isDraggingSlotItemRx.Value = false;

            Destroy(m_dragItem.gameObject);
            m_canvasGroup.Show();
        }

        void OnPointerClick(PointerEventData eventData)
        {
            BatteryPage.selectedSlotIndexRx.Value = m_slotIndex;
        }

        // 뷰 초기화 함수
        public void UpdateView()
        {
            if (arty == null)
            {
                m_canvasGroup.Hide();
                return;
            }

            m_canvasGroup.Show();
            m_portraitImage.sprite = arty.Sprite;
            m_levelText.text = arty.levelRx.ToString();
            m_nameText.text = arty.displayName;
        }

        void UpdateLevelText(int value)
        {
            m_levelText.text = value.ToString();
        }
    }
}