using Sirenix.OdinInspector;
using Sirenix.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

#if UNITY_EDITOR
using Sirenix.OdinInspector.Editor;
#endif

namespace Mathlife.ProjectL.Gameplay
{
    [CreateAssetMenu(fileName = "Constant Data Asset", menuName = "Data Asset/Constant Data")]
    public class GlobalDataAsset : SerializedScriptableObject
    {
        [HorizontalGroup("Constants", Title = "Constants")]
        public int maxLevel = 100;

        [HorizontalGroup("Constants")]
        public int teamSize = 4;

        public Dictionary<EPrefabId, GameObject> prefabs = new();
    }
}