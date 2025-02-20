using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine;
using VContainer;
using UniRx;

namespace Mathlife.ProjectL.Gameplay
{
    public class CharacterDetailPage : Page
    {
        public override EPageId pageId => EPageId.CharacterDetailPage;

        [Inject] MainSceneManager m_mainSceneManager;
        [Inject] PartyPage m_partyPage;
        [Inject] CharacterPage m_characterPage;

        [SerializeField] Button m_navBackButton;
        [SerializeField] Image m_background;     // TODO: 월드 맵에 따라 배경 이미지 변경

        [SerializeField] CharacterBasicInfoPresenter m_basicInfoPresenter;
        [SerializeField] CharacterStatPresenter m_statPresenter;
        [SerializeField] List<CharacterEquipmentSlotPresenter> m_artifactSlotPresenters;

        [field: SerializeField] public EquipmentChangeModal equipmentChangeModal { get; private set; }

        public State<CharacterModel> character { get; private set; } = new();

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

        protected override void SubscribeUserInteractions()
        {
            m_navBackButton.OnClickAsObservable()
                .Subscribe(_ => OnClickBackButton())
                .AddTo(gameObject);
        }

        // 유저 상호 작용
        void OnClickBackButton()
        {
            Page prevPage = m_mainSceneManager.GetPreviousPage();

            if (prevPage is PartyPage)
                m_partyPage.selectedSlotIndex.SetState(-1);
            else if (prevPage is CharacterPage)
                m_characterPage.selectedCharacter.SetState(null);

            m_mainSceneManager.NavigateBack();
        }

        public override void Open()
        {
            Page prevPage = m_mainSceneManager.GetPreviousPage();

            if (prevPage is PartyPage)
                character.SetState(m_partyPage.GetSelectedCharacter());
            else if (prevPage is CharacterPage)
                character.SetState(m_characterPage.selectedCharacter.GetState());

            base.Open();
        }
    }
}