using System;
using UnityEngine;
using Sirenix.OdinInspector;

#if UNITY_EDITOR
using ObjectFieldAlignment = Sirenix.OdinInspector.ObjectFieldAlignment;
#endif

namespace Mathlife.ProjectL.Gameplay
{
    [Serializable]
    public class ArtyPreset
    {
#if UNITY_EDITOR
        [LabelText("화포")]
        [HorizontalGroup(group: "Basic", LabelWidth = 50, Width = 125)]
        [PreviewField(Alignment = ObjectFieldAlignment.Left, PreviewGetter = nameof(GetArtyPreview))]
#endif
        public ArtyGameData arty;

        [LabelText("레벨")]
        [HorizontalGroup(GroupID = "Basic", LabelWidth = 50, Width = 125)]
        [VerticalGroup(GroupID = "Basic/Growth")]
        [MinValue(0)]
        public int level;

        [LabelText("경험치")]
        [HorizontalGroup(group: "Basic", LabelWidth = 50, Width = 125)]
        [VerticalGroup(GroupID = "Basic/Growth")]
        [MinValue(0)]
        public int currentLevelExp;

        [LabelText("포신")]
        [HorizontalGroup(group: "MechPart", LabelWidth = 50, Width = 125)]
#if UNITY_EDITOR
        //[ValidateInput(nameof(IsBarrel), "포신이 아닙니다.")]
        [PreviewField(Alignment = ObjectFieldAlignment.Left, PreviewGetter = nameof(GetBarrelPreview))]
#endif
        public MechPartGameData barrel;

        [LabelText("장갑")]
        [HorizontalGroup(group: "MechPart", LabelWidth = 50, Width = 125)]
#if UNITY_EDITOR
        //[ValidateInput(nameof(IsArmor), "장갑이 아닙니다.")]
        [PreviewField(Alignment = ObjectFieldAlignment.Left, PreviewGetter = nameof(GetArmorPreview))]
#endif
        public MechPartGameData armor;

        [LabelText("엔진")]
        [HorizontalGroup(group: "MechPart", LabelWidth = 50, Width = 125)]
#if UNITY_EDITOR
        //[ValidateInput(nameof(IsEngine), "엔진이 아닙니다.")]
        [PreviewField(Alignment = ObjectFieldAlignment.Left, PreviewGetter = nameof(GetEnginePreview))]
#endif
        public MechPartGameData engine;

#if UNITY_EDITOR
        public Texture2D GetArtyPreview()
        {
            return arty?.sprite?.texture;
        }

        public Texture2D GetBarrelPreview()
        {
            return barrel?.icon?.texture;
        }

        public Texture2D GetArmorPreview()
        {
            return armor?.icon?.texture;
        }

        public Texture2D GetEnginePreview()
        {
            return engine?.icon?.texture;
        }
#endif
    }
}
