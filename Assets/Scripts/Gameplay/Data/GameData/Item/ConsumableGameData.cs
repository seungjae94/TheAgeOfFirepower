using Sirenix.OdinInspector;
using System;

namespace Mathlife.ProjectL.Gameplay
{
    [Serializable]
    public class ConsumableGameData : ItemGameData
    {
        [ShowInInspector]
        [ReadOnly]
        public override EItemType ItemType => EItemType.ConsumableItem;
    }
}
