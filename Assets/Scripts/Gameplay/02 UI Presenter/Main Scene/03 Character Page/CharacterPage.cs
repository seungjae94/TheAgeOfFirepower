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

        [SerializeField] SimpleActionButton m_navigateBackBar;
        [SerializeField] Image m_background;     // TODO: ���� �ʿ� ���� ��� �̹��� ����

        [SerializeField] CharacterBasicInfoPresenter m_basicInfoPresenter;
        [SerializeField] CharacterStatPresenter m_statPresenter;
        [SerializeField] List<CharacterEquipmentSlotPresenter> m_artifactSlotPresenters;

        [field: SerializeField] public EquipmentChangeModal equipmentChangeModal { get; private set; }

        protected override void SubscribeDataChange()
        {
        }

        protected override void SubscribeUserInteractions()
        {
        }

        protected override void InitializeView()
        {
        }

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

        // ���� ��ȣ �ۿ�
        void OnClickBackButton()
        {
            m_mainSceneManager.GetPage<PartyPage>().selectedCharacter = null;
            m_mainSceneManager.NavigateBack();
        }
    }
}