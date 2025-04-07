using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEngine;
using UnityEngine.Serialization;

namespace Mathlife.ProjectL.Gameplay
{
    public class ShellGameData : GameData
    {
        [LabelWidth(100)]
        [LabelText("아이콘")]
        [PreviewField(50, ObjectFieldAlignment.Left)]
        public Sprite icon;
        
        [LabelWidth(100)]
        [LabelText("설명")]
        [Multiline(5)]
        public string description;
        
        [LabelWidth(100)]
        [LabelText("데미지")]
        public int damage;

        [FormerlySerializedAs("shouldExplode")]
        [LabelWidth(100)]
        [LabelText("지형 파괴 여부")]
        public bool shouldDestructTerrain = true;

        [LabelWidth(100)]
        [LabelText("폭발 반경")]
        [Range(0f, 200f)]
        public float explosionRadius = 0f;
        
        [LabelWidth(100)]
        [LabelText("프리팹")]
        public GameObject prefab;
        
#if UNITY_EDITOR
        public override void SetMenuItem(ref OdinMenuItem menuItem)
        {
            menuItem.Name = displayName;
            
            if (icon != null)
                menuItem.Icon = icon.texture;
        }
#endif
    }
}