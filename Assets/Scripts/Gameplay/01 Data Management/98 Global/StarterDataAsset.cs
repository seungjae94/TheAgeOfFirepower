using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

namespace Mathlife.ProjectL.Gameplay
{
    [CreateAssetMenu(fileName = "Starter Data Asset", menuName = "Data Asset/Starter Data")]
    public class StarterDataAsset : SerializedScriptableObject
    {
#if UNITY_EDITOR
        [TabGroup("Starter", "���")]
        [ListDrawerSettings(HideAddButton = true, HideRemoveButton = true)]
#endif
        public List<CharacterSlot> starterMembers = new List<CharacterSlot>();

#if UNITY_EDITOR
        [TabGroup("Starter", "����� �ƴ� ĳ����")]
#endif
        public List<CharacterSlot> starterNonMemberCharacters = new List<CharacterSlot>();

#if UNITY_EDITOR
        [TabGroup("Starter", "�������� ���� ���")]
#endif
        public List<EquipmentSlot> starterUnequippedEquipments = new List<EquipmentSlot>();

#if UNITY_EDITOR
        [TabGroup("Editor Starter", "���")]
        [ListDrawerSettings(HideAddButton = true, HideRemoveButton = true)]
        public List<CharacterSlot> editorStarterMembers = new List<CharacterSlot>();

        [TabGroup("Editor Starter", "����� �ƴ� ĳ����")]
        public List<CharacterSlot> editorStarterNonMemberCharacters = new List<CharacterSlot>();

        [TabGroup("Editor Starter", "�������� ���� ���")]
        public List<EquipmentSlot> editorStarterUnequippedEquipments = new List<EquipmentSlot>();

        [OnInspectorInit]
        public void OnInspectorInit()
        {
            InitializeMemberSlots(starterMembers);
            InitializeMemberSlots(editorStarterMembers);
        }

        public void InitializeMemberSlots(List<CharacterSlot> members)
        {
            if (members.Count < 4)
            {
                int addCount = 4 - members.Count;
                for (int i = 0; i < addCount; ++i)
                {
                    members.Add(new CharacterSlot());
                }
            }
        }

        [TabGroup("Editor Starter", "���")]
        [Button("������ ��� ����", Stretch = false, ButtonHeight = 30)]
        public void ResetEditorMembers()
        {
            editorStarterMembers = new();
            InitializeMemberSlots(editorStarterMembers);
        }
#endif


    }
}