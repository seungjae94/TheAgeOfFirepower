using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

namespace Mathlife.ProjectL.Gameplay.UI
{
    public class ArtyPage : Page
    {
        [SerializeField] Button m_navBackButton;
        [SerializeField] Image m_background; // TODO: ���� �ʿ� ���� ��� �̹��� ����

        //[SerializeField] CharacterBasicInfoPresenter m_basicInfoPresenter;
        //[SerializeField] CharacterStatPresenter m_statPresenter;
        //[SerializeField] List<CharacterEquipmentSlotPresenter> m_artifactSlotPresenters;

        [field: SerializeField] public EquipmentChangeModal equipmentChangeModal { get; private set; }

        public readonly ReactiveProperty<ArtyModel> characterRx = new();

        public override void Open()
        {
            // Page prevPage = lobbySceneGameMode.GetPreviousPage();
            //
            // if (prevPage is PartyPage)
            //     characterRx.Value = m_partyPage.GetSelectedCharacter();
            // else if (prevPage is CharacterPage)
            //     characterRx.Value = m_characterPage.selectedCharacterRx.Value;

            //m_basicInfoPresenter.Initialize();
            //m_statPresenter.Initialize();
            //
            //foreach (var artifactSlot in m_artifactSlotPresenters)
            //{
            //    artifactSlot.Initialize();
            //}
            //
            //equipmentChangeModal.Initialize();
            //
            //m_navBackButton.OnClickAsObservable()
            //    .Subscribe(_ => OnClickBackButton())
            //    .AddTo(gameObject);
            //
            //base.Open();
        }


        void OnClickBackButton()
        {
            // Page prevPage = lobbySceneGameMode.GetPreviousPage();
            //
            // if (prevPage is PartyPage)
            //     m_partyPage.selectedSlotIndexRx.Value = -1;
            // else if (prevPage is CharacterPage)
            //     m_characterPage.selectedCharacterRx.Value = null;
            //
            // lobbySceneGameMode.NavigateBack();
        }
    }
}