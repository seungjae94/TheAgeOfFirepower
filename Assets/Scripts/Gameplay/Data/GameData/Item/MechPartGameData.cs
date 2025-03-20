using Sirenix.OdinInspector;
using UnityEngine;
using System;


#if UNITY_EDITOR
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
#endif

namespace Mathlife.ProjectL.Gameplay
{
    public enum EMechPartType
    {
        Barrel,
        Armor,
        Engine
    }

    public static class EMechPartTypeExtensions
    {
        public static string ToDisplayName(this EMechPartType type)
        {
            return type switch
            {
                EMechPartType.Barrel => "화포",
                EMechPartType.Armor => "장갑",
                EMechPartType.Engine => "엔진",
                _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
            };
        }
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
        public EMechPartType type;

        [LabelWidth(100)]
        [LabelText("스탯")]
        [InlineProperty]
        public BasicStat stat;
    }
}
