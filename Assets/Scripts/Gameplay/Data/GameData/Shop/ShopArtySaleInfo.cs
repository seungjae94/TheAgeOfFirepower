using System;
using Mathlife.ProjectL.Utils;
using UnityEngine;

#if UNITY_EDITOR
using Sirenix.OdinInspector;
using ObjectFieldAlignment = Sirenix.OdinInspector.ObjectFieldAlignment;
#endif

namespace Mathlife.ProjectL.Gameplay
{
    [Serializable]
    public class ShopArtySaleInfo
    {
#if UNITY_EDITOR
        [LabelText("@GetDisplayName()")]
        [PreviewField(Alignment = ObjectFieldAlignment.Left, PreviewGetter = nameof(GetArtyPreview))]
        [HorizontalGroup(group: "Row", Width = 150, LabelWidth = 100)]
#endif
        public ArtyGameData arty;

#if UNITY_EDITOR
        [ShowInInspector]
        [SpaceOnly(0)]
        [HorizontalGroup(group: "Row", Width = 125, LabelWidth = 50)]
        private bool _dummy0;

        [LabelText("가격")]
        [HorizontalGroup(group: "Row", Width = 125, LabelWidth = 50)]
#endif
        public int price;

#if UNITY_EDITOR
        [ShowInInspector]
        [SpaceOnly(0)]
        [HorizontalGroup(group: "Row", Width = 125, LabelWidth = 50)]
        private bool _dummy1;

        [LabelText("개수")]
        [HorizontalGroup(group: "Row", Width = 125, LabelWidth = 50)]
#endif
        public int amount;

#if UNITY_EDITOR
        public Sprite GetArtyPreview()
        {
            return arty?.sprite;
        }

        public string GetDisplayName()
        {
            return arty != null ? arty.displayName : string.Empty;
        }
#endif
    }
}