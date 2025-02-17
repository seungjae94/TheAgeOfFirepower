using System;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Mathlife.ProjectL.Gameplay
{
    internal class ShopItemCardView : MonoBehaviour
    {
        [SerializeField] Image m_iconImage;
        [SerializeField] TMP_Text m_itemNameText;
        [SerializeField] TMP_Text m_itemDescriptionText;
        [SerializeField] TMP_Text m_itemPriceText;
        [SerializeField] Button m_buyButton;

        public void Render(EquipmentSO equipmentSO, Action<EquipmentSO> onClickAction)
        {
            m_iconImage.sprite = equipmentSO.icon;
            m_itemNameText.text = equipmentSO.displayName;
            m_itemDescriptionText.text = equipmentSO.description;
            m_itemPriceText.text = equipmentSO.shopPrice.ToString();

            m_buyButton.OnClickAsObservable()
                .Subscribe(_ => onClickAction(equipmentSO))
                .AddTo(gameObject);
        }
    }
}
