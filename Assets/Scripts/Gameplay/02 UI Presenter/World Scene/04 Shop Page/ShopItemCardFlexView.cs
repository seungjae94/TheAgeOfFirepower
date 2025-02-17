using System;
using System.Collections.Generic;
using UnityEngine;

namespace Mathlife.ProjectL.Gameplay
{
    internal class ShopItemCardFlexView : MonoBehaviour
    {
        [SerializeField] Transform m_contents;

        List<ShopItemCardView> m_itemCards = new();

        public void Render(
            List<EquipmentSO> equipmentSOs, 
            Func<Transform, EquipmentSO, ShopItemCardView> itemCardViewFactoryMethod)
        {
            foreach (ShopItemCardView card in m_itemCards)
            {
                Destroy(card.gameObject);
            }
            m_itemCards.Clear();

            foreach (EquipmentSO equipmentSO in equipmentSOs)
            {
                ShopItemCardView card = itemCardViewFactoryMethod(m_contents, equipmentSO);
                m_itemCards.Add(card);
            }
        }
    }
}
