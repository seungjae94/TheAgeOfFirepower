using System;
using TMPro;
using UnityEngine;

namespace Mathlife.ProjectL.Gameplay.UI
{
    public class InventoryTabMenuBarCell : SimpleScrollRectCell<InventoryTabMenuItemData, SimpleScrollRectContext>
    {
        [SerializeField]
        public TextMeshProUGUI tabMenuNameText;
        
        public override void UpdateContent(InventoryTabMenuItemData itemData)
        {
            tabMenuNameText.text = itemData.displayName;
            tabMenuNameText.characterSpacing = itemData.GetCharacterSpacing();

            tabMenuNameText.color = (Index == Context.selectedIndex) ? Color.cyan : Color.white;
        }
    }
}
