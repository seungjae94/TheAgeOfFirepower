using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

namespace Mathlife.ProjectL.Gameplay
{
    [Serializable]
    public struct ItemStackSaveData : IEquatable<ItemStackSaveData>
    {
        public int id;
        public int amount;

        public bool Equals(ItemStackSaveData other)
        {
            return id == other.id && amount == other.amount;
        }

        public override bool Equals(object obj)
        {
            return obj is ItemStackSaveData other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(id, amount);
        }
    }
    
    [Serializable]
    public class InventorySaveFile : SaveFile
    {
        public long gold = 0L;
        public long diamond = 0L;
        
        public List<int> mechParts = new();
        public List<ItemStackSaveData> materialItems = new();
        public List<ItemStackSaveData> battleItems = new();
        
        public override bool Equals(object obj)
        {
            if (obj is not InventorySaveFile other) return false;
            if (ReferenceEquals(this, other)) return true;
            return gold == other.gold 
                   && diamond == other.diamond
                   && materialItems.SequenceEqual(other.materialItems)
                   && battleItems.SequenceEqual(other.battleItems);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(gold, diamond, mechParts, ListToHashCode(materialItems), ListToHashCode(battleItems));
        }
    }
}
