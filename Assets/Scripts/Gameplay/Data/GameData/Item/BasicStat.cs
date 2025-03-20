using Sirenix.OdinInspector;
using System;
using UnityEngine.Serialization;

namespace Mathlife.ProjectL.Gameplay
{
    [Serializable]
    [InlineProperty]
    public struct BasicStat
    {
        [LabelWidth(100)]
        [LabelText("차체 내구력")]
        public int maxHp;

        [LabelWidth(100)]
        [LabelText("공격력")]
        public int atk;

        [LabelWidth(100)]
        [LabelText("방어력")]
        public int def;

        [LabelWidth(100)]
        [LabelText("스피드")]
        public int mob;
    }
}
