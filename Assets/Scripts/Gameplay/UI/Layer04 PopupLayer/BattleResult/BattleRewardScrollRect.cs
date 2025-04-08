using System.Collections.Generic;

namespace Mathlife.ProjectL.Gameplay.UI
{
    public class BattleRewardScrollRect
        : SimpleScrollRect<Reward, SimpleScrollRectContext>
    {
        public override void UpdateContents(IList<Reward> itemData)
        {
            itemData.Insert(0, null);
            base.UpdateContents(itemData);
        }
    }
}