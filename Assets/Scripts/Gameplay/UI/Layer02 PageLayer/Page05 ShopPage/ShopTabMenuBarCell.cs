using System;
using TMPro;
using UnityEngine;

namespace Mathlife.ProjectL.Gameplay.UI
{
    public class ShopTabMenuBarCell : SimpleScrollRectCell<ShopTabMenuItemData, SimpleScrollRectContext>
    {
        [SerializeField]
        public TextMeshProUGUI tabMenuNameText;
        
        public override void UpdateContent(ShopTabMenuItemData itemData)
        {
            tabMenuNameText.text = itemData.displayName;
            tabMenuNameText.characterSpacing = itemData.GetCharacterSpacing();

            if (Index == Context.selectedIndex)
            {
                tabMenuNameText.color = Color.cyan;
            }
            else
            {
                tabMenuNameText.color = Color.white;
            }
        }
    }
}
