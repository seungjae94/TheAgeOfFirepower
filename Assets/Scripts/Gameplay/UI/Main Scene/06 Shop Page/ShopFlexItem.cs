using System;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace Mathlife.ProjectL.Gameplay
{
    class ShopFlexItemFactory : IFlexItemFactory<MechPartGameData, Action<MechPartGameData>, ShopFlexItem>
    {
        [Inject] GameDataLoader gameDataLoader;

        public ShopFlexItem Create(Transform parent, MechPartGameData flexItemData, Action<MechPartGameData> flexItemAction)
        {
            ShopFlexItem item = gameDataLoader.Instantiate<ShopFlexItem>(EPrefabId.ShopFlexItem, parent);
            item.Initialize(flexItemData, flexItemAction);
            return item;
        }
    }

    class ShopFlexItem : Presenter<MechPartGameData, Action<MechPartGameData>>
    {
        [SerializeField] Image m_iconImage;
        [SerializeField] TMP_Text m_itemNameText;
        [SerializeField] TMP_Text m_itemDescriptionText;
        [SerializeField] TMP_Text m_itemPriceText;
        [SerializeField] Button m_buyButton;

        MechPartGameData mechPartGameData;
        Action<MechPartGameData> m_buyAction;

        protected override void Store(MechPartGameData mechPartGameData, Action<MechPartGameData> buyAction)
        {
            this.mechPartGameData = mechPartGameData;
            m_buyAction = buyAction;
        }

        protected override void InitializeView()
        {
            m_iconImage.sprite = mechPartGameData.icon;
            m_itemNameText.text = mechPartGameData.displayName;
            m_itemDescriptionText.text = mechPartGameData.description;
            m_itemPriceText.text = mechPartGameData.shopPrice.ToString();
        }

        protected override void SubscribeDataChange()
        {}

        protected override void SubscribeUserInteractions()
        {
            m_buyButton.OnClickAsObservable()
                .Subscribe(_ => m_buyAction(mechPartGameData))
                .AddTo(gameObject);
        }
    }
}
