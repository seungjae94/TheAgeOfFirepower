using System;
using UnityEngine;
using UnityEngine.Serialization;

#if UNITY_EDITOR
using Sirenix.OdinInspector;
using Sirenix.Utilities.Editor;
using ObjectFieldAlignment = Sirenix.OdinInspector.ObjectFieldAlignment;
#endif

namespace Mathlife.ProjectL.Gameplay
{
    [Serializable]
    public class MechPartStack
    {
#if UNITY_EDITOR
        [FormerlySerializedAs("equipment")]
        [LabelText("부품")]
        [LabelWidth(125)]
        [PreviewField(Alignment = ObjectFieldAlignment.Left, PreviewGetter = nameof(GetEquipmentPreview))]
        [HorizontalGroup(group: "Row")]
#endif
        public MechPartGameData mechPart;

#if UNITY_EDITOR
        [LabelText("개수")]
        [LabelWidth(125)]
        [HorizontalGroup(group: "Row")]
#endif
        public int count;

#if UNITY_EDITOR
        MechPartGameData cachedMechPart;
        Texture2D cachedTexture;

        [OnInspectorInit]
        public void OnInspectorInit()
        {
            cachedMechPart = null; 
            cachedTexture = null;
        }

        public Texture2D GetEquipmentPreview()
        {
            if (cachedMechPart == mechPart)
            {
                return cachedTexture;
            }

            cachedMechPart = mechPart;

            if (mechPart == null)
            {
                cachedTexture = null;
            }
            else
            {
                Rect texRect = mechPart.icon.textureRect;
                cachedTexture = mechPart.icon.texture.CropTexture(texRect);
            }
            return cachedTexture;
        }
#endif
    }
}
