using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mathlife.ProjectL.Gameplay
{
    [InlineProperty]
    public struct EquipmentStat
    {
        [LabelWidth(100)]
        [LabelText("최대 체력")]
        public int maxHp;

        [LabelWidth(100)]
        [LabelText("최대 에너지")]
        public int maxEnergy;

        [LabelWidth(100)]
        [LabelText("에너지 회복량")]
        public int energyRecovery;

        [LabelWidth(100)]
        [LabelText("공격력")]
        public int atk;

        [LabelWidth(100)]
        [LabelText("방어력")]
        public int def;

        [LabelWidth(100)]
        [LabelText("마력")]
        public int mag;

        [LabelWidth(100)]
        [LabelText("스피드")]
        public int spd;

        [LabelWidth(100)]
        [LabelText("치명타 확률")]
        [SuffixLabel("%", overlay: true)]
        public float criticalPer;
    }
}
