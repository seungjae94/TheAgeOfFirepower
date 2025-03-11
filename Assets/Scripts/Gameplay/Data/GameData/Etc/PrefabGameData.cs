using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
#endif

namespace Mathlife.ProjectL.Gameplay
{
    public class PrefabGameData : SerializedScriptableObject
    {
        public Dictionary<EPrefabId, GameObject> prefabs = new();
    }
}