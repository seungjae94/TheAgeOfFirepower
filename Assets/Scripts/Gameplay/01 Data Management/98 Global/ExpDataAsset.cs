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
        [TitleGroup("����ġ ���̺�", GroupID = "Exp")]
        [Delayed]
        [TabGroup("Exp/tab", "ĳ���� �ʿ� ����ġ")] // �׷� �̸�, �� �̸�
        [HorizontalGroup("Exp/tab/ĳ���� �ʿ� ����ġ/hortest")]
        [LabelText("���� �������� �ʿ� ����ġ")]
        [ListDrawerSettings(DraggableItems = false, ShowFoldout = false, ShowIndexLabels = true)]
        [OnValueChanged(nameof(OnCharacterNeedExpListChanged), includeChildren: true)]
#endif
        public List<long> characterNeedExpAtLevelList = new();

#if UNITY_EDITOR
        [ReadOnly]
        [TabGroup("Exp/tab", "ĳ���� �ʿ� ����ġ")]
        [LabelText("�� �������� ���� ����ġ")]
        [HorizontalGroup("Exp/tab/ĳ���� �ʿ� ����ġ/hortest")]
        [ListDrawerSettings(DraggableItems = false, ShowFoldout = false, ShowIndexLabels = true)]
#endif
        public List<long> characterTotalExpAtLevelList = new();

#if UNITY_EDITOR
        [Delayed]
        [TabGroup("Exp/tab", "���� ���� ����ġ")]
        [LabelText("���� ���� ����ġ")]
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