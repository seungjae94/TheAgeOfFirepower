using Mathlife.ProjectL.Utils;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

#if UNITY_EDITOR
using Sirenix.Utilities.Editor;
#endif

namespace Mathlife.ProjectL.Gameplay
{
    public class StarterGameData : SerializedScriptableObject
    {
        [SerializeField]
        [TitleGroup("릴리즈 모드 스타터", GroupID = "Starter", Indent = true, HorizontalLine = true)]
        [LabelText("골드")]
        [PropertySpace(spaceBefore: 0, spaceAfter: 35)]
        long starterGold = 0L;

        [SerializeField]
        [TabGroup("Starter/Tab", "포대")]
        [HideReferenceObjectPicker]
        [LabelText("포대")]
        [ListDrawerSettings(HideAddButton = true, HideRemoveButton = true, ShowFoldout = false)]
        List<ArtyPreset> starterBattery = new ();
        
        [SerializeField]
        [TabGroup("Starter/Tab", "대기 멤버")]
        [HideReferenceObjectPicker]
        [LabelText("대기 멤버")]
        [ListDrawerSettings(ShowFoldout = false)]
        List<ArtyPreset> starterBench = new();
        
        [SerializeField]
        [Space(10)]
        [TabGroup("Starter/Tab", "인벤토리(백업 부품)")]
        [HideReferenceObjectPicker]
        [LabelText("백업 부품")]
        [ListDrawerSettings(ShowFoldout = false)]
        List<MechPartStack> starterBackupMechParts = new();

        [ShowInInspector]
        [SpaceOnly(50)]
        bool _dummySpacing;

#if UNITY_EDITOR
        [SerializeField]
        [TitleGroup("디버그 모드 스타터", GroupID = "Editor Starter", Indent = true, HorizontalLine = true)]
        [LabelText("골드")]
        [PropertySpace(spaceBefore: 0, spaceAfter: 35)]
        long editorStarterGold = 0L;

        [SerializeField]
        [TabGroup("Editor Starter/Tab", "포대")]
        [HideReferenceObjectPicker]
        [LabelText("포대")]
        [ListDrawerSettings(HideAddButton = true, HideRemoveButton = true, ShowFoldout = false)]
        List<ArtyPreset> editorStarterBattery = new();
        
        [SerializeField]
        [TabGroup("Editor Starter/Tab", "대기 멤버")]
        [HideReferenceObjectPicker]
        [LabelText("대기 멤버")]
        [ListDrawerSettings(ShowFoldout = false)]
        List<ArtyPreset> editorStarterBench = new();
        
        [SerializeField]
        [TabGroup("Editor Starter/Tab", "인벤토리(백업 부품)")]
        [HideReferenceObjectPicker]
        [LabelText("백업 부품")]
        [ListDrawerSettings(ShowFoldout = false)]
        List<MechPartStack> editorStarterBackupMechParts = new();

        [OnInspectorInit]
        void OnInspectorInit()
        {
            InitializeMemberSlots(starterBattery);
            InitializeMemberSlots(editorStarterBattery);
        }

        void InitializeMemberSlots(List<ArtyPreset> members)
        {
            if (members.Count < 3)
            {
                int addCount = 3 - members.Count;
                for (int i = 0; i < addCount; ++i)
                {
                    members.Add(new ArtyPreset());
                }
            }
        }

        [TabGroup("Editor Starter/Tab", "포대")]
        [Button("포대 초기화", Stretch = false, ButtonHeight = 30)]
        void ResetEditorMembers()
        {
            editorStarterBattery = new();
            InitializeMemberSlots(editorStarterBattery);
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

        public List<ArtyPreset> GetStarterBattery()
        {
#if UNITY_EDITOR
            return editorStarterBattery;
#else
            return starterBattery;
#endif
        }

        public List<ArtyPreset> GetStarterBench()
        {
#if UNITY_EDITOR
            return editorStarterBench;
#else
            return starterBench;
#endif
        }

        public List<MechPartStack> GetStarterBackupMechParts()
        {
#if UNITY_EDITOR
            return editorStarterBackupMechParts;
#else
            return starterBackupMechParts;
#endif
        }
    }
}