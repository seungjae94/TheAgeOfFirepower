using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Mathlife.ProjectL.Utils;
using UniRx;
using UnityEngine;

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

        [field: SerializeField] public BatteryPageArtySlotItemDragView ArtySlotItemViewDragView { get; private set; }

        // Field
        private readonly CompositeDisposable disposables = new();
        
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
        public override void OnSceneInitialize()
        {
            base.OnSceneInitialize();

            artySlots = transform.FindAllRecursive<BatteryPageArtySlot>();
            selectedArtyView = transform.FindRecursive<BatteryPageSelectedArtyView>();
        }

        protected override void OnOpen()
        {
            // Overlay
            NavigateBackOverlay navBackOverlay = Find<NavigateBackOverlay>();
            navBackOverlay.onNavigateBack
                .Subscribe(OnNavBack)
                .AddTo(disposables);

            navBackOverlay.Activate();

            // 상태 초기화
            selectedSlotIndexRx.Value = -1;

            // 뷰 초기화
            selectedArtyView.Draw();
            artySlots.ForEach(slot => slot.Draw());
        }

        protected override void OnClose()
        {
            disposables.Clear();

            // 뷰 정리
            selectedArtyView.Clear();
            artySlots.ForEach(slot => slot.Clear());
        }

        private void OnDestroy()
        {
            disposables.Dispose();
        }

        // 이벤트 콜백
        private void OnNavBack(NavigateBackOverlay navigator)
        {
            if (ArtyRosterState.Battery.Validate() == false)
            {
                navigator.StopNavigation();
                Find<BatteryPageValidationPopup>().OpenWithAnimation().Forget();
            }
        }
    }
}