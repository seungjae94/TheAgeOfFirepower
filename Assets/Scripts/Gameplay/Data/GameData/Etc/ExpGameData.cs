using Mathlife.ProjectL.Utils;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;


#if UNITY_EDITOR
#endif

namespace Mathlife.ProjectL.Gameplay
{
    public class ExpGameData : SerializedScriptableObject
    {
#if UNITY_EDITOR
        [ShowInInspector]
        [HorizontalGroup("Curve", Title = "경험치 커브")]
        [LabelWidth(100)]
        [HideReferenceObjectPicker]
        [OnValueChanged(nameof(OnCurveChanged))]
        private AnimationCurve curve = new();
        
        private void OnCurveChanged()
        {
            needExpAtLevelList.Clear();
            for (int i = 0; i <= 99; ++i)
            {
                needExpAtLevelList.Add(Mathf.FloorToInt(curve.Evaluate(i)));
            }
            
            totalExpAtLevelList = needExpAtLevelList
                .Aggregate(new List<long>() { 0L }, (acc, next) =>
                {
                    acc.Add(acc.IsNullOrEmpty() ? 0L : acc.Last() + next);
                    return acc;
                });
        }
        
        [ShowInInspector]
        [SpaceOnly(50)]
        private bool _dummy;
#endif
        
        [FormerlySerializedAs("characterNeedExpAtLevelList")]
        [ReadOnly]
        [HorizontalGroup("Exp", Title = "경험치 테이블")]
        [LabelText("다음 레벨까지 필요 경험치")]
        [ListDrawerSettings(DraggableItems = false, ShowFoldout = false, ShowIndexLabels = true)]
        public List<long> needExpAtLevelList = new();

        [FormerlySerializedAs("characterTotalExpAtLevelList")]
        [ReadOnly]
        [HorizontalGroup("Exp")]
        [LabelText("이 레벨까지 누적 경험치")]
        [ListDrawerSettings(DraggableItems = false, ShowFoldout = false, ShowIndexLabels = true)]
        public List<long> totalExpAtLevelList = new();
    }
}