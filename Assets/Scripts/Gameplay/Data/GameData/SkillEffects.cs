using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Mathlife.ProjectL.Gameplay
{
    [Serializable]
    public class StatChangeSkillEffect : SkillEffect
    {
        [SerializeField]
        [LabelWidth(100)]
        [LabelText("스탯 상승량")]
        public BasicStat stat = new();
    }

    [Serializable]
    public class HealSkillEffect : SkillEffect
    {
        [LabelWidth(100)]
        [LabelText("힐 수치")]
        public int value;

        [LabelWidth(100)]
        [LabelText("마법 계수")]
        public float magCoeff;
    }

    [Serializable]
    public class BlockSkillEffect : SkillEffect
    {
        [LabelWidth(100)]
        [LabelText("보호막 수치")]
        public int value;

        [LabelWidth(100)]
        [LabelText("방어 계수")]
        public float defCoeff;
    }

    [Serializable]
    public class EnergyRecoverySkillEffect : SkillEffect
    {
        [LabelWidth(100)]
        [LabelText("에너지 회복량")]
        public int value;
    }

    [Serializable]
    public class DebuffSkillEffect : SkillEffect
    {
        [LabelWidth(100)]
        [LabelText("디버프 종류")]
        public EDebuff debuffType;

        [LabelWidth(100)]
        [LabelText("스택")]
        public int stack;

        [LabelWidth(100)]
        [LabelText("배수")]
        public float multiplier;
    }

    [Serializable]
    public class BuffSkillEffect : SkillEffect
    {
        [LabelWidth(100)]
        [LabelText("버프 종류")]
        public EBuff buffType;

        [LabelWidth(100)]
        [LabelText("스택")]
        public int stack;

        [LabelWidth(100)]
        [LabelText("배수")]
        public float multiplier;
    }

    [Serializable]
    public class CureSkillEffect : SkillEffect
    {
        [LabelWidth(100)]
        [LabelText("디버프 종류")]
        public EDebuff buffType;

        [LabelWidth(100)]
        [LabelText("스택")]
        public int stack;

        [LabelWidth(100)]
        [Range(0, 1)]
        [LabelText("배수")]
        public float multiplier;
    }

    [Serializable]
    public class DamageByDebuffSkillEffect : SkillEffect
    {
        [LabelWidth(100)]
        [LabelText("디버프 종류")]
        public EDebuff debuffType;

        [LabelWidth(100)]
        [LabelText("배수")]
        public float multiplier;
    }

    [Serializable]
    public class DamageSkillEffect : SkillEffect
    {
        [LabelWidth(100)]
        [LabelText("기본 데미지")]
        public int baseDamage;

        [LabelWidth(100)]
        [LabelText("공격 계수")]
        public float atkCoeff;

        [LabelWidth(100)]
        [LabelText("마법 계수")]
        public float magCoeff;
    }

    [Serializable]
    [InlineProperty]
    public abstract class SkillEffect
    {
        [LabelWidth(100)]
        [LabelText("발동 대상")]
        public EBattleTarget target;
    }
}
