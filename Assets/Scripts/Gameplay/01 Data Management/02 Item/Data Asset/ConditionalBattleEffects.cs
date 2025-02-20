using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mathlife.ProjectL.Gameplay
{
    public enum EBattleEventCondition
    {
        Never = 0,
        OnEveryTurn = 1,
        OnSpecificTurn = 2,
    }

    [Serializable]
    public class ConditionalBlock : ConditionalBattleEffect
    {
        [HideIf("@condition==EBattleEventCondition.Never")]
        [LabelWidth(100)]
        [LabelText("보호막 수치")]
        public int value;
    }

    [Serializable]
    public class ConditionalHeal : ConditionalBattleEffect
    {
        [HideIf("@condition==EBattleEventCondition.Never")]
        [LabelWidth(100)]
        [LabelText("힐 수치")]
        public int value;
    }

    [Serializable]
    public class ConditionalDamage : ConditionalBattleEffect
    {
        [LabelWidth(100)]
        [LabelText("데미지 대상")]
        public EBattleTarget target;

        [HideIf("@condition==EBattleEventCondition.Never")]
        [LabelWidth(100)]
        [LabelText("데미지 수치")]
        public int value;
    }

    [Serializable]
    public class ConditionalDebuff : ConditionalBattleEffect
    {
        [LabelWidth(100)]
        [LabelText("디버프 종류")]
        public EDebuff debuff;

        [HideIf("@condition==EBattleEventCondition.Never")]
        [LabelWidth(100)]
        [LabelText("스택")]
        public int stack;
    }

    [Serializable]
    [InlineProperty]
    public abstract class ConditionalBattleEffect
    {
        [LabelWidth(100)]
        [LabelText("발동 조건")]
        public EBattleEventCondition condition;

        [ShowIf("@condition==EBattleEventCondition.OnSpecificTurn")]
        [LabelWidth(100)]
        [LabelText("발동 턴")]
        public int turn;
    }
}
