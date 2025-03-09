using System;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace Mathlife.ProjectL.Gameplay
{
    class ShopFlexItemFactory : IFlexItemFactory<EquipmentGameData, Action<EquipmentGameData>, ShopFlexItem>
    {
        [Inject] GameDataLoader gameDataLoader;

        public ShopFlexItem Create(Transform parent, EquipmentGameData flexItemData, Action<EquipmentGameData> flexItemAction)
        {
            ShopFlexItem item = gameDataLoader.Instantiate<ShopFlexItem>(EPrefabId.ShopFlexItem, parent);
            item.Initialize(flexItemData, flexItemAction);
            return item;
        }
    }

    class ShopFlexItem : Presenter<EquipmentGameData, Action<EquipmentGameData>>
    {
        [SerializeField] Image m_iconImage;
        [SerializeField] TMP_Text m_itemNameText;
        [SerializeField] TMP_Text m_itemDescriptionText;
        [SerializeField] TMP_Text m_itemPriceText;
        [SerializeField] Button m_buyButton;

        EquipmentGameData equipmentGameData;
        Action<EquipmentGameData> m_buyAction;

        protected override void Store(EquipmentGameData equipmentGameData, Action<EquipmentGameData> buyAction)
        {
            this.equipmentGameData = equipmentGameData;
            m_buyAction = buyAction;
        }

        protected override void InitializeView()
        {
            m_iconImage.sprite = equipmentGameData.icon;
            m_itemNameText.text = equipmentGameData.displayName;
            m_itemDescriptionText.text = equipmentGameData.description;
            m_itemPriceText.text = equipmentGameData.shopPrice.ToString();
        }

        protected override void SubscribeDataChange()
        {}

        protected override void SubscribeUserInteractions()
        {
            m_buyButton.OnClickAsObservable()
                .Subscribe(_ => m_buyAction(equipmentGameData))
                .AddTo(gameObject);
        }
    }
}
