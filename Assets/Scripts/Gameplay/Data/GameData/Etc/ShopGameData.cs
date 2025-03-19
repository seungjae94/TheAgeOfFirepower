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
        public List<MechPartGameData> shopWeapons = new();

        [TabGroup("Equipment", "방어구")]
        [LabelText("방어구")]
        [ListDrawerSettings(ShowFoldout = false, ListElementLabelName = "displayName")]
#if UNITY_EDITOR
        [PreviewField(PreviewGetter = nameof(GetItemPreview), Alignment = ObjectFieldAlignment.Left)]
#endif
        public List<MechPartGameData> shopArmors = new();

        [TabGroup("Equipment", "아티팩트")]
        [LabelText("아티팩트")]
        [ListDrawerSettings(ShowFoldout = false, ListElementLabelName = "displayName")]
#if UNITY_EDITOR
        [PreviewField(PreviewGetter = nameof(GetItemPreview), Alignment = ObjectFieldAlignment.Left)]
#endif
        public List<MechPartGameData> shopArtifacts = new();

#if UNITY_EDITOR
        public Sprite GetItemPreview(MechPartGameData mechPart)
        {
            return mechPart.icon;
        }
#endif

        public List<MechPartGameData> GetItemCatalog(EMechPartType type)
        {
            switch (type)
            {
                case EMechPartType.Barrel:
                    return shopWeapons;
                case EMechPartType.Armor:
                    return shopArmors;
                case EMechPartType.Engine:
                    return shopArtifacts;
                default:
                    return null;
            }
        }
    }
}