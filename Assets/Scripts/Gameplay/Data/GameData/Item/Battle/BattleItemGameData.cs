namespace Mathlife.ProjectL.Gameplay.Battle
{
    public class BattleItemGameData : CountableItemGameData
    {
        public override EItemType ItemType => EItemType.BattleItem;
        
        // TODO: abstract UseEffect (어느 타이밍에 어떻게 적용하느냐에 따라 여러 함수로 나눠야 할 수도)
    }
}