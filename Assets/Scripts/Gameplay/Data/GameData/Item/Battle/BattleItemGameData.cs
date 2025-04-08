using Sirenix.OdinInspector;

namespace Mathlife.ProjectL.Gameplay
{
    public class BattleItemGameData : CountableItemGameData
    {
        public override EItemType ItemType => EItemType.BattleItem;
        
        [LabelText("사용 효과")]
        [LabelWidth(100)]
        public BattleItemEffect effect;
    }
}