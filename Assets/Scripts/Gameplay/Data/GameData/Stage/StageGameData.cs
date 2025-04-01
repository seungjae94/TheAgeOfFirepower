using System;
using System.Collections.Generic;
using Mathlife.ProjectL.Utils;
using Sirenix.OdinInspector;
using UnityEngine;

#if UNITY_EDITOR
using Sirenix.OdinInspector.Editor;
#endif

namespace Mathlife.ProjectL.Gameplay
{
    [Serializable]
    public class Enemy
    {
#if UNITY_EDITOR
        [HorizontalGroup("Enemy", LabelWidth = 50, Width = 100)]
        [LabelText("적")]
        [PreviewField(ObjectFieldAlignment.Left, PreviewGetter = nameof(GetPreview))]
#endif
        public ArtyGameData artyGameData;

        [HorizontalGroup("Enemy", Gap = 50, Width = 100)]
        [LabelText("레벨")]
        public int level;

#if UNITY_EDITOR
        private Sprite GetPreview()
        {
            return artyGameData?.enemySprite;
        }
#endif
    }

    [Serializable]
    public class Reward
    {
        [HorizontalGroup("Gold", LabelWidth = 50, Width = 150)]
        [LabelText("골드")]
        [LabelWidth(50)]
        public int gold;

        [HorizontalGroup("Diamond", LabelWidth = 50, Width = 150)]
        [LabelText("다이아")]
        [LabelWidth(50)]
        public int diamond;

#if UNITY_EDITOR
        [HorizontalGroup("Item", LabelWidth = 50, Width = 100)]
        [LabelText("아이템")]
        [PreviewField(ObjectFieldAlignment.Left, PreviewGetter = nameof(GetPreview))]
#endif
        public ItemGameData itemGameData;

        [HorizontalGroup("Item", Gap = 50, Width = 100)]
        [LabelText("개수")]
        public int itemAmount;

#if UNITY_EDITOR
        private Sprite GetPreview()
        {
            return itemGameData?.icon;
        }
#endif
    }

    public class StageGameData : GameData
    {
        [TitleGroup("스테이지 데이터", GroupID = "Stage", HorizontalLine = true)]
        [LabelText("번호")]
        [HorizontalGroup("Stage/No", Width = 200, LabelWidth = 100)]
        public int worldNo;

        [HideLabel]
        [HorizontalGroup("Stage/No", Width = 100, Gap = 25)]
        public int stageNo;

        [LabelWidth(100)]
        [PreviewField(ObjectFieldAlignment.Left, Height = 400)]
        public Sprite mapSprite;
        
#if UNITY_EDITOR
        [SpaceOnly(25)]
        private bool _dummy0;
#endif

        [HideReferenceObjectPicker]
        [LabelText("적 목록")]
        [ListDrawerSettings(ShowFoldout = false)]
        [PropertySpace(SpaceBefore = 25)]
        public List<Enemy> enemyList = new();
        
#if UNITY_EDITOR
        [SpaceOnly(25)]
        private bool _dummy1;
#endif

        [HideReferenceObjectPicker]
        [LabelText("보상 목록")]
        [ListDrawerSettings(ShowFoldout = false)]
        [PropertySpace(SpaceBefore = 25, SpaceAfter = 25)]
        public List<Reward> rewardList = new();

#if UNITY_EDITOR
        public override void SetMenuItem(ref OdinMenuItem menuItem)
        {
            menuItem.Name = displayName;
        }
#endif
    }
}