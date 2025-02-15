using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mathlife.ProjectL.Gameplay
{
    public abstract class NamedSO : SerializedScriptableObject
    {
        [Title("게임 공통 데이터")]
        [LabelWidth(100)]
        [LabelText("이름")]
        public string displayName = "";

        public abstract int intId { get; }
    }
}
