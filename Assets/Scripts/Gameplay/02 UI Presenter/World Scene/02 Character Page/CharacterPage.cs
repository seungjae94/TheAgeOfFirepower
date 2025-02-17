using Cysharp.Threading.Tasks;
using UnityEngine.UI;
using Mathlife.ProjectL.Utils;
using UniRx;
using System.Collections.Generic;
using UnityEngine;

namespace Mathlife.ProjectL.Gameplay
{
    public class CharacterPage : Page
    {
        // View
        [SerializeField] NavigateBackBarView m_navigateBackBar;
        [SerializeField] Image m_background;     // TODO: ���� �ʿ� ���� ��� �̹��� ����

        [SerializeField] CharacterBasicInfoPresenter m_basicInfoPresenter;
        [SerializeField] CharacterStatPresenter m_statPresenter;
        [SerializeField] List<CharacterEquipmentSlotPresenter> m_artifactSlotPresenters;

        [field: SerializeField] public EquipmentChangeModal equipmentChangeModal { get; private set; }

        // Fields
        public override EPageId pageId => EPageId.CharacterPage;

        protected override void Awake()
        {
            base.Awake();
        }

        public override void Initialize()
        {
            InitializeChildren();

            Close();
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
            m_worldSceneManager.GetPage<TeamPage>().selectedCharacter = null;
            m_worldSceneManager.NavigateBack();
        }
    }
}