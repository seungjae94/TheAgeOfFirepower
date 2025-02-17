using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
#endif

namespace Mathlife.ProjectL.Gameplay
{
    [CreateAssetMenu(fileName = "ShopSO", menuName = "SO/Shop SO")]
    public class ShopSO : SerializedScriptableObject
    {
        [TabGroup("Equipment", "����")]
        [LabelText("����")]
        [ListDrawerSettings(ShowFoldout = false, ListElementLabelName = "displayName")]
#if UNITY_EDITOR
        [PreviewField(PreviewGetter = nameof(GetItemPreview), Alignment = ObjectFieldAlignment.Left)]
#endif
        public List<EquipmentSO> shopWeapons = new();

        [TabGroup("Equipment", "��")]
        [LabelText("��")]
        [ListDrawerSettings(ShowFoldout = false, ListElementLabelName = "displayName")]
#if UNITY_EDITOR
        [PreviewField(PreviewGetter = nameof(GetItemPreview), Alignment = ObjectFieldAlignment.Left)]
#endif
        public List<EquipmentSO> shopArmors = new();

        [TabGroup("Equipment", "��Ƽ��Ʈ")]
        [LabelText("��Ƽ��Ʈ")]
        [ListDrawerSettings(ShowFoldout = false, ListElementLabelName = "displayName")]
#if UNITY_EDITOR
        [PreviewField(PreviewGetter = nameof(GetItemPreview), Alignment = ObjectFieldAlignment.Left)]
#endif
        public List<EquipmentSO> shopArtifacts = new();

#if UNITY_EDITOR
        public Sprite GetItemPreview(EquipmentSO equipment)
        {
            return equipment.icon;
        }
#endif
    }
}