using Sirenix.OdinInspector;
using UnityEngine;

#if UNITY_EDITOR
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
#endif

namespace Mathlife.ProjectL.Gameplay
{
    public enum EItemType
    {
        MechPart,
        Consumable,
        CombatKit
    }

    public abstract class ItemGameData : GameData
    {
        public abstract EItemType ItemType { get; }

        [LabelWidth(75)]
        [LabelText("아이콘")]
        [PreviewField(50, Sirenix.OdinInspector.ObjectFieldAlignment.Left)]
        [AssetSelector(Paths = "Assets/Arts/UI/Icons", FlattenTreeView = true)]
#if UNITY_EDITOR
        [OnValueChanged(nameof(CacheIconTexture))]
#endif
        public Sprite icon = null;

        [LabelWidth(100)]
        [LabelText("설명")]
        [Multiline(10)]
        public string description = "";

        [LabelWidth(100)]
        [LabelText("상점 판매가")]
        [Min(0)]
        public int shopPrice = 0;
        
#if UNITY_EDITOR
        // 게임 디자이너 에디터 프리뷰 최적화
        private Texture2D cachedTexture;

        private void CacheIconTexture()
        {
            if (icon == null) 
                return;
            
            Rect texRect = icon.textureRect;
            cachedTexture = icon.texture.CropTexture(texRect);
        }

        public override void SetMenuItem(ref OdinMenuItem menuItem)
        {
            menuItem.Name = displayName;
            
            if (cachedTexture != null)
            {
                menuItem.Icon = cachedTexture;
            }
        }
#endif
    }
}