using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEngine;

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

        [LabelWidth(100)]
        [LabelText("폭발 여부")]
        public bool shouldExplode = true;

        [LabelWidth(100)]
        [LabelText("폭발 반경")]
        [Range(0f, 200f)]
        [ShowIf("@shouldExplode==true")]
        public float explosionRadius = 0f;

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