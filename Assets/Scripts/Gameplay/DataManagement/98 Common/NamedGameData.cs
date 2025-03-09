using Sirenix.OdinInspector;

#if UNITY_EDITOR
using Sirenix.OdinInspector.Editor;
#endif

namespace Mathlife.ProjectL.Gameplay
{
    public abstract class NamedGameData : SerializedScriptableObject
    {
        [Title("게임 공통 데이터")]
        [LabelWidth(100)]
        [LabelText("이름")]
        public string displayName = "";

        [LabelWidth(100)]
        [LabelText("ID")]
        public int id = 0;
        
#if UNITY_EDITOR
        public abstract void ToMenuItem(ref OdinMenuItem menuItem);
#endif
    }
}
