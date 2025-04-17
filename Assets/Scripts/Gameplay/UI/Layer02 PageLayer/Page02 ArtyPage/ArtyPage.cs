using System;
using System.Collections.Generic;
using UniRx;
using Mathlife.ProjectL.Gameplay.UI.ArtyPageView;
using Mathlife.ProjectL.Utils;
using UnityEngine;

namespace Mathlife.ProjectL.Gameplay.UI
{
    public class ArtyPage : Page
    {
        // Override
        public override string PageName => "화포 관리";

        // Alias
        private static ArtyRosterState ArtyRosterState => GameState.Inst.artyRosterState;
        
        // Field
        public readonly ReactiveProperty<int> selectedArtyIndexRx = new(0);

        //public ArtyModel SelectedArty => ArtyRosterState[selectedArtyIndexRx.Value];
        public ArtyModel SelectedArty { get; private set; }
        
        private CompositeDisposable disposables = new();
        
        // View
        [SerializeField]
        private ArtyPageScrollView scrollView;

        [SerializeField]
        private RectTransform scrollViewMaskRectTransform;

        public Rect ScrollViewMaskRect => scrollViewMaskRectTransform.GetGlobalRect();

        [SerializeField]
        private ArtyPageSelectedArtyView artyPageSelectedArtyView;

        protected override void OnOpen()
        {
            // Overlay
            NavigateBackOverlay navBackOverlay = Find<NavigateBackOverlay>();
            navBackOverlay.Activate();

            disposables.Clear();

            selectedArtyIndexRx.Subscribe(idx => SelectedArty = ArtyRosterState[idx])
                .AddTo(disposables);
            
            UpdateScrollView();

            ArtyRosterState.SubscribeAllArtyLevelChange(UpdateScrollView)
                .AddTo(disposables);

            artyPageSelectedArtyView.Draw();
        }
        
        protected override void OnClose()
        {
            // 뷰 정리
            artyPageSelectedArtyView.Clear();
            disposables.Clear();
        }

        private void OnDestroy()
        {
            disposables.Dispose();
        }
        
        public void UpdateSelectedArtyView()
        {
            artyPageSelectedArtyView.SubscribeArty();
        }

        private void UpdateScrollView()
        {
            // 데이터 fetching
            List<ArtyModel> sortedList = ArtyRosterState.GetSortedList();

            if (selectedArtyIndexRx.Value < 0 || selectedArtyIndexRx.Value >= sortedList.Count)
                selectedArtyIndexRx.Value = 0;

            if (SelectedArty != sortedList[selectedArtyIndexRx.Value])
            {
                selectedArtyIndexRx.Value = sortedList.IndexOf(SelectedArty);
                
                if (selectedArtyIndexRx.Value < 0 || selectedArtyIndexRx.Value >= sortedList.Count)
                    selectedArtyIndexRx.Value = 0;
            }
            
            // 뷰 초기화
            scrollView.Setup(sortedList);
            scrollView.SelectCell(selectedArtyIndexRx.Value, false);
        }
    }
}