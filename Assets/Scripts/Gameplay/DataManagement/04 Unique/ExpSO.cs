using Mathlife.ProjectL.Utils;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


#if UNITY_EDITOR
#endif

namespace Mathlife.ProjectL.Gameplay
{
    [CreateAssetMenu(fileName = "ExpSO", menuName = "SO/Exp SO")]
    public class ExpSO : SerializedScriptableObject
    {
        [Delayed]
        [HorizontalGroup("Character Exp", Title = "캐릭터 경험치")]
        [LabelText("다음 레벨까지 필요 경험치")]
        [ListDrawerSettings(DraggableItems = false, ShowFoldout = false, ShowIndexLabels = true)]
#if UNITY_EDITOR
        [OnValueChanged(nameof(OnCharacterNeedExpListChanged), includeChildren: true)]
#endif
        public List<long> characterNeedExpAtLevelList = new();

        [ReadOnly]
        [HorizontalGroup("Character Exp")]
        [LabelText("이 레벨까지 누적 경험치")]
        [ListDrawerSettings(DraggableItems = false, ShowFoldout = false, ShowIndexLabels = true)]
        public List<long> characterTotalExpAtLevelList = new();

        [ShowInInspector]
        [SpaceOnly(25)]
        bool _dummySpacing;

        [Delayed]
        [Title("몬스터 경험치", horizontalLine: false)]
        [LabelText("레벨별 몬스터 기초 경험치")]
        [ListDrawerSettings(DraggableItems = false, ShowFoldout = false, ShowIndexLabels = true)]
        public List<long> monsterBaseExpAtLevelList = new();

#if UNITY_EDITOR
        void OnCharacterNeedExpListChanged()
        {
            characterTotalExpAtLevelList = characterNeedExpAtLevelList
                                .Aggregate(new List<long>() { 0L }, (acc, next) =>
                                {
                                    acc.Add(acc.IsNullOrEmpty() ? 0L : acc.Last() + next);
                                    return acc;
                                });
        }
#endif
    }
}