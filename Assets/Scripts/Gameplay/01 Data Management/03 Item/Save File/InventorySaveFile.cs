using System;
using System.Collections.Generic;
using UnityEngine;

namespace Mathlife.ProjectL.Gameplay
{
    [Serializable]
    public class InventorySaveFile : SaveFile
    {
        public long gold = 0L;
        public List<EEquipmentId> equipments = new();
    }
}
