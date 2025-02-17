using Mathlife.ProjectL.Utils;
using System;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace Mathlife.ProjectL.Gameplay
{
    public class CharacterEquipmentSlotPresenter : Presenter
    {
        [Inject] CharacterRepository m_characterRepository;

        [SerializeField] EEquipmentType m_slotType;
        [SerializeField] Button m_button;
        [SerializeField] CanvasGroup m_iconAreaCanvasGroup;
        [SerializeField] Image m_iconImage;

        EquipmentModel m_equipment;
        IDisposable m_selectedCharacterSub;

        void Awake()
        {
            // Views
            //m_button = GetComponent<Button>();
            //m_iconAreaCanvasGroup = transform.FindRecursiveByName<CanvasGroup>("Icon Area");
            //m_iconImage = transform.FindRecursiveByName<Image>("Icon Image");
        }

        void OnDestroy()
        {
            UnsubscribeSelectedCharacter();
        }

        public void Initialize()
        {
            PartyPage teamPage = m_worldSceneManager.GetPage<PartyPage>();

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

            m_selectedCharacterSub = selected.SubscribeEquipmentChangeEvent(m_slotType, OnEquipmentChange);

            OnEquipmentChange(selected.GetEquipment(m_slotType));
        }

        void OnEquipmentChange(EquipmentModel artifact)
        {
            m_equipment = artifact;

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
            await m_worldSceneManager.GetPage<CharacterPage>().equipmentChangeModal.Show(m_slotType);
        }

        void UnsubscribeSelectedCharacter()
        {
            if (m_selectedCharacterSub != null)
                m_selectedCharacterSub.Dispose();
        }
    }
}
