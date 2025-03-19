using Sirenix.OdinInspector;

#if UNITY_EDITOR
using Sirenix.OdinInspector.Editor;
using UnityEditor;
#endif

namespace Mathlife.ProjectL.Gameplay
{
    public abstract class GameData : SerializedScriptableObject
    {
        [Title("공통 데이터")]
        [LabelWidth(100)]
        [LabelText("이름")]
        public string displayName = "";

        [LabelWidth(100)]
        [LabelText("ID")]
        [ReadOnly]
        public int id = 0;
        
#if UNITY_EDITOR
        public abstract void SetMenuItem(ref OdinMenuItem menuItem);
#endif
    }
}
