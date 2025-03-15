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
        // Alias
        private ArtyRosterState ArtyRosterState => GameState.Inst.artyRosterState;
        
        // View
        //Transform m_partySlotsParent;
        private List<BatteryPageArtySlot> artySlots;
        private BatteryPageSelectedArtyView selectedArtyView;
        
        // Field - Selected Slot Index
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
        
        // Field - Is Dragging Slot Item
        public readonly ReactiveProperty<bool> isDraggingSlotItemRx = new(false);

        // 이벤트 함수
        private void Awake()
        {
            artySlots = transform.FindAllRecursive<BatteryPageArtySlot>();
            selectedArtyView = transform.FindRecursive<BatteryPageSelectedArtyView>();
        }

        public override void Open()
        {
            base.Open();
            
            // Overlay
            NavigateBackOverlay navBackOverlay = Find<NavigateBackOverlay>();
            navBackOverlay.Open();
            // TODO: navBackOverlay 콜백에 OnNavBack 등록
            
            // 상태 초기화
            selectedSlotIndexRx.Value = -1;
        }

        public override void Close()
        {
            base.Close();
            
            // TODO: navBackOverlay 콜백에서 OnNavBack 제거
        }

        // 이벤트 콜백
        async UniTaskVoid OnNavBack(Unit _)
        {
            // if (CharacterRosterState.party.Validate() == false)
            //     await Find<PartyValidationModal>().Open(0.5f);
        }
    }
}