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
        public readonly ReactiveProperty<int> viewingArtyIndex = new(0);

        // View
        [SerializeField]
        private ArtyPageScrollView scrollView;

        [SerializeField]
        private RectTransform scrollViewMaskRectTransform;

        public Rect ScrollViewMaskRect => scrollViewMaskRectTransform.GetGlobalRect();

        public override void Open()
        {
            base.Open();

            // Overlay
            NavigateBackOverlay navBackOverlay = Find<NavigateBackOverlay>();
            navBackOverlay.Activate();

            // 데이터 fetching
            List<ArtyModel> sortedList = ArtyRosterState.GetSortedList();

            if (viewingArtyIndex.Value < 0 || viewingArtyIndex.Value >= sortedList.Count)
                viewingArtyIndex.Value = 0;
            
            // 뷰 초기화
            scrollView.Setup(sortedList);
            scrollView.SelectCell(viewingArtyIndex.Value);

            // 

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
    }
}