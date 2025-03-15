// using Mathlife.ProjectL.Utils;
// using System;
// using UniRx;
// using UnityEngine;
// using UnityEngine.UI;
//
// namespace Mathlife.ProjectL.Gameplay.UI
// {
//     public class CharacterEquipmentSlotPresenter : Presenter
//     {
//         CharacterDetailPage m_characterDetailPage;
//
//         [SerializeField] EEquipmentType m_slotType;
//         [SerializeField] Button m_button;
//         [SerializeField] CanvasGroup m_iconAreaCanvasGroup;
//         [SerializeField] Image m_iconImage;
//
//         IDisposable m_selectedCharacterSub;
//
//         void OnDestroy()
//         {
//             UnsubscribeCharacter();
//         }
//
//         protected override void SubscribeDataChange()
//         {
//             m_characterDetailPage.characterRx
//                 .Subscribe(OnCharacterChange)
//                 .AddTo(gameObject);
//         }
//
//         protected override void SubscribeUserInteractions()
//         {
//             m_button.OnClickAsObservable()
//                 .Subscribe(OnClick)
//                 .AddTo(gameObject);
//         }
//
//         protected override void InitializeView()
//         {
//             m_iconAreaCanvasGroup.Hide();
//         }
//
//         void OnCharacterChange(CharacterModel character)
//         {
//             UnsubscribeCharacter();
//
//             if (character == null)
//                 return;
//
//             m_selectedCharacterSub = character.SubscribeEquipmentChangeEvent(m_slotType, OnEquipmentChange);
//
//             OnEquipmentChange(character.GetEquipment(m_slotType));
//         }
//
//         void OnEquipmentChange(EquipmentModel artifact)
//         {
//             if (artifact == null)
//             {
//                 m_iconAreaCanvasGroup.Hide();
//                 m_iconImage.sprite = null;
//             }
//             else
//             {
//                 m_iconAreaCanvasGroup.Show();
//                 m_iconImage.sprite = artifact.icon;
//             }
//         }
//
//         async void OnClick(Unit _)
//         {
//             await m_characterDetailPage.equipmentChangeModal.Show(m_slotType);
//         }
//
//         void UnsubscribeCharacter()
//         {
//             if (m_selectedCharacterSub != null)
//                 m_selectedCharacterSub.Dispose();
//         }
//     }
// }
