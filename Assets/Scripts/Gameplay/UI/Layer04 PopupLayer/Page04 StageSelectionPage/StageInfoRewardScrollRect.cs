using System.Collections.Generic;

namespace Mathlife.ProjectL.Gameplay.UI
{
    public class StageInfoRewardScrollRect
    : SimpleScrollRect<Reward, SimpleScrollRectContext>
    {
        
        public override void UpdateContents(IList<Reward> itemData)
        {
            base.UpdateContents(itemData);
        }
    }
}