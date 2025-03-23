using System;
using UnityEngine;

#if UNITY_EDITOR
using Sirenix.OdinInspector;
using Sirenix.Utilities.Editor;
using ObjectFieldAlignment = Sirenix.OdinInspector.ObjectFieldAlignment;
#endif

namespace Mathlife.ProjectL.Gameplay
{
    [Serializable]
    public class CountableItemStack
    {
#if UNITY_EDITOR
        [LabelText("아이템")]
        [LabelWidth(125)]
        [PreviewField(Alignment = ObjectFieldAlignment.Left, PreviewGetter = nameof(GetPreview))]
        [HorizontalGroup(group: "Row")]
#endif
        public CountableItemGameData item;

#if UNITY_EDITOR
        [LabelText("개수")]
        [LabelWidth(125)]
        [HorizontalGroup(group: "Row")]
#endif
        public int count;

#if UNITY_EDITOR
        public Sprite GetPreview()
        {
            return item?.icon;
        }
#endif
    }
}