using Sirenix.OdinInspector;
using System.Collections.Generic;
using Mathlife.ProjectL.Gameplay.Battle;
using UnityEngine;
using UnityEngine.Serialization;

#if UNITY_EDITOR
#endif

namespace Mathlife.ProjectL.Gameplay
{
    public class ShopGameData : SerializedScriptableObject
    {
        [TabGroup("Tab", "화포")]
        [LabelText("화포")]
        [HideReferenceObjectPicker]
        [ListDrawerSettings(ShowFoldout = false, ListElementLabelName = "displayName")]
#if UNITY_EDITOR
        [PreviewField(PreviewGetter = nameof(GetArtyPreview), Alignment = ObjectFieldAlignment.Left)]
#endif
        public List<ArtyGameData> shopArtyList = new();

        [TabGroup("Tab", "포신")]
        [LabelText("포신")]
        [HideReferenceObjectPicker]
        [ListDrawerSettings(ShowFoldout = false, ListElementLabelName = "displayName")]
#if UNITY_EDITOR
        [PreviewField(PreviewGetter = nameof(GetItemPreview), Alignment = ObjectFieldAlignment.Left)]
#endif
        public List<MechPartGameData> shopBarrels = new();

        [TabGroup("Tab", "장갑")]
        [LabelText("장갑")]
        [HideReferenceObjectPicker]
        [ListDrawerSettings(ShowFoldout = false, ListElementLabelName = "displayName")]
#if UNITY_EDITOR
        [PreviewField(PreviewGetter = nameof(GetItemPreview), Alignment = ObjectFieldAlignment.Left)]
#endif
        public List<MechPartGameData> shopArmors = new();

        [TabGroup("Tab", "엔진")]
        [LabelText("엔진")]
        [HideReferenceObjectPicker]
        [ListDrawerSettings(ShowFoldout = false, ListElementLabelName = "displayName")]
#if UNITY_EDITOR
        [PreviewField(PreviewGetter = nameof(GetItemPreview), Alignment = ObjectFieldAlignment.Left)]
#endif
        public List<MechPartGameData> shopEngines = new();

        [TabGroup("Tab", "재료")]
        [LabelText("재료")]
        [HideReferenceObjectPicker]
        [ListDrawerSettings(ShowFoldout = false, ListElementLabelName = "displayName")]
#if UNITY_EDITOR
        [PreviewField(PreviewGetter = nameof(GetItemPreview), Alignment = ObjectFieldAlignment.Left)]
#endif
        public List<MaterialItemGameData> shopMaterialItems = new();

        [TabGroup("Tab", "전투")]
        [LabelText("전투")]
        [HideReferenceObjectPicker]
        [ListDrawerSettings(ShowFoldout = false, ListElementLabelName = "displayName")]
#if UNITY_EDITOR
        [PreviewField(PreviewGetter = nameof(GetItemPreview), Alignment = ObjectFieldAlignment.Left)]
#endif
        public List<BattleItemGameData> shopBattleItems = new();

#if UNITY_EDITOR
        public Sprite GetArtyPreview(ArtyGameData arty)
        {
            return arty?.sprite;
        }

        public Sprite GetItemPreview(ItemGameData item)
        {
            return item?.icon;
        }
#endif
    }
}