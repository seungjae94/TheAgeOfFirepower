using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine;
using VContainer;

namespace Mathlife.ProjectL.Gameplay
{
    public class CharacterPage : Page
    {
        public override EPageId pageId => EPageId.CharacterPage;

        [Inject] MainSceneManager m_mainSceneManager;
        [Inject] PartyPage m_partyPage; 


        [SerializeField] SimpleActionButton m_navigateBackBar;
        [SerializeField] Image m_background;     // TODO: 월드 맵에 따라 배경 이미지 변경

        [SerializeField] CharacterBasicInfoPresenter m_basicInfoPresenter;
        [SerializeField] CharacterStatPresenter m_statPresenter;
        [SerializeField] List<CharacterEquipmentSlotPresenter> m_artifactSlotPresenters;

        [field: SerializeField] public EquipmentChangeModal equipmentChangeModal { get; private set; }

        public State<CharacterModel> character { get; private set; } = new();

        protected override void InitializeChildren()
        {
            m_navigateBackBar.Initialize(OnClickBackButton);
            m_basicInfoPresenter.Initialize();
            m_statPresenter.Initialize();

            foreach (var artifactSlot in m_artifactSlotPresenters)
            {
                artifactSlot.Initialize();
            }

            equipmentChangeModal.Initialize();
        }

        // 유저 상호 작용
        void OnClickBackButton()
        {
            m_partyPage.selectedSlotIndex.SetState(-1);
            m_mainSceneManager.NavigateBack();
        }

        public override void Open()
        {
            character.SetState(m_partyPage.GetSelectedCharacter());
            base.Open();
        }
    }
}