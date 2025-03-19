using Sirenix.OdinInspector;
using System;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Serialization;

#if UNITY_EDITOR
using UnityEditor;
using Sirenix.OdinInspector.Editor;
#endif


namespace Mathlife.ProjectL.Gameplay
{
    public class ArtyGameData : GameData
    {
        [Title("화포 데이터", horizontalLine: true)]
        [HorizontalGroup("Sprites")]
        [LabelWidth(100)]
        [LabelText("스프라이트")]
        [PreviewField(Alignment = ObjectFieldAlignment.Left)]
        public Sprite sprite = null;

        [LabelText("무장(포탄)")]
        [LabelWidth(100)]
        [PreviewField(Alignment = ObjectFieldAlignment.Left, PreviewGetter = nameof(GetShellPreview))]
        public ShellGameData shell;
        
        
        [VerticalGroup("Stats")]
        [HorizontalGroup("Stats/내구력", Title = "Stats", Width = 200, LabelWidth = 100)]
        [LabelText("내구력")]
        [GUIColor(0.5f, 1.0f, 0.5f)]
        [SuffixLabel("at level 1")]
        public int maxHp = 500;

        [VerticalGroup("Stats")]
        [HorizontalGroup("Stats/내구력", Width = 100, LabelWidth = 20)]
        [LabelText(" + ")]
        [GUIColor(0.5f, 1.0f, 0.5f)]
        [SuffixLabel("per level")]
        public float maxHpGrowth = 25.0f;
        
        [VerticalGroup("Stats"), HorizontalGroup("Stats/파괴력", Width = 200, LabelWidth = 100)]
        [LabelText("파괴력")]
        [GUIColor(1.0f, 0.65f, 0.65f)]
        [SuffixLabel("at level 1")]
        public int atk = 60;

        [VerticalGroup("Stats"), HorizontalGroup("Stats/파괴력", Width = 100, LabelWidth = 20)]
        [LabelText(" + ")]
        [GUIColor(1.0f, 0.65f, 0.65f)]
        [SuffixLabel("per level")]
        public float atkGrowth = 6.0f;

        [VerticalGroup("Stats"), HorizontalGroup("Stats/방어력", Width = 200, LabelWidth = 100)]
        [LabelText("방어력")]
        [GUIColor(0.65f, 0.65f, 1.0f)]
        [SuffixLabel("at level 1")]
        public int def = 20;

        [VerticalGroup("Stats"), HorizontalGroup("Stats/방어력", Width = 100, LabelWidth = 20)]
        [LabelText(" + ")]
        [GUIColor(0.65f, 0.65f, 1.0f)]
        [SuffixLabel("per level")]
        public float defGrowth = 2.0f;
        
        [FormerlySerializedAs("spd")]
        [VerticalGroup("Stats"), HorizontalGroup("Stats/기동력", Width = 200, LabelWidth = 100)]
        [LabelText("기동력")]
        [GUIColor(1.0f, 1.0f, 0.5f)]
        [SuffixLabel("at level 1")]
        public int mob = 100;

        [FormerlySerializedAs("spdGrowth")]
        [VerticalGroup("Stats"), HorizontalGroup("Stats/기동력", Width = 100, LabelWidth = 20)]
        [LabelText(" + ")]
        [GUIColor(1.0f, 1.0f, 0.5f)]
        [SuffixLabel("per level")]
        public float mobGrowth = 4.0f;
        
#if UNITY_EDITOR
        private Texture2D GetShellPreview()
        {
            return shell?.icon?.texture;
        }
        
        public override void SetMenuItem(ref OdinMenuItem menuItem)
        {
            menuItem.Name = displayName;

            if (sprite != null)
                menuItem.Icon = sprite.texture;
        }
#endif
    }
}