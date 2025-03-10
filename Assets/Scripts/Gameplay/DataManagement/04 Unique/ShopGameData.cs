using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
#endif

namespace Mathlife.ProjectL.Gameplay
{
    public class ShopGameData : SerializedScriptableObject
    {
        [TabGroup("Equipment", "무기")]
        [LabelText("무기")]
        [ListDrawerSettings(ShowFoldout = false, ListElementLabelName = "displayName")]
#if UNITY_EDITOR
        [PreviewField(PreviewGetter = nameof(GetItemPreview), Alignment = ObjectFieldAlignment.Left)]
#endif
        public List<EquipmentGameData> shopWeapons = new();

        [TabGroup("Equipment", "방어구")]
        [LabelText("방어구")]
        [ListDrawerSettings(ShowFoldout = false, ListElementLabelName = "displayName")]
#if UNITY_EDITOR
        [PreviewField(PreviewGetter = nameof(GetItemPreview), Alignment = ObjectFieldAlignment.Left)]
#endif
        public List<EquipmentGameData> shopArmors = new();

        [TabGroup("Equipment", "아티팩트")]
        [LabelText("아티팩트")]
        [ListDrawerSettings(ShowFoldout = false, ListElementLabelName = "displayName")]
#if UNITY_EDITOR
        [PreviewField(PreviewGetter = nameof(GetItemPreview), Alignment = ObjectFieldAlignment.Left)]
#endif
        public List<EquipmentGameData> shopArtifacts = new();

#if UNITY_EDITOR
        public Sprite GetItemPreview(EquipmentGameData equipment)
        {
            return equipment.icon;
        }
#endif

        public List<EquipmentGameData> GetItemCatalog(EEquipmentType type)
        {
            switch (type)
            {
                case EEquipmentType.Weapon:
                    return shopWeapons;
                case EEquipmentType.Armor:
                    return shopArmors;
                case EEquipmentType.Artifact:
                    return shopArtifacts;
                default:
                    return null;
            }
        }
    }
}