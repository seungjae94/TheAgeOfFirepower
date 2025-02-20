using Sirenix.OdinInspector;
using UnityEngine;
using System.Collections.Generic;
using Sirenix.Utilities.Editor;



#if UNITY_EDITOR
using Sirenix.OdinInspector.Editor;
#endif

namespace Mathlife.ProjectL.Gameplay
{
    public enum ESkillId
    {
        None = 0,
        Slash,
        Guard,
        Overflow,
        SpinningSlash,
        DeepBreathe,
        PowerSlash,
        Parry,
        BladeDance,
        QuickShot,
        SpeedUp,
        PoisonShot,
        ScatterShot,
        DeadlyPoisonShot,
        Fireball,
        Recover,
        Ignite,
        Flashover,
        Backdraft,
        Incinerate,
        HolyRay,
        Heal,
        Bless,
        PrayerOfRegeneration,
        DivineJudgement,
        Purify,
        CelestialSalvation
    }

    public class SkillSO : NamedSO
    {
        public override int intId => (int) id;

        [LabelWidth(100)]
        [LabelText("ID")]
        public ESkillId id;

        [LabelWidth(100)]
        [LabelText("설명")]
        [Multiline(10)]
        public string description = "";

        [LabelWidth(75)]
        [LabelText("아이콘")]
        [PreviewField(50, Sirenix.OdinInspector.ObjectFieldAlignment.Left)]
        [AssetSelector(Paths = "Assets/Arts/Skill Icons", FlattenTreeView = true)]
        public Sprite icon = null;

        [MinValue(1)]
        [LabelText("사용 가능 횟수")]
        public int count = 1;

        [MinValue(0)]
        [LabelText("에너지 소모")]
        public int cost = 0;

        [PropertySpace(spaceBefore: 20, spaceAfter: 0)]
        [LabelText("스킬 효과")]
        [SerializeReference]
        [PolymorphicDrawerSettings(ShowBaseType = true)]
        public List<SkillEffect> skillEffects = new();

#if UNITY_EDITOR
        public override void ToMenuItem(ref OdinMenuItem menuItem)
        {
            menuItem.Name = displayName;

            if (icon != null)
            {
                Rect texRect = icon.textureRect;
                menuItem.Icon = icon.texture.CropTexture(texRect);
            }
        }
#endif
    }
}
