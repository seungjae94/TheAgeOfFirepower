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
        [Inject] PartyPage m_partyPage;
        [Inject] CharacterPage m_characterPage;

        [SerializeField] EEquipmentType m_slotType;
        [SerializeField] Button m_button;
        [SerializeField] CanvasGroup m_iconAreaCanvasGroup;
        [SerializeField] Image m_iconImage;

        EquipmentModel m_equipment;
        IDisposable m_selectedCharacterSub;

        void OnDestroy()
        {
            UnsubscribeSelectedCharacter();
        }

        protected override void SubscribeDataChange()
        {
            m_partyPage.selectedCharacter.SubscribeChangeEvent(OnSelectedCharacterChanged);
        }

        protected override void SubscribeUserInteractions()
        {
            m_button.OnClickAsObservable()
                .Subscribe(OnClick)
                .AddTo(gameObject);
        }

        protected override void InitializeView()
        {
            OnSelectedCharacterChanged(m_partyPage.selectedCharacter.GetState());

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
            await m_characterPage.equipmentChangeModal.Show(m_slotType);
        }

        void UnsubscribeSelectedCharacter()
        {
            if (m_selectedCharacterSub != null)
                m_selectedCharacterSub.Dispose();
        }
    }
}
