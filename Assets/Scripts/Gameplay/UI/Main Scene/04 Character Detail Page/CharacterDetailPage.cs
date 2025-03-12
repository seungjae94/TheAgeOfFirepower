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

        [Inject] LobbySceneGameMode lobbySceneGameMode;
        [Inject] PartyPage m_partyPage;
        [Inject] CharacterPage m_characterPage;

        [SerializeField] Button m_navBackButton;
        [SerializeField] Image m_background; // TODO: ���� �ʿ� ���� ��� �̹��� ����

        [SerializeField] CharacterBasicInfoPresenter m_basicInfoPresenter;
        [SerializeField] CharacterStatPresenter m_statPresenter;
        [SerializeField] List<CharacterEquipmentSlotPresenter> m_artifactSlotPresenters;

        [field: SerializeField] public EquipmentChangeModal equipmentChangeModal { get; private set; }

        public readonly ReactiveProperty<CharacterModel> characterRx = new();

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

        // ���� ��ȣ �ۿ�
        void OnClickBackButton()
        {
            Page prevPage = lobbySceneGameMode.GetPreviousPage();

            if (prevPage is PartyPage)
                m_partyPage.selectedSlotIndexRx.Value = -1;
            else if (prevPage is CharacterPage)
                m_characterPage.selectedCharacterRx.Value = null;

            lobbySceneGameMode.NavigateBack();
        }

        public override void Open()
        {
            Page prevPage = lobbySceneGameMode.GetPreviousPage();

            if (prevPage is PartyPage)
                characterRx.Value = m_partyPage.GetSelectedCharacter();
            else if (prevPage is CharacterPage)
                characterRx.Value = m_characterPage.selectedCharacterRx.Value;

            base.Open();
        }
    }
}