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
        
        //[SerializeField] CharacterBasicInfoPresenter m_basicInfoPresenter;
        //[SerializeField] CharacterStatPresenter m_statPresenter;
        //[SerializeField] List<CharacterEquipmentSlotPresenter> m_artifactSlotPresenters;

        //[field: SerializeField] public EquipmentChangeModal equipmentChangeModal { get; private set; }

        // Field
        public readonly ReactiveProperty<int> selectedArtyIndexRx = new(0);

        public ArtyModel SelectedArty => ArtyRosterState[selectedArtyIndexRx.Value];
        
        // View
        [SerializeField]
        private ArtyPageScrollView scrollView;

        [SerializeField]
        private RectTransform scrollViewMaskRectTransform;

        public Rect ScrollViewMaskRect => scrollViewMaskRectTransform.GetGlobalRect();

        [SerializeField]
        private ArtyPageSelectedArtyView artyPageSelectedArtyView;
        
        public override void Open()
        {
            base.Open();

            // Overlay
            NavigateBackOverlay navBackOverlay = Find<NavigateBackOverlay>();
            navBackOverlay.Activate();

            // 데이터 fetching
            List<ArtyModel> sortedList = ArtyRosterState.GetSortedList();

            if (selectedArtyIndexRx.Value < 0 || selectedArtyIndexRx.Value >= sortedList.Count)
                selectedArtyIndexRx.Value = 0;
            
            // 뷰 초기화
            scrollView.Setup(sortedList);
            scrollView.SelectCell(selectedArtyIndexRx.Value);

            artyPageSelectedArtyView.Draw();

            // 화포
            // TODO: 화포 기본 정보 (이름, 레벨, 경험치)
            // artyBasicInfoView.Draw();
            // TODO: 화포 스탯
            // artyBasicInfoView.Draw();
            // TODO: 화포 초상화
            // artyPortraitImage.sprite = ...;
            // TODO: 화포 부품
            // artyMechPartSlots.Foreach(...);
        }

        public override void Close()
        {
            base.Close();
            
            NavigateBackOverlay navBackOverlay = Find<NavigateBackOverlay>();
            navBackOverlay.Deactivate();
            
            // 뷰 정리
            artyPageSelectedArtyView.Clear();
        }
    }
}