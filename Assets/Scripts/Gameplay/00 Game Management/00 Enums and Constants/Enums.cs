using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mathlife.ProjectL.Gameplay
{
    public enum EPageId
    {
        HomePage,
        PartyPage,
        CharacterPage,
        CharacterDetailPage,
        InventoryPage,
        ShopPage,
        StageSelectionPage
    }

    public enum EPrefabId
    {
        None = 0,
        PartyMemberSlotItem,
        PartyMemberSlotDragItem,
        InventoryFlexItem,
        ShopFlexItem,
        CharacterSelectionFlexItem
    }

    public enum EBattleTarget
    {
        None = 0,
        Self,
        Ally,
        AllyAll,
        Enemy,
        EnemyAll,
        SelfZone,
        AllyAllZone,
        EnemyZone,
        EnemyAllZone
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
        All = 100
    }

    public enum EBuff
    {
        None = 0,
        Regeneration = 1,
        All = 100
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
