using Mathlife.ProjectL.Utils;
using System;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace Mathlife.ProjectL.Gameplay
{
    public class EquipmentSlotPresenter : Presenter
    {
        [SerializeField] EEquipmentType m_slotType;

        [Inject] CharacterRepository m_characterRepository;

        Button m_button;
        CanvasGroup m_iconAreaCanvasGroup;
        Image m_iconImage;

        EquipmentModel m_artifact;
        IDisposable selectedCharacterSubscription;

        void Awake()
        {
            // Views
            m_button = GetComponent<Button>();
            m_iconAreaCanvasGroup = transform.FindRecursiveByName<CanvasGroup>("Icon Area");
            m_iconImage = transform.FindRecursiveByName<Image>("Icon Image");
        }

        void OnDestroy()
        {
            UnsubscribeSelectedCharacter();
        }

        public void Initialize()
        {
            TeamPage teamPage = m_worldSceneManager.GetPage<TeamPage>();

            m_button.OnClickAsObservable()
                .Subscribe(OnClick);

            teamPage.SubscribeSelectedCharacterChangeEvent(OnSelectedCharacterChanged);

            OnSelectedCharacterChanged(teamPage.selectedCharacter);

            m_iconAreaCanvasGroup.Hide();
        }

        void OnSelectedCharacterChanged(CharacterModel selected)
        {
            UnsubscribeSelectedCharacter();

            if (selected == null)
                return;

            selectedCharacterSubscription = selected.SubscribeEquipmentChangeEvent(m_slotType, OnArtifactChanged);

            OnArtifactChanged(selected.GetEquipment(m_slotType));
        }

        void OnArtifactChanged(EquipmentModel artifact)
        {
            m_artifact = artifact;

            if (artifact == null)
            {
                m_iconAreaCanvasGroup.Hide();
                m_iconImage.sprite = null;
            }
            else
            {
                m_iconAreaCanvasGroup.Show();
                m_iconImage.sprite = artifact.icon;
            }
        }

        async void OnClick(Unit _)
        {
            m_worldSceneManager.GetPage<CharacterPage>().artifactChangeModal.slotType = m_slotType;
            await m_worldSceneManager.GetPage<CharacterPage>().artifactChangeModal.Show();
        }

        void UnsubscribeSelectedCharacter()
        {
            if (selectedCharacterSubscription != null)
                selectedCharacterSubscription.Dispose();
        }
    }
}
