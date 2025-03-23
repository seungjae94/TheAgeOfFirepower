using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
#endif

namespace Mathlife.ProjectL.Gameplay
{
    public class ShopGameData : SerializedScriptableObject
    {
        [TabGroup("Tab", "화포")]
        [LabelText("화포")]
        [HideReferenceObjectPicker]
        [ListDrawerSettings(ShowFoldout = false)]
        public List<ShopArtySaleInfo> shopArtyList = new();

        [TabGroup("Tab", "포신")]
        [LabelText("포신")]
        [HideReferenceObjectPicker]
        [ListDrawerSettings(ShowFoldout = false)]
        public List<ShopItemSaleInfo> shopBarrels = new();

        [TabGroup("Tab", "장갑")]
        [LabelText("장갑")]
        [HideReferenceObjectPicker]
        [ListDrawerSettings(ShowFoldout = false)]
        public List<ShopItemSaleInfo> shopArmors = new();

        [TabGroup("Tab", "엔진")]
        [LabelText("엔진")]
        [HideReferenceObjectPicker]
        [ListDrawerSettings(ShowFoldout = false)]
        public List<ShopItemSaleInfo> shopEngines = new();

        [TabGroup("Tab", "재료")]
        [LabelText("재료")]
        [HideReferenceObjectPicker]
        [InlineProperty]
        [ListDrawerSettings(ShowFoldout = false)]
        public List<ShopItemSaleInfo> shopMaterialItems = new();

        [TabGroup("Tab", "전투")]
        [LabelText("전투")]
        [HideReferenceObjectPicker]
        [ListDrawerSettings(ShowFoldout = false)]
        public List<ShopItemSaleInfo> shopBattleItems = new();
    }
}