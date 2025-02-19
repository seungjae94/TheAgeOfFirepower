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
        [TitleGroup("실제 게임 세팅", GroupID = "Starter", Indent = true, HorizontalLine = true)]
        [LabelText("시작 골드")]
        [PropertySpace(spaceBefore: 0, spaceAfter: 35)]
        long starterGold = 0L;

        [SerializeField]
        [TabGroup("Starter/Tab", "파티")]
        [HideReferenceObjectPicker]
        [LabelText("파티")]
        [ListDrawerSettings(HideAddButton = true, HideRemoveButton = true, ShowFoldout = false)]
        List<CharacterState> starterParty = new ();

        [SerializeField]
        [TabGroup("Starter/Tab", "보유 캐릭터 (파티 X)")]
        [HideReferenceObjectPicker]
        [LabelText("보유 캐릭터")]
        [ListDrawerSettings(ShowFoldout = false)]
        List<CharacterState> starterCharactersNotInParty = new();

        [SerializeField]
        [Space(10)]
        [TabGroup("Starter/Tab", "보유 장비 (장착 X)")]
        [HideReferenceObjectPicker]
        [LabelText("보유 장비")]
        [ListDrawerSettings(ShowFoldout = false)]
        List<EquipmentStack> starterEquipmentsNotOwned = new();

        [ShowInInspector]
        [SpaceOnly(50)]
        bool _dummySpacing;

#if UNITY_EDITOR
        [SerializeField]
        [TitleGroup("에디터 게임 세팅", GroupID = "Editor Starter", Indent = true, HorizontalLine = true)]
        [LabelText("에디터 시작 골드")]
        [PropertySpace(spaceBefore: 0, spaceAfter: 35)]
        long editorStarterGold = 0L;

        [SerializeField]
        [TabGroup("Editor Starter/Tab", "파티")]
        [HideReferenceObjectPicker]
        [LabelText("파티")]
        [ListDrawerSettings(HideAddButton = true, HideRemoveButton = true, ShowFoldout = false)]
        List<CharacterState> editorStarterParty = new();

        [SerializeField]
        [TabGroup("Editor Starter/Tab", "보유 캐릭터 (파티 X)")]
        [HideReferenceObjectPicker]
        [LabelText("보유 캐릭터")]
        [ListDrawerSettings(ShowFoldout = false)]
        List<CharacterState> editorStarterCharactersNotInParty = new();

        [SerializeField]
        [TabGroup("Editor Starter/Tab", "보유 장비 (장착 X)")]
        [HideReferenceObjectPicker]
        [LabelText("보유 장비")]
        [ListDrawerSettings(ShowFoldout = false)]
        List<EquipmentStack> editorStarterEquipmentsNotOwned = new();

        [OnInspectorInit]
        void OnInspectorInit()
        {
            InitializeMemberSlots(starterParty);
            InitializeMemberSlots(editorStarterParty);
        }

        void InitializeMemberSlots(List<CharacterState> members)
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

        [TabGroup("Editor Starter/Tab", "파티")]
        [Button("에디터 멤버 리셋", Stretch = false, ButtonHeight = 30)]
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

        public List<CharacterState> GetStarterParty()
        {
#if UNITY_EDITOR
            return editorStarterParty;
#else
            return starterParty;
#endif
        }

        public List<CharacterState> GetStarterCharactersNotInParty()
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