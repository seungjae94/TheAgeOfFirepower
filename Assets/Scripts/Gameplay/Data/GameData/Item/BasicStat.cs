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

        public string Description
        {
            get
            {
                string description = string.Empty;
                description += (maxHp > 0) ? $"내구력 + {maxHp}\n" :  string.Empty;
                description += (atk > 0) ? $"공격력 + {atk}\n" :  string.Empty;
                description += (def > 0) ? $"방어력 + {def}\n" :  string.Empty;
                description += (mob > 0) ? $"기동력 + {mob}\n" :  string.Empty;
                return description;
            }
        }
    }
}
