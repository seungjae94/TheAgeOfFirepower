// using Sirenix.OdinInspector;
// using UnityEngine;
// using System.Collections.Generic;
//
// #if UNITY_EDITOR
// using Sirenix.OdinInspector.Editor;
// #endif
//
// namespace Mathlife.ProjectL.Gameplay
// {
//     public enum ESkillId
//     {
//         None = 0,
//         Slash,
//         Guard,
//         Overflow,
//         SpinningSlash,
//         DeepBreathe,
//         PowerSlash,
//         Parry,
//         BladeDance,
//         QuickShot,
//         SpeedUp,
//         PoisonShot,
//         ScatterShot,
//         DeadlyPoisonShot,
//         Fireball,
//         Ignite,
//         Flashover,
//         Pyroblast,
//         Backdraft,
//         Incinerate,
//         HolyRay,
//         Heal,
//         Bless,
//         PrayerOfRegeneration,
//         DivineJudgement,
//         Purify,
//         CelestialSalvation
//     }
//
//     public class SkillGameData : GameData
//     {
//         [Title("스킬 데이터", horizontalLine: false)]
//         [LabelWidth(100)]
//         [LabelText("설명")]
//         [Multiline(10)]
//         public string description = "";
//
//         [LabelWidth(75)]
//         [LabelText("아이콘")]
//         [PreviewField(50, Sirenix.OdinInspector.ObjectFieldAlignment.Left)]
//         [AssetSelector(Paths = "Assets/Arts/Skill Icons", FlattenTreeView = true)]
//         public Sprite icon = null;
//
//         [MinValue(1)]
//         [LabelText("사용 가능 횟수")]
//         public int count = 1;
//
//         [MinValue(0)]
//         [LabelText("에너지 소모")]
//         public int cost = 0;
//
//         [PropertySpace(spaceBefore: 20, spaceAfter: 0)]
//         [LabelText("스킬 효과")]
//         [SerializeReference]
//         [PolymorphicDrawerSettings(ShowBaseType = true)]
//         public List<SkillEffect> skillEffects = new();
//
// #if UNITY_EDITOR
//         public override void SetMenuItem(ref OdinMenuItem menuItem)
//         {
//             menuItem.Name = displayName;
//             menuItem.Icon = GetPreview();
//         }
//
//         public Texture2D GetPreview()
//         {
//             if (icon == null) return null;
//
//             //Rect texRect = icon.textureRect;
//             //menuItem.Icon = icon.texture.CropTexture(texRect);
//
//             return icon.texture;
//         }
// #endif
//     }
// }
