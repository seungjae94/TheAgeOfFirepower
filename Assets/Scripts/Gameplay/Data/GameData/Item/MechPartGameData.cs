using Sirenix.OdinInspector;
using UnityEngine;
using System;


#if UNITY_EDITOR
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
#endif

namespace Mathlife.ProjectL.Gameplay
{
    public enum EEquipmentType
    {
        Barrel,
        Armor,
        Engine
    }

    [Serializable]
    public class MechPartGameData : ItemGameData
    {
        [ShowInInspector]
        [ReadOnly]
        public override EItemType ItemType => EItemType.MechPart;
        
        [Title("부품 데이터", horizontalLine: true)]
        [LabelWidth(100)]
        [LabelText("부품 타입")]
        public EEquipmentType type;

        [LabelWidth(100)]
        [LabelText("스탯")]
        [InlineProperty]
        public BasicStat stat;
    }
}
