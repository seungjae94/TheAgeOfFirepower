using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mathlife.ProjectL.Gameplay
{
    public enum EPageId
    {
        TeamPage,
        CharacterPage,
        InventoryPage,
        BattlePage
    }

    public enum EPrefabId
    {
        None = 0,
        CharacterCard = 1,
        DragCharacterCard = 2,
        EquipmentSlot = 3,
    }

    public enum EBattleTarget
    {
        None = 0,
        Self = 1,
        AllyAll = 2,
        Enemy = 3,
        EnemyAll = 4,
        SelfZone = 11,
        AllyAllZone = 12,
        EnemyZone = 13,
        EnemyAllZone = 14
    }

    public enum EDebuff
    {
        None = 0,
        Stun,
        Vulnerable,
        Defenseless,
        Tired,
        Exhausted,
        Poisoned,
        Burning,
    }

    public enum EComparisonOperator
    {
        Equal = 0,
        NotEqual = 1,
        GreaterThan = 2,
        GreatherThanOrEqualTo = 3,
        LessThan = 3,
        LessThanOrEqualTo = 4,
    }
}
