using Sirenix.OdinInspector;
using UnityEngine;
using System;


#if UNITY_EDITOR
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
#endif

namespace Mathlife.ProjectL.Gameplay
{
    public enum EEquipmentType
    {
        Weapon,
        Armor,
        Artifact
    }

    public enum EEquipmentId
    {
        None = 0,

        // 무기
        LongSword = 1,
        GildedFang,
        Excalibur,
        MagicalOrb,
        AncientGrimoire,
        GrandSagesStaff,

        // 방어구
        LeatherVest = 1000,
        ChainMail,
        GeneralsPlate,

        // 아티팩트
        PackageOfHurbs = 2000,
        Dynamite,
        CursedNecklace,
        LampOfEternity,
    }

    [Serializable]
    public class EquipmentSO : NamedSO
    {
        public override int intId => (int)id;

        [LabelWidth(100)]
        [LabelText("장비 ID")]
        public EEquipmentId id;

        [LabelWidth(100)]
        [LabelText("장비 타입")]
        public EEquipmentType type;

        [LabelWidth(100)]
        [LabelText("설명")]
        [Multiline(10)]
        public string description = "";

        [LabelWidth(75)]
        [LabelText("아이콘")]
        [PreviewField(50, Sirenix.OdinInspector.ObjectFieldAlignment.Left)]
        [AssetSelector(Paths = "Assets/Arts/UI/Icons", FlattenTreeView = true)]
        public Sprite icon = null;

        [LabelWidth(100)]
        [LabelText("상점 판매가")]
        public long shopPrice = 0;

        [LabelWidth(100)]
        [LabelText("스탯")]
        public BasicStat stat;

        [SerializeReference]
        [LabelWidth(100)]
        [LabelText("배틀 효과")]
        [PolymorphicDrawerSettings(ShowBaseType = true)]
        public ConditionalBattleEffect battleEffect;

#if UNITY_EDITOR
        public override void ToMenuItem(ref OdinMenuItem menuItem)
        {
            menuItem.Name = displayName;

            if (icon != null)
            {
                Rect texRect = icon.textureRect;
                menuItem.Icon = icon.texture.CropTexture(texRect);
            }
        }
#endif
    }
}
