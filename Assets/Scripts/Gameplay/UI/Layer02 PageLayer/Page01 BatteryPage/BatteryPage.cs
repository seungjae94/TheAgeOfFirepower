using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Mathlife.ProjectL.Utils;
using UniRx;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Mathlife.ProjectL.Gameplay.UI
{
    public class BatteryPage : Page
    {
        public override string PageName => "포대 관리";

        // Alias
        private ArtyRosterState ArtyRosterState => GameState.Inst.artyRosterState;

        // View
        private List<BatteryPageArtySlot> artySlots;
        private BatteryPageSelectedArtyView selectedArtyView;
        
        [field: SerializeField]
        public BatteryPageArtySlotItemDragView ArtySlotItemViewDragView { get; private set; }

        // State - Selected Slot Index
        public readonly ReactiveProperty<int> selectedSlotIndexRx = new(-1);

        private bool IsSelectedSlotIndexInRange()
        {
            return selectedSlotIndexRx.Value >= 0 && selectedSlotIndexRx.Value < artySlots.Count;
        }

        public ArtyModel SelectedArty
        {
            get
            {
                if (IsSelectedSlotIndexInRange() == false)
                    return null;

                return ArtyRosterState.Battery[selectedSlotIndexRx.Value];
            }
        }
        
        // 이벤트 함수
        public override void Initialize()
        {
            base.Initialize();
        
            artySlots = transform.FindAllRecursive<BatteryPageArtySlot>();
            selectedArtyView = transform.FindRecursive<BatteryPageSelectedArtyView>();
        }

        protected override void OnOpen()
        {
            // Overlay
            NavigateBackOverlay navBackOverlay = Find<NavigateBackOverlay>();
            navBackOverlay.Activate();
            // TODO: navBackOverlay 콜백에 OnNavBack 등록

            // 상태 초기화
            selectedSlotIndexRx.Value = -1;
            
            // 뷰 초기화
            selectedArtyView.Draw();
            artySlots.ForEach(slot => slot.Draw());
        }
        
        protected override void OnClose()
        {
            // Overlay
            NavigateBackOverlay navBackOverlay = Find<NavigateBackOverlay>();
            // TODO: navBackOverlay 콜백에서 OnNavBack 제거
            
            // 뷰 정리
            selectedArtyView.Clear();
            artySlots.ForEach(slot => slot.Clear());
        }

        // 이벤트 콜백
        async UniTaskVoid OnNavBack(Unit _)
        {
            if (ArtyRosterState.Battery.Validate() == false)
                await Find<BatteryPageBatteryValidationModal>().OpenWithAnimation();
        }
    }
}