using System;
using Sirenix.OdinInspector;
using UnityEngine;

#if UNITY_EDITOR
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
#endif

namespace Mathlife.ProjectL.Gameplay
{
    public enum EItemRarity
    {
        N,
        R,
        SR,
        SSR
    }
    
    public static class EItemRarityExtensions
    {
        public static string ToGradientPresetName(this EItemRarity rarity)
        {
            return rarity switch
            {
                EItemRarity.N => "GradientN",
                EItemRarity.R => "GradientR",
                EItemRarity.SR => "GradientSR",
                EItemRarity.SSR => "GradientSSR",
                _ => throw new ArgumentOutOfRangeException(nameof(rarity), rarity, null)
            };
        }
    }
    
    public enum EItemType
    {
        MechPart,
        ConsumableItem,
        BattleItem
    }

    public abstract class ItemGameData : GameData
    {
        public abstract EItemType ItemType { get; }

        [Title("아이템 데이터")]
        [LabelWidth(100)]
        [LabelText("희귀도")]
        public EItemRarity rarity;

        [LabelWidth(100)]
        [LabelText("아이콘")]
        [AssetSelector(Paths = "Assets/Arts/UI/Icons", FlattenTreeView = true)]
#if UNITY_EDITOR
        [PreviewField(50, Sirenix.OdinInspector.ObjectFieldAlignment.Left)]
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

        public Texture2D GetPreview()
        {
            if (cachedTexture != null)
                return cachedTexture;
            
            if (icon != null)
                CacheIconTexture();
            
            return cachedTexture;
        }
        
        private void CacheIconTexture()
        {
            if (icon == null) 
                return;
            
            cachedTexture = TextureUtilities.ConvertSpriteToTexture(icon);
        }

        public override void SetMenuItem(ref OdinMenuItem menuItem)
        {
            menuItem.Name = displayName;
            menuItem.Icon = GetPreview();
        }
#endif
    }
}