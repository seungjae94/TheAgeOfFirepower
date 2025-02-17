using Cysharp.Threading.Tasks;
using UnityEngine.UI;
using Mathlife.ProjectL.Utils;
using UniRx;
using System.Collections.Generic;

namespace Mathlife.ProjectL.Gameplay
{
    public class CharacterPage : Page
    {
        // View
        Button m_backButton;
        Image m_background;     // TODO: 월드 맵에 따라 배경 이미지 변경

        CharacterBasicInfoPresenter m_basicInfoPresenter;
        CharacterStatPresenter m_statPresenter;
        List<CharacterEquipmentSlotPresenter> m_artifactSlotPresenters;
        
        public EquipmentChangeModal equipmentChangeModal { get; private set; }

        // Fields
        public override EPageId pageId => EPageId.CharacterPage;

        protected override void Awake()
        {
            base.Awake();

            m_backButton = transform.FindRecursiveByName<Button>("Back Button");
            m_background = transform.FindRecursiveByName<Image>("Background");

            m_basicInfoPresenter = GetComponent<CharacterBasicInfoPresenter>();
            m_statPresenter = transform.FindRecursive<CharacterStatPresenter>();
            m_artifactSlotPresenters = transform.FindAllRecursive<CharacterEquipmentSlotPresenter>();
            equipmentChangeModal = transform.FindRecursive<EquipmentChangeModal>();
        }

        public override void Initialize()
        {
            // Subscribe Views
            m_backButton
                .OnClickAsObservable()
                .Subscribe(_ => OnClickBackButton())
                .AddTo(gameObject);

            InitializeChildren();

            Close();
        }

        void OnClickBackButton()
        {
            m_worldSceneManager.GetPage<TeamPage>().selectedCharacter = null;
            m_worldSceneManager.NavigateBack();
        }

        protected override void InitializeChildren()
        {
            m_basicInfoPresenter.Initialize();
            m_statPresenter.Initialize();

            foreach (var artifactSlot in m_artifactSlotPresenters)
            {
                artifactSlot.Initialize();
            }

            equipmentChangeModal.Initialize();
        }
    }
}