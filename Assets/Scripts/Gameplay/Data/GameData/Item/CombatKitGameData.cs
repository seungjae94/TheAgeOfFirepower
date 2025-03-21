using Sirenix.OdinInspector;
using System;

namespace Mathlife.ProjectL.Gameplay
{
    [Serializable]
    public class CombatKitGameData : ItemGameData
    {
        [ShowInInspector]
        [ReadOnly]
        public override EItemType ItemType => EItemType.BattleItem;
    }
}
