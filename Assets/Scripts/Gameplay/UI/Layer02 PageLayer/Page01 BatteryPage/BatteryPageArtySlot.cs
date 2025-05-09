﻿using Cysharp.Threading.Tasks;
using Mathlife.ProjectL.Gameplay.UI.BatteryPagePopup;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.EventSystems;
using Mathlife.ProjectL.Utils;

namespace Mathlife.ProjectL.Gameplay.UI
{
    public class BatteryPageArtySlot : AbstractView
    {
        // Alias
        private BatteryPage BatteryPage => Presenter.Find<BatteryPage>();
        private ArtyRosterState ArtyRosterState => GameState.Inst.artyRosterState;

        // Trigger
        private ObservableDropTrigger dropTrigger;
        private ObservablePointerClickTrigger clickTrigger;

        // View
        [SerializeField]
        private CanvasGroup addMemberGuideCanvasGroup;
        
        [SerializeField]
        private GameObject selectionOverlay;
        
        [SerializeField]
        private BatteryPageArtySlotItem slotItem;
        
        // Field
        private int slotIndex = 0;
        private readonly CompositeDisposable disposables = new();

        // 이벤트 함수
        public override void OnSceneInitialize()
        {
            base.OnSceneInitialize();
            
            slotIndex = transform.GetSiblingIndex();
            dropTrigger = GetComponent<ObservableDropTrigger>();
            clickTrigger = GetComponent<ObservablePointerClickTrigger>();
            
            slotItem.Setup(slotIndex);
        }

        public override void Draw()
        {
            // 모델 구독
            ArtyRosterState.Battery
                .ObserveEveryValueChanged(battery => battery[slotIndex])
                .Subscribe(UpdateAddMemberGuideView)
                .AddTo(disposables);

            BatteryPage.selectedSlotIndexRx
                .Subscribe(OnSelectedSlotIndexChange)
                .AddTo(disposables);
            
            // 이벤트 구독
            dropTrigger
                .OnDropAsObservable()
                .Subscribe(OnDrop)
                .AddTo(disposables);

            clickTrigger
                .OnPointerClickAsObservable()
                .Subscribe(OnClickSlot)
                .AddTo(disposables);

            // 뷰 초기화
            ArtyModel arty = ArtyRosterState.Battery[slotIndex];
            UpdateAddMemberGuideView(arty);
            slotItem.Draw();
        }

        public override void Clear()
        {
            disposables.Clear();
            slotItem.Clear();
        }

        // 모델 구독 콜백
        void UpdateAddMemberGuideView(ArtyModel arty)
        {
            if (arty != null)
                addMemberGuideCanvasGroup.Hide();
            else
                addMemberGuideCanvasGroup.Show();
        }
        
        void OnSelectedSlotIndexChange(int selectedSlotIndex)
        {
            // This character is selected.
            if (selectedSlotIndex == slotIndex)
            {
                selectionOverlay.SetActive(true);
            }
            // Another character is selected.
            else
            {
                selectionOverlay.SetActive(false);
            }
        }

        // 이벤트 구독 콜백
        private void OnDrop(PointerEventData eventData)
        {
            var draggedArty = eventData.pointerDrag?
                .GetComponent<BatteryPageArtySlotItem>()?
                .Arty;

            // 유효한 Arty를 드래그한 경우만 처리
            if (null == draggedArty)
                return;

            var currentArty = ArtyRosterState.Battery[slotIndex];

            AudioManager.Inst.PlaySE(ESoundEffectId.Ok);
            
            // 같은 슬롯으로 드래그한 경우 무시
            if (currentArty == draggedArty)
                return;

            // 슬롯 변경
            if (ArtyRosterState.Battery.Contains(draggedArty))
            {
                int draggedSlotIndex = ArtyRosterState.Battery.IndexOf(draggedArty);
                ArtyRosterState.Battery.Swap(slotIndex, draggedSlotIndex);
            }

            // 변경사항 저장
            GameState.Inst.Save();

            // 선택된 슬롯 초기화
            BatteryPage.selectedSlotIndexRx.Value = -1;
        }

        private void OnClickSlot(PointerEventData ev)
        {
            BatteryPage.selectedSlotIndexRx.Value = slotIndex;

            BatteryPageMemberChangePopup popup = Presenter.Find<BatteryPageMemberChangePopup>();
            ArtyModel arty = ArtyRosterState.Battery[slotIndex];
            if (arty == null && popup.isActiveAndEnabled == false)
            {
                popup.OpenWithAnimation().Forget();
            }
        }
    }
}