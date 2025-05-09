using System;
using System.Collections.Generic;
using Mathlife.ProjectL.Gameplay.Play;
using Mathlife.ProjectL.Utils;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Serialization;

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

        [HorizontalGroup("Enemy", Gap = 25, Width = 100)]
        [LabelText("레벨")]
        public int level;

        [HorizontalGroup("Strategy", Gap = 25, Width = 225)]
        [LabelText("이동 전략")]
        [LabelWidth(75)]
        public MoveStrategy moveStrategy;

        [HorizontalGroup("Strategy", Gap = 25, Width = 225)]
        [LabelText("타겟팅 전략")]
        [LabelWidth(75)]
        public AttackTargetingStrategy targetingStrategy;

#if UNITY_EDITOR
        private Sprite GetPreview()
        {
            return artyGameData?.enemySprite;
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
        [LabelText("BGM")]
        public AudioClip bgm;

        [LabelWidth(100)]
        [PreviewField(ObjectFieldAlignment.Left, Height = 400)]
        public AssetReferenceSprite mapSprite;

        [FormerlySerializedAs("spawnXs")]
        [LabelWidth(100)]
        [LabelText("스폰 위치")]
        [ListDrawerSettings(ShowFoldout = false)]
        public List<Vector2> spawnPositions = new();

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