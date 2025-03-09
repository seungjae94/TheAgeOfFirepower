using Mathlife.ProjectL.Utils;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

namespace Mathlife.ProjectL.Gameplay
{
    [CreateAssetMenu(fileName = "StarterSO", menuName = "SO/Starter SO")]
    public class StarterSO : SerializedScriptableObject
    {
        [SerializeField]
        [TitleGroup("���� ���� ����", GroupID = "Starter", Indent = true, HorizontalLine = true)]
        [LabelText("���� ���")]
        [PropertySpace(spaceBefore: 0, spaceAfter: 35)]
        long starterGold = 0L;

        [SerializeField]
        [TabGroup("Starter/Tab", "��Ƽ")]
        [HideReferenceObjectPicker]
        [LabelText("��Ƽ")]
        [ListDrawerSettings(HideAddButton = true, HideRemoveButton = true, ShowFoldout = false)]
        List<CharacterPreset> starterParty = new ();

        [SerializeField]
        [TabGroup("Starter/Tab", "���� ĳ���� (��Ƽ X)")]
        [HideReferenceObjectPicker]
        [LabelText("���� ĳ����")]
        [ListDrawerSettings(ShowFoldout = false)]
        List<CharacterPreset> starterCharactersNotInParty = new();

        [SerializeField]
        [Space(10)]
        [TabGroup("Starter/Tab", "���� ��� (���� X)")]
        [HideReferenceObjectPicker]
        [LabelText("���� ���")]
        [ListDrawerSettings(ShowFoldout = false)]
        List<EquipmentStack> starterEquipmentsNotOwned = new();

        [ShowInInspector]
        [SpaceOnly(50)]
        bool _dummySpacing;

#if UNITY_EDITOR
        [SerializeField]
        [TitleGroup("������ ���� ����", GroupID = "Editor Starter", Indent = true, HorizontalLine = true)]
        [LabelText("������ ���� ���")]
        [PropertySpace(spaceBefore: 0, spaceAfter: 35)]
        long editorStarterGold = 0L;

        [SerializeField]
        [TabGroup("Editor Starter/Tab", "��Ƽ")]
        [HideReferenceObjectPicker]
        [LabelText("��Ƽ")]
        [ListDrawerSettings(HideAddButton = true, HideRemoveButton = true, ShowFoldout = false)]
        List<CharacterPreset> editorStarterParty = new();

        [SerializeField]
        [TabGroup("Editor Starter/Tab", "���� ĳ���� (��Ƽ X)")]
        [HideReferenceObjectPicker]
        [LabelText("���� ĳ����")]
        [ListDrawerSettings(ShowFoldout = false)]
        List<CharacterPreset> editorStarterCharactersNotInParty = new();

        [SerializeField]
        [TabGroup("Editor Starter/Tab", "���� ��� (���� X)")]
        [HideReferenceObjectPicker]
        [LabelText("���� ���")]
        [ListDrawerSettings(ShowFoldout = false)]
        List<EquipmentStack> editorStarterEquipmentsNotOwned = new();

        [OnInspectorInit]
        void OnInspectorInit()
        {
            InitializeMemberSlots(starterParty);
            InitializeMemberSlots(editorStarterParty);
        }

        void InitializeMemberSlots(List<CharacterPreset> members)
        {
            if (members.Count < 4)
            {
                int addCount = 4 - members.Count;
                for (int i = 0; i < addCount; ++i)
                {
                    members.Add(new CharacterPreset());
                }
            }
        }

        [TabGroup("Editor Starter/Tab", "��Ƽ")]
        [Button("������ ��� ����", Stretch = false, ButtonHeight = 30)]
        void ResetEditorMembers()
        {
            editorStarterParty = new();
            InitializeMemberSlots(editorStarterParty);
        }

        public void StopWarning()
        {
            var a = starterGold;
            var b = starterParty;
            var c = starterCharactersNotInParty;
            var d = starterEquipmentsNotOwned;
        }
#endif

        public long GetStarterGold()
        {
#if UNITY_EDITOR
            return editorStarterGold;
#else
            return starterGold;
#endif
        }

        public List<CharacterPreset> GetStarterParty()
        {
#if UNITY_EDITOR
            return editorStarterParty;
#else
            return starterParty;
#endif
        }

        public List<CharacterPreset> GetStarterCharactersNotInParty()
        {
#if UNITY_EDITOR
            return editorStarterCharactersNotInParty;
#else
            return starterPartyNotInParty;
#endif
        }

        public List<EquipmentStack> GetStarterEquipmentsNotOwned()
        {
#if UNITY_EDITOR
            return editorStarterEquipmentsNotOwned;
#else
            return starterEquipmentsNotOwned;
#endif
        }
    }
}