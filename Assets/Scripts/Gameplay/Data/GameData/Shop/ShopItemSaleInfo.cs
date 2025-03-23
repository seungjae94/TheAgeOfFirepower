using System;
using Mathlife.ProjectL.Utils;
using UnityEngine;

#if UNITY_EDITOR
using Sirenix.OdinInspector;
using Sirenix.Utilities.Editor;
using ObjectFieldAlignment = Sirenix.OdinInspector.ObjectFieldAlignment;
#endif

namespace Mathlife.ProjectL.Gameplay
{
    [Serializable]
    public class ShopItemSaleInfo
    {
#if UNITY_EDITOR
        [LabelText("@GetDisplayName()")]
        [PreviewField(Alignment = ObjectFieldAlignment.Left, PreviewGetter = nameof(GetItemPreview))]
        [HorizontalGroup(group: "Row", Width = 150, LabelWidth = 100)]
#endif
        public ItemGameData item;

#if UNITY_EDITOR
        [ShowInInspector]
        [SpaceOnly(0)]
        [HorizontalGroup(group: "Row", Width = 125, LabelWidth = 50)]
        private bool _dummy0;
        
        [LabelText("가격")]
        [HorizontalGroup(group: "Row", Width = 125, LabelWidth = 50)]
#endif
        public int price;
        
#if UNITY_EDITOR
        [ShowInInspector]
        [SpaceOnly(0)]
        [HorizontalGroup(group: "Row", Width = 125, LabelWidth = 50)]
        private bool _dummy1;
        
        [LabelText("개수")]
        [HorizontalGroup(group: "Row", Width = 125, LabelWidth = 50)]
#endif
        public int amount;

#if UNITY_EDITOR
        public Sprite GetItemPreview()
        {
            return item?.icon;
        }

        public string GetDisplayName()
        {
            return item != null ? item.displayName : string.Empty;
        }
#endif
    }
}