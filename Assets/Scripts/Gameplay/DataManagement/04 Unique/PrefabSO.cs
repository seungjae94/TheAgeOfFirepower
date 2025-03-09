using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
#endif

namespace Mathlife.ProjectL.Gameplay
{
    [CreateAssetMenu(fileName = "PrefabSO", menuName = "SO/Prefab SO")]
    public class PrefabSO : SerializedScriptableObject
    {
        public Dictionary<EPrefabId, GameObject> prefabs = new();
    }
}