using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Mathlife.ProjectL.Gameplay
{
    [Serializable]
    public class InventorySaveFile : SaveFile
    {
        public long gold = 0L;
        
        public List<int> mechParts = new();
        public List<int> consumables = new();
        public List<int> combatKits = new();
    }
}
