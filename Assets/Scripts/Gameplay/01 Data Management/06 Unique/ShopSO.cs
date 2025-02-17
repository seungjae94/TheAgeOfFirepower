using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
#endif

namespace Mathlife.ProjectL.Gameplay
{
    [CreateAssetMenu(fileName = "ShopSO", menuName = "SO/Shop SO")]
    public class ShopSO : SerializedScriptableObject
    {
        [TabGroup("Equipment", "무기")]
        [LabelText("무기")]
        [ListDrawerSettings(ShowFoldout = false, ListElementLabelName = "displayName")]
#if UNITY_EDITOR
        [PreviewField(PreviewGetter = nameof(GetItemPreview), Alignment = ObjectFieldAlignment.Left)]
#endif
        public List<EquipmentSO> shopWeapons = new();

        [TabGroup("Equipment", "방어구")]
        [LabelText("방어구")]
        [ListDrawerSettings(ShowFoldout = false, ListElementLabelName = "displayName")]
#if UNITY_EDITOR
        [PreviewField(PreviewGetter = nameof(GetItemPreview), Alignment = ObjectFieldAlignment.Left)]
#endif
        public List<EquipmentSO> shopArmors = new();

        [TabGroup("Equipment", "아티팩트")]
        [LabelText("아티팩트")]
        [ListDrawerSettings(ShowFoldout = false, ListElementLabelName = "displayName")]
#if UNITY_EDITOR
        [PreviewField(PreviewGetter = nameof(GetItemPreview), Alignment = ObjectFieldAlignment.Left)]
#endif
        public List<EquipmentSO> shopArtifacts = new();

#if UNITY_EDITOR
        public Sprite GetItemPreview(EquipmentSO equipment)
        {
            return equipment.icon;
        }
#endif
    }
}