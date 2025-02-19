using System;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace Mathlife.ProjectL.Gameplay
{
    class ShopFlexItemFactory : IFlexItemFactory<EquipmentSO, Action<EquipmentSO>, ShopFlexItem>
    {
        [Inject] GameDataDB m_gameDataDB;

        public ShopFlexItem Create(Transform parent, EquipmentSO flexItemData, Action<EquipmentSO> flexItemAction)
        {
            ShopFlexItem item = m_gameDataDB.Instantiate<ShopFlexItem>(EPrefabId.ShopFlexItem, parent);
            item.Initialize(flexItemData, flexItemAction);
            return item;
        }
    }

    class ShopFlexItem : Presenter<EquipmentSO, Action<EquipmentSO>>
    {
        [SerializeField] Image m_iconImage;
        [SerializeField] TMP_Text m_itemNameText;
        [SerializeField] TMP_Text m_itemDescriptionText;
        [SerializeField] TMP_Text m_itemPriceText;
        [SerializeField] Button m_buyButton;

        EquipmentSO m_equipmentSO;
        Action<EquipmentSO> m_buyAction;

        protected override void Store(EquipmentSO equipmentSO, Action<EquipmentSO> buyAction)
        {
            m_equipmentSO = equipmentSO;
            m_buyAction = buyAction;
        }

        protected override void InitializeView()
        {
            m_iconImage.sprite = m_equipmentSO.icon;
            m_itemNameText.text = m_equipmentSO.displayName;
            m_itemDescriptionText.text = m_equipmentSO.description;
            m_itemPriceText.text = m_equipmentSO.shopPrice.ToString();
        }

        protected override void SubscribeDataChange()
        {}

        protected override void SubscribeUserInteractions()
        {
            m_buyButton.OnClickAsObservable()
                .Subscribe(_ => m_buyAction(m_equipmentSO))
                .AddTo(gameObject);
        }
    }
}
