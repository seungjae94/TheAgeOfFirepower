using Sirenix.OdinInspector;
using Sirenix.Utilities;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


#if UNITY_EDITOR
#endif

namespace Mathlife.ProjectL.Gameplay
{
    [CreateAssetMenu(fileName = "Exp Data Asset", menuName = "Data Asset/Exp Data")]
    public class ExpDataAsset : SerializedScriptableObject
    {

#if UNITY_EDITOR
        [TitleGroup("경험치 테이블", GroupID = "Exp")]
        [Delayed]
        [TabGroup("Exp/tab", "캐릭터 필요 경험치")] // 그룹 이름, 탭 이름
        [HorizontalGroup("Exp/tab/캐릭터 필요 경험치/hortest")]
        [LabelText("다음 레벨까지 필요 경험치")]
        [ListDrawerSettings(DraggableItems = false, ShowFoldout = false, ShowIndexLabels = true)]
        [OnValueChanged(nameof(OnCharacterNeedExpListChanged), includeChildren: true)]
#endif
        public List<long> characterNeedExpAtLevelList = new();

#if UNITY_EDITOR
        [ReadOnly]
        [TabGroup("Exp/tab", "캐릭터 필요 경험치")]
        [LabelText("이 레벨까지 누적 경험치")]
        [HorizontalGroup("Exp/tab/캐릭터 필요 경험치/hortest")]
        [ListDrawerSettings(DraggableItems = false, ShowFoldout = false, ShowIndexLabels = true)]
#endif
        public List<long> characterTotalExpAtLevelList = new();

#if UNITY_EDITOR
        [Delayed]
        [TabGroup("Exp/tab", "몬스터 기준 경험치")]
        [LabelText("몬스터 기준 경험치")]
        [ListDrawerSettings(DraggableItems = false, ShowFoldout = false, ShowIndexLabels = true)]
#endif
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