using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

namespace Mathlife.ProjectL.Gameplay
{
    [CreateAssetMenu(fileName = "Starter Data Asset", menuName = "Data Asset/Starter Data")]
    public class StarterDataAsset : SerializedScriptableObject
    {
#if UNITY_EDITOR
        [TabGroup("Starter", "멤버")]
        [ListDrawerSettings(HideAddButton = true, HideRemoveButton = true)]
#endif
        public List<CharacterState> starterMembers = new List<CharacterState>();

#if UNITY_EDITOR
        [TabGroup("Starter", "멤버가 아닌 캐릭터")]
#endif
        public List<CharacterState> starterNonMemberCharacters = new List<CharacterState>();

#if UNITY_EDITOR
        [TabGroup("Starter", "장착하지 않은 장비")]
#endif
        public List<EquipmentStack> starterUnequippedEquipments = new List<EquipmentStack>();

#if UNITY_EDITOR
        [TabGroup("Editor Starter", "멤버")]
        [ListDrawerSettings(HideAddButton = true, HideRemoveButton = true)]
        public List<CharacterState> editorStarterMembers = new List<CharacterState>();

        [TabGroup("Editor Starter", "멤버가 아닌 캐릭터")]
        public List<CharacterState> editorStarterNonMemberCharacters = new List<CharacterState>();

        [TabGroup("Editor Starter", "장착하지 않은 장비")]
        public List<EquipmentStack> editorStarterUnequippedEquipments = new List<EquipmentStack>();

        [OnInspectorInit]
        public void OnInspectorInit()
        {
            InitializeMemberSlots(starterMembers);
            InitializeMemberSlots(editorStarterMembers);
        }

        public void InitializeMemberSlots(List<CharacterState> members)
        {
            if (members.Count < 4)
            {
                int addCount = 4 - members.Count;
                for (int i = 0; i < addCount; ++i)
                {
                    members.Add(new CharacterState());
                }
            }
        }

        [TabGroup("Editor Starter", "멤버")]
        [Button("에디터 멤버 리셋", Stretch = false, ButtonHeight = 30)]
        public void ResetEditorMembers()
        {
            editorStarterMembers = new();
            InitializeMemberSlots(editorStarterMembers);
        }
#endif


    }
}