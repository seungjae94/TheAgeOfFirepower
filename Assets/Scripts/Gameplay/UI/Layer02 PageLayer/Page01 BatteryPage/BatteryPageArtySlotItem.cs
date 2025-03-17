using Mathlife.ProjectL.Utils;
using System;
using TMPro;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Mathlife.ProjectL.Gameplay.UI
{
    public class BatteryPageArtySlotItem : AbstractView
    {
        // Alias
        BatteryPage BatteryPage => Presenter.Find<BatteryPage>();
        ArtyRosterState ArtyRosterState => GameState.Inst.artyRosterState;
        BatteryPageArtySlotItemDragView ItemDragView => BatteryPage.ArtySlotItemViewDragView;

        // Component
        private CanvasGroup itemCanvasGroup;
        private ObservableBeginDragTrigger beginDragTrigger;
        private ObservableDragTrigger dragTrigger;
        private ObservableEndDragTrigger endDragTrigger;
        private ObservablePointerClickTrigger pointerClickTrigger;

        [SerializeField]
        Image portraitImage;

        [SerializeField]
        TMP_Text nameText;

        [SerializeField]
        TMP_Text levelText;

        // Field
        private int slotIndex;
        public ArtyModel Arty { get; private set; }

        CompositeDisposable disposables = new CompositeDisposable();
        IDisposable artySub = null;

        // Setup
        public void Setup(int pSlotIndex)
        {
            slotIndex = pSlotIndex;
        }

        // 이벤트 함수
        public override void Initialize()
        {
            base.Initialize();
            
            itemCanvasGroup = GetComponent<CanvasGroup>();
            beginDragTrigger = GetComponent<ObservableBeginDragTrigger>();
            dragTrigger = GetComponent<ObservableDragTrigger>();
            endDragTrigger = GetComponent<ObservableEndDragTrigger>();
            pointerClickTrigger = GetComponent<ObservablePointerClickTrigger>();
        }

        public override void Draw()
        {
            // 모델 구독
            ArtyRosterState.Battery
                .ObserveEveryValueChanged(party => party[slotIndex])
                .Subscribe(OnSlotMemberChange)
                .AddTo(disposables);

            // 이벤트 구독
            beginDragTrigger
                .OnBeginDragAsObservable()
                .Subscribe(OnBeginDrag)
                .AddTo(disposables);

            dragTrigger
                .OnDragAsObservable()
                .Subscribe(OnDrag)
                .AddTo(disposables);

            endDragTrigger
                .OnEndDragAsObservable()
                .Subscribe(OnEndDrag)
                .AddTo(disposables);

            pointerClickTrigger
                .OnPointerClickAsObservable()
                .Subscribe(OnPointerClick)
                .AddTo(disposables);

            // 뷰 초기화
            UpdateView();
        }

        public override void Clear()
        {
            disposables.Clear();
            artySub?.Dispose();
        }

        // 모델 구독 콜백
        void OnSlotMemberChange(ArtyModel arty)
        {
            artySub?.Dispose();

            Arty = arty;
            if (Arty != null)
            {
                artySub = arty.levelRx
                    .Subscribe(UpdateLevelText);
            }

            UpdateView();
        }

        // 이벤트 구독 콜백
        private void OnBeginDrag(PointerEventData eventData)
        {
            ItemDragView.gameObject.SetActive(true);
            ItemDragView.Setup(Arty);
            ItemDragView.Draw();

            itemCanvasGroup.HideWithAlpha(0.25f);

            //BatteryPage.isDraggingSlotItemRx.Value = true;
        }

        private void OnDrag(PointerEventData eventData)
        {
            RectTransformUtility.ScreenPointToWorldPointInRectangle(
                ItemDragView.transform as RectTransform,
                eventData.position,
                eventData.pressEventCamera,
                out Vector3 worldPoint
            );

            ItemDragView.transform.position = worldPoint;
        }

        private void OnEndDrag(PointerEventData eventData)
        {
            //BatteryPage.isDraggingSlotItemRx.Value = false;

            ItemDragView.gameObject.SetActive(false);
            itemCanvasGroup.Show();
        }

        private void OnPointerClick(PointerEventData eventData)
        {
            BatteryPage.selectedSlotIndexRx.Value = slotIndex;
        }

        // 뷰 초기화 함수
        private void UpdateView()
        {
            if (Arty == null)
            {
                itemCanvasGroup.Hide();
                return;
            }

            itemCanvasGroup.Show();
            portraitImage.sprite = Arty.Sprite;
            levelText.text = Arty.levelRx.ToString();
            nameText.text = Arty.displayName;
        }

        private void UpdateLevelText(int value)
        {
            levelText.text = value.ToString();
        }
    }
}