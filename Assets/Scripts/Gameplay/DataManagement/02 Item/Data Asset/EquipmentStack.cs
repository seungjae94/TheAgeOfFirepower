using System;
using UnityEngine;

#if UNITY_EDITOR
using Sirenix.OdinInspector;
using Sirenix.Utilities.Editor;
using ObjectFieldAlignment = Sirenix.OdinInspector.ObjectFieldAlignment;
#endif

namespace Mathlife.ProjectL.Gameplay
{
    [Serializable]
    public class EquipmentStack
    {
#if UNITY_EDITOR
        [LabelText("아티팩트")]
        [LabelWidth(125)]
        [PreviewField(Alignment = ObjectFieldAlignment.Left, PreviewGetter = nameof(GetEquipmentPreview))]
        [HorizontalGroup(group: "Row")]
#endif
        public EquipmentGameData equipment;

#if UNITY_EDITOR
        [LabelText("개수")]
        [LabelWidth(125)]
        [HorizontalGroup(group: "Row")]
#endif
        public int count;

#if UNITY_EDITOR
        EquipmentGameData cachedEquipment;
        Texture2D cachedTexture;

        [OnInspectorInit]
        public void OnInspectorInit()
        {
            cachedEquipment = null; 
            cachedTexture = null;
        }

        public Texture2D GetEquipmentPreview()
        {
            if (cachedEquipment == equipment)
            {
                return cachedTexture;
            }

            cachedEquipment = equipment;

            if (equipment == null)
            {
                cachedTexture = null;
            }
            else
            {
                Rect texRect = equipment.icon.textureRect;
                cachedTexture = equipment.icon.texture.CropTexture(texRect);
            }
            return cachedTexture;
        }
#endif
    }
}
