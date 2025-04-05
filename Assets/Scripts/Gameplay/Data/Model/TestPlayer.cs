using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Mathlife.ProjectL.Gameplay.Play
{
    [Serializable]
    public class TestPlayer
    {
#if UNITY_EDITOR
        [HorizontalGroup("Enemy", LabelWidth = 50, Width = 100)]
        [LabelText("적")]
        [PreviewField(ObjectFieldAlignment.Left, PreviewGetter = nameof(GetPreview))]
#endif
        public ArtyGameData artyGameData;
        
        [HorizontalGroup("Enemy", Gap = 25, Width = 100)]
        [LabelText("레벨")]
        public int level;
        
#if UNITY_EDITOR
        private Sprite GetPreview()
        {
            return artyGameData?.enemySprite;
        }
#endif
    }
}