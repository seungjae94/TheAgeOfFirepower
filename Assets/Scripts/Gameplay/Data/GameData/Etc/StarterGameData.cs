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
        [HorizontalGroup(GroupID = "Starter/Currency", Width = 150, LabelWidth = 60)]
        [MinValue(0)]
        [LabelText("골드")]
        private long starterGold = 0L;


        [SerializeField]
        [LabelText("다이아")]
        [HorizontalGroup(GroupID = "Starter/Currency", Gap = 50, Width = 150, LabelWidth = 60)]
        [MinValue(0)]
        private long starterDiamond = 0L;

#if UNITY_EDITOR
        [ShowInInspector]
        [HorizontalGroup(GroupID = "Starter/Dummy00")]
        [SpaceOnly(15)]
        private bool dummy00;
#endif

        [SerializeField]
        [LabelText("월드")]
        [HorizontalGroup(GroupID = "Starter/Stage", Width = 150, LabelWidth = 60)]
        [MinValue(1)]
        private int starterUnlockWorldNo = 1;

        [SerializeField]
        [LabelText("스테이지")]
        [HorizontalGroup(GroupID = "Starter/Stage", Gap = 50, Width = 150, LabelWidth = 60)]
        [MinValue(1)]
        private int starterUnlockStageNo = 1;

#if UNITY_EDITOR
        [ShowInInspector]
        [HorizontalGroup(GroupID = "Starter/Dummy01")]
        [SpaceOnly(15)]
        private bool dummy01;
#endif

        [SerializeField]
        [TabGroup("Starter/Tab", "포대")]
        [HideReferenceObjectPicker]
        [LabelText("포대")]
        [ListDrawerSettings(HideAddButton = true, HideRemoveButton = true, ShowFoldout = false)]
        List<ArtyPreset> starterBattery = new();

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

        [SerializeField]
        [TabGroup("Starter/Tab", "인벤토리(재료, 전투)")]
        [HideReferenceObjectPicker]
        [LabelText("재료/전투 아이템")]
        [ListDrawerSettings(ShowFoldout = false)]
        List<CountableItemStack> starterItemStacks = new();

        [ShowInInspector]
        [SpaceOnly(50)]
        bool _dummySpacing;

#if UNITY_EDITOR
        [SerializeField]
        [TitleGroup("디버그 모드 스타터", GroupID = "EditorStarter", Indent = true, HorizontalLine = true)]
        [HorizontalGroup(GroupID = "EditorStarter/Currency", Width = 150, LabelWidth = 60)]
        [LabelText("골드")]
        private long editorStarterGold = 0L;

        [SerializeField]
        [LabelText("다이아")]
        [HorizontalGroup(GroupID = "EditorStarter/Currency", Gap = 50, Width = 150, LabelWidth = 60)]
        private long editorStarterDiamond = 0L;
        
        [ShowInInspector]
        [HorizontalGroup(GroupID = "EditorStarter/Dummy10")]
        [SpaceOnly(15)]
        private bool dummy10;

        [SerializeField]
        [LabelText("월드")]
        [HorizontalGroup(GroupID = "EditorStarter/Stage", Width = 150, LabelWidth = 60)]
        [MinValue(1)]
        private int editorStarterUnlockWorldNo = 1;

        [SerializeField]
        [LabelText("스테이지")]
        [HorizontalGroup(GroupID = "EditorStarter/Stage", Gap = 50, Width = 150, LabelWidth = 60)]
        [MinValue(1)]
        private int editorStarterUnlockStageNo = 1;
        
        [ShowInInspector]
        [HorizontalGroup(GroupID = "EditorStarter/Dummy11")]
        [SpaceOnly(15)]
        private bool dummy11;

        [SerializeField]
        [TabGroup("EditorStarter/Tab", "포대")]
        [HideReferenceObjectPicker]
        [LabelText("포대")]
        [ListDrawerSettings(HideAddButton = true, HideRemoveButton = true, ShowFoldout = false)]
        private List<ArtyPreset> editorStarterBattery = new();

        [SerializeField]
        [TabGroup("EditorStarter/Tab", "대기 멤버")]
        [HideReferenceObjectPicker]
        [LabelText("대기 멤버")]
        [ListDrawerSettings(ShowFoldout = false)]
        private List<ArtyPreset> editorStarterBench = new();

        [SerializeField]
        [TabGroup("EditorStarter/Tab", "인벤토리(백업 부품)")]
        [HideReferenceObjectPicker]
        [LabelText("백업 부품")]
        [ListDrawerSettings(ShowFoldout = false)]
        private List<MechPartStack> editorStarterBackupMechParts = new();

        [SerializeField]
        [TabGroup("EditorStarter/Tab", "인벤토리(재료, 전투)")]
        [HideReferenceObjectPicker]
        [LabelText("재료/전투 아이템")]
        [ListDrawerSettings(ShowFoldout = false)]
        private List<CountableItemStack> editorStarterItemStacks = new();

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

        [TabGroup("EditorStarter/Tab", "포대")]
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

        public long GetStarterDiamond()
        {
#if UNITY_EDITOR
            return editorStarterDiamond;
#else
            return starterDiamond;
#endif
        }
        
        public int GetStarterUnlockWorldNo()
        {
#if UNITY_EDITOR
            return editorStarterUnlockWorldNo;
#else
            return starterUnlockWorldNo;
#endif
        }
        
        public int GetStarterUnlockStageNo()
        {
#if UNITY_EDITOR
            return editorStarterUnlockStageNo;
#else
            return starterUnlockStageNo;
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

        public List<CountableItemStack> GetStarterItemStacks()
        {
#if UNITY_EDITOR
            return editorStarterItemStacks;
#else
            return starterItemStacks;
#endif
        }
    }
}