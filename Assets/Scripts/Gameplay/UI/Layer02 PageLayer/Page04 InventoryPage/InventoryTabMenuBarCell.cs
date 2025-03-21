using System;
using TMPro;
using UnityEngine;

namespace Mathlife.ProjectL.Gameplay.UI
{
    public class InventoryTabMenuBarCell : TabMenuBarCell<InventoryTabMenuItemData, TabMenuContext>
    {
        [SerializeField]
        public TextMeshProUGUI tabMenuNameText;
        
        public override void UpdateContent(InventoryTabMenuItemData itemData)
        {
            tabMenuNameText.text = itemData.displayName;
            tabMenuNameText.characterSpacing = itemData.GetCharacterSpacing();

            if (Index == Context.selectedIndex)
            {
                tabMenuNameText.color = Color.yellow;
            }
            else
            {
                tabMenuNameText.color = Color.white;
            }
        }
    }
}
