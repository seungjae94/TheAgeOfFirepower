using System;
using System.Collections.Generic;
using UnityEngine;

namespace Mathlife.ProjectL.Gameplay
{
    class InventorySlotFlexItemData
    {
        public InventorySlotFlexItemData(EquipmentModel equipment, bool isSelected)
        {
            this.equipment = equipment;
            this.isSelected = isSelected;
        }

        public EquipmentModel equipment;
        public bool isSelected;
    }

    internal class InventorySlotFlex : AbstractFlex<InventorySlotFlexItemData, Action<EquipmentModel>, InventorySlotFlexItem>
    {
    }
}
