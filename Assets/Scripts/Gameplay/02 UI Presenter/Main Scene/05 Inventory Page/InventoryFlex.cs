using System;
using System.Collections.Generic;
using UnityEngine;

namespace Mathlife.ProjectL.Gameplay
{
    class InventoryFlexItemData
    {
        public InventoryFlexItemData(EquipmentModel equipment, bool isSelected)
        {
            this.equipment = equipment;
            this.isSelected = isSelected;
        }

        public EquipmentModel equipment;
        public bool isSelected;
    }

    class InventoryFlex 
        : AbstractFlex<InventoryFlexItemData, Action<EquipmentModel>, InventoryFlexItem>
    {
        
    }
}
