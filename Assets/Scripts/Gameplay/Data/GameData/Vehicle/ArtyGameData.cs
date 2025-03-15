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
        [FormerlySerializedAs("portrait")]
        [Title("화포 데이터", horizontalLine: false)]
        [HorizontalGroup("Sprites")]
        [LabelWidth(75)]
        [LabelText("스프라이트")]
        [PreviewField(50, ObjectFieldAlignment.Left)]
        public Sprite sprite = null;

        [LabelWidth(75)]
        [LabelText("무장(포탄)")]
        [PreviewField(Alignment = ObjectFieldAlignment.Left, PreviewGetter = nameof(GetShellPreview))]
        public ShellGameData shell;
        
        [VerticalGroup("Stats"), HorizontalGroup("Stats/Max HP", Title = "Stats")]
        [LabelWidth(100)]
        [Range(10, 100), GUIColor(0.5f, 1.0f, 0.5f)]
        public int maxHp = 30;

        [VerticalGroup("Stats"), HorizontalGroup("Stats/Max HP")]
        [LabelText(" + "), LabelWidth(20)]
        [Range(0, 20), GUIColor(0.5f, 1.0f, 0.5f)]
        [SuffixLabel("per level")]
        public float maxHpGrowth = 5.0f;

        [VerticalGroup("Stats"), HorizontalGroup("Stats/Speed")]
        [LabelWidth(100)]
        [Range(80, 120), GUIColor(1.0f, 1.0f, 0.5f)]
        public int spd = 100;

        [VerticalGroup("Stats"), HorizontalGroup("Stats/Speed")]
        [LabelText(" + "), LabelWidth(20)]
        [Range(0, 4),GUIColor(1.0f, 1.0f, 0.5f)]
        [SuffixLabel("per level")]
        public float spdGrowth = 2.0f;

        [VerticalGroup("Stats"), HorizontalGroup("Stats/Attack")]
        [LabelWidth(100)]
        [Range(10, 60), GUIColor(1.0f, 0.65f, 0.65f)]
        public int atk = 30;

        [VerticalGroup("Stats"), HorizontalGroup("Stats/Attack")]
        [LabelText(" + "), LabelWidth(20)]
        [Range(0, 10), GUIColor(1.0f, 0.65f, 0.65f)]
        [SuffixLabel("per level")]
        public float atkGrowth = 5.0f;

        [VerticalGroup("Stats"), HorizontalGroup("Stats/Defense")]
        [LabelWidth(100)]
        [Range(0, 50), GUIColor(0.65f, 1.0f, 0.65f)]
        public int def = 20;

        [VerticalGroup("Stats"), HorizontalGroup("Stats/Defense")]
        [LabelText(" + "), LabelWidth(20)]
        [Range(0, 10), GUIColor(0.65f, 1.0f, 0.65f)]
        [SuffixLabel("per level")]
        public float defGrowth = 3.0f;
        
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