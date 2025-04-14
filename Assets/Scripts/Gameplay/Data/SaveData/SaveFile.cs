using System;
using System.Collections.Generic;
using UnityEngine;

namespace Mathlife.ProjectL.Gameplay
{
    public abstract class SaveFile
    {
        public Vector3Int version = Vector3Int.zero;
        
        public static int ListToHashCode<T>(List<T> list)
        {
            HashCode hash = new HashCode();
            foreach (var item in list)
            {
                hash.Add(item.GetHashCode());
            }
            return hash.GetHashCode();
        }
    }
}
