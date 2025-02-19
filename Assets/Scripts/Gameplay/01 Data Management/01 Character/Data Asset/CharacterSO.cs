using Sirenix.OdinInspector;
using System;
using UnityEngine;

#if UNITY_EDITOR
using Sirenix.OdinInspector.Editor;
#endif


namespace Mathlife.ProjectL.Gameplay
{
    public enum ECharacterId
    {
        None = 0,
        Emily,
        Aiden,
        Chloe,
        Leah,
        Noah,
        Tristan,
        Max
    }

    public class CharacterSO : NamedSO
    {
        public override int intId => (int)id;

        [Title("캐릭터 데이터", horizontalLine: false)]
        [LabelWidth(100)]
        public ECharacterId id = ECharacterId.None;

        [HorizontalGroup("Sprites")]
        [LabelWidth(75)]
        [PreviewField(50, ObjectFieldAlignment.Left)]
        [AssetSelector(Paths = "Assets/Arts/Character/Portrait", FlattenTreeView = true)]
        public Sprite portrait = null;

        [HorizontalGroup("Sprites")]
        [LabelWidth(75)]
        [PreviewField(50, ObjectFieldAlignment.Left)]
        [AssetSelector(Paths = "Assets/Arts/Character/Battler", FlattenTreeView = true)]
        [PropertySpace(0.0f, 20.0f)]
        public Sprite battler = null;

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

        [VerticalGroup("Stats"), HorizontalGroup("Stats/Magic")]
        [LabelWidth(100)]
        [Range(0, 50), GUIColor(0.65f, 0.65f, 1.0f)]
        public int mag = 20;

        [VerticalGroup("Stats"), HorizontalGroup("Stats/Magic")]
        [LabelText(" + "), LabelWidth(20)]
        [Range(0, 10), GUIColor(0.65f, 0.65f, 1.0f)]
        [SuffixLabel("per level")]
        public float magGrowth = 3.0f;

#if UNITY_EDITOR
        public override void ToMenuItem(ref OdinMenuItem menuItem)
        {
            menuItem.Name = displayName;

            if (portrait != null)
                menuItem.Icon = portrait.texture;
        }
#endif
    }
}