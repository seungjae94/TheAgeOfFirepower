using Mathlife.ProjectL.Utils;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

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
        [TabGroup("Starter/Tab", "파티")]
        [HideReferenceObjectPicker]
        [LabelText("파티")]
        [ListDrawerSettings(HideAddButton = true, HideRemoveButton = true, ShowFoldout = false)]
        List<VehiclePreset> starterParty = new ();

        [SerializeField]
        [TabGroup("Starter/Tab", "로스터")]
        [HideReferenceObjectPicker]
        [LabelText("로스터")]
        [ListDrawerSettings(ShowFoldout = false)]
        List<VehiclePreset> starterRoster = new();

        [SerializeField]
        [Space(10)]
        [TabGroup("Starter/Tab", "인벤토리(부품)")]
        [HideReferenceObjectPicker]
        [LabelText("부품")]
        [ListDrawerSettings(ShowFoldout = false)]
        List<MechPartStack> starterMechParts = new();

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
        [TabGroup("Editor Starter/Tab", "파티")]
        [HideReferenceObjectPicker]
        [LabelText("파티")]
        [ListDrawerSettings(HideAddButton = true, HideRemoveButton = true, ShowFoldout = false)]
        List<VehiclePreset> editorStarterParty = new();

        [SerializeField]
        [TabGroup("Editor Starter/Tab", "로스터")]
        [HideReferenceObjectPicker]
        [LabelText("로스터")]
        [ListDrawerSettings(ShowFoldout = false)]
        List<VehiclePreset> editorStarterRoster = new();

        [SerializeField]
        [TabGroup("Editor Starter/Tab", "인벤토리(부품)")]
        [HideReferenceObjectPicker]
        [LabelText("부품")]
        [ListDrawerSettings(ShowFoldout = false)]
        List<MechPartStack> editorStarterMechParts = new();

        [OnInspectorInit]
        void OnInspectorInit()
        {
            InitializeMemberSlots(starterParty);
            InitializeMemberSlots(editorStarterParty);
        }

        void InitializeMemberSlots(List<VehiclePreset> members)
        {
            if (members.Count < 4)
            {
                int addCount = 4 - members.Count;
                for (int i = 0; i < addCount; ++i)
                {
                    members.Add(new VehiclePreset());
                }
            }
        }

        [TabGroup("Editor Starter/Tab", "파티")]
        [Button("파티 초기화", Stretch = false, ButtonHeight = 30)]
        void ResetEditorMembers()
        {
            editorStarterParty = new();
            InitializeMemberSlots(editorStarterParty);
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

        public List<VehiclePreset> GetStarterParty()
        {
#if UNITY_EDITOR
            return editorStarterParty;
#else
            return starterParty;
#endif
        }

        public List<VehiclePreset> GetStarterMechParts()
        {
#if UNITY_EDITOR
            return editorStarterRoster;
#else
            return starterPartyNotInParty;
#endif
        }

        public List<MechPartStack> GetStarterRoster()
        {
#if UNITY_EDITOR
            return editorStarterMechParts;
#else
            return starterEquipmentsNotOwned;
#endif
        }
    }
}