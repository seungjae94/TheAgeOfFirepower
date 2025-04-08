using Sirenix.OdinInspector;
using System;
using Mathlife.ProjectL.Gameplay.Play;

namespace Mathlife.ProjectL.Gameplay
{
    [Serializable]
    public class DoubleFireEffect : BattleItemEffect
    {
        public override void Apply(ArtyController target)
        {
            target.DoubleFireBuff();
        }
    }

    [Serializable]
    public class RefuelEffect : BattleItemEffect
    {
        [LabelWidth(100)]
        [LabelText("충전량")]
        public int amount;

        public override void Apply(ArtyController target)
        {
            target.Refuel(amount);
        }
    }

    [Serializable]
    public class RepairEffect : BattleItemEffect
    {
        [LabelWidth(100)]
        [LabelText("비율")]
        public float ratio;
        
        public override void Apply(ArtyController target)
        {
            target.Repair(ratio);
        }
    }

    [Serializable]
    [InlineProperty]
    public abstract class BattleItemEffect
    {
        public abstract void Apply(ArtyController target);
    }
}
