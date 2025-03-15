// using Mathlife.ProjectL.Utils;
// using System;
// using UniRx;
// using UnityEngine;
// using UnityEngine.UI;
//
// namespace Mathlife.ProjectL.Gameplay
// {
//     class InventoryFlexItemFactory : IFlexItemFactory<InventoryFlexItemData, Action<EquipmentModel>, InventoryFlexItem>
//     {
//         GameDataLoader gameDataLoader;
//
//         public InventoryFlexItem Create(Transform parent, InventoryFlexItemData flexItemData, Action<EquipmentModel> flexItemAction)
//         {
//             InventoryFlexItem item = gameDataLoader.Instantiate<InventoryFlexItem>(EPrefabId.InventoryFlexItem, parent);
//             item.Initialize(flexItemData, flexItemAction);
//             return item;
//         }
//     }
//
//     class InventoryFlexItem : Presenter<InventoryFlexItemData, Action<EquipmentModel>>
//     {
//         [SerializeField] Button m_button;
//         [SerializeField] Image m_iconImage;
//         [SerializeField] CanvasGroup m_equippedMark;
//         [SerializeField] CanvasGroup m_selectedMark;
//
//         IDisposable m_onClickSub;
//
//         InventoryFlexItemData m_itemData;
//         Action<EquipmentModel> m_onClick;
//
//         protected override void Store(InventoryFlexItemData itemData, Action<EquipmentModel> onClick)
//         {
//             m_itemData = itemData;
//             m_onClick = onClick;
//         }
//
//         protected override void InitializeView()
//         {
//             m_iconImage.sprite = m_itemData.equipment.icon;
//
//             if (m_itemData.equipment.owner != null)
//                 m_equippedMark.Show();
//             else
//                 m_equippedMark.Hide();
//
//             if (m_itemData.isSelected)
//                 m_selectedMark.Show();
//             else
//                 m_selectedMark.Hide();
//         }
//
//         protected override void SubscribeUserInteractions()
//         {
//             if (m_onClickSub != null)
//                 m_onClickSub.Dispose();
//
//             m_onClickSub = m_button.OnClickAsObservable()
//                 .Subscribe(_ => m_onClick(m_itemData.equipment))
//                 .AddTo(gameObject);
//         }
//     }
// }
