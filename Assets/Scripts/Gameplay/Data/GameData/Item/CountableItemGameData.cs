using Sirenix.OdinInspector;

namespace Mathlife.ProjectL.Gameplay
{
    public abstract class CountableItemGameData : ItemGameData
    {
        [LabelWidth(100)]
        public int maxStack = 10000;
    }
}