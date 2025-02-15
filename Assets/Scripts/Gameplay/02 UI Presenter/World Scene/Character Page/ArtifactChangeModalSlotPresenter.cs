using Mathlife.ProjectL.Utils;
using System;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Mathlife.ProjectL.Gameplay
{
    public class ArtifactChangeModalSlotPresenter : Presenter
    {
        Button m_button;
        [SerializeField] Image m_iconImage;
        [SerializeField] CanvasGroup m_equippedMark;
        [SerializeField] CanvasGroup m_selectedMark;

        EquipmentModel m_artifact;

        void Awake()
        {
            m_button = GetComponent<Button>();
        }

        public void Initialize(EquipmentModel artifact)
        {
            m_artifact = artifact;

            bool selected = m_worldSceneManager.GetPage<CharacterPage>().artifactChangeModal.IsSelected(artifact);

            m_button.OnClickAsObservable()
                .Subscribe(OnClick)
                .AddTo(gameObject);

            m_iconImage.sprite = artifact.icon;

            if (artifact.owner != null)
                m_equippedMark.Show();
            else
                m_equippedMark.Hide();

            if (selected)
                m_selectedMark.Show();
            else
                m_selectedMark.Hide();
        }

        void OnClick(Unit _)
        {
            m_worldSceneManager.GetPage<CharacterPage>().artifactChangeModal.selectedEquipment = m_artifact;
        }
    }
}
