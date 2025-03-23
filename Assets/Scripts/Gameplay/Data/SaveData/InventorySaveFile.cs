using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Mathlife.ProjectL.Gameplay
{
    [Serializable]
    public struct ItemStackSaveData
    {
        public int id;
        public int amount;
    }
    
    [Serializable]
    public class InventorySaveFile : SaveFile
    {
        public long gold = 0L;
        public long diamond = 0L;
        
        public List<int> mechParts = new();
        public List<ItemStackSaveData> materialItems = new();
        public List<ItemStackSaveData> battleItems = new();
    }
}
