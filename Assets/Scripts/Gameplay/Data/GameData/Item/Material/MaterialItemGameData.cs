using Sirenix.OdinInspector;
using System;

namespace Mathlife.ProjectL.Gameplay
{
    [Serializable]
    public class MaterialItemGameData : CountableItemGameData
    {
        [ShowInInspector]
        [ReadOnly]
        public override EItemType ItemType => EItemType.MaterialItem;

        [LabelText("획득량")]
        [LabelWidth(100)]
        public long gainValue;
    }
}
