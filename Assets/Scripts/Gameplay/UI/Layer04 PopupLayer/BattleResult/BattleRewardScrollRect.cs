using System.Collections.Generic;

namespace Mathlife.ProjectL.Gameplay.UI
{
    public class RewardOrExp
    {
        public Reward reward;
        public long exp;
    }
    
    public class BattleRewardScrollRect
        : SimpleScrollRect<RewardOrExp, SimpleScrollRectContext>
    {
        public override void UpdateContents(IList<RewardOrExp> itemData)
        {
            base.UpdateContents(itemData);
        }
    }
}