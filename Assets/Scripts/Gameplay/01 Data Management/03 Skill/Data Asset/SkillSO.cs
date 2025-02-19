using Sirenix.OdinInspector;
using UnityEngine;

#if UNITY_EDITOR
using Sirenix.OdinInspector.Editor;
#endif

namespace Mathlife.ProjectL.Gameplay
{
    public enum ESkillId
    {
        None = 0,

        // 에이든
        Slash = 1,
        Parrying = 2,
        SpinningSlash = 3,

        // 에밀리
        
    }

    public class SkillSO : NamedSO
    {
        public override int intId => (int) cardId;

        [LabelText("ID")]
        public ESkillId cardId;

        [LabelText("캐릭터 ID")]
        public ECharacterId characterId;

        [LabelText("삽화")]
        [PreviewField(Height = 300, Alignment = ObjectFieldAlignment.Left)]
        public Sprite illustration;

        [MinValue(0)]
        [LabelText("코스트")]
        public int cost = 0;

        [MinValue(1)]
        [LabelText("최대 포함 개수")]
        public int maxCount = 1;

        [LabelText("스킬 설명 템플릿")]
        [Multiline(10)]
        public string descriptionTemplate = "";

#if UNITY_EDITOR
        public override void ToMenuItem(ref OdinMenuItem menuItem)
        {
            throw new System.NotImplementedException();
        }
#endif
    }
}
