using System;
using UnityEngine;
using System.Collections.Generic;
using Sirenix.OdinInspector.Editor;
using UnityEditor.Experimental;
using UnityEngine.Serialization;


#if UNITY_EDITOR
using Sirenix.Utilities.Editor;
using Sirenix.OdinInspector;
using ObjectFieldAlignment = Sirenix.OdinInspector.ObjectFieldAlignment;
#endif

namespace Mathlife.ProjectL.Gameplay
{
    [Serializable]
    public class VehiclePreset
    {
        [FormerlySerializedAs("character")]
        [LabelText("캐릭터")]
        [LabelWidth(125)]
#if UNITY_EDITOR
        [PreviewField(Alignment = ObjectFieldAlignment.Left, PreviewGetter = nameof(GetCharacterPreview))]
#endif
        public VehicleGameData vehicle;

        [LabelText("레벨")]
        [LabelWidth(125)]
        [HorizontalGroup(group: "Level and Exp")]
        public int level;

        [LabelText("경험치")]
        [LabelWidth(125)]
        [HorizontalGroup(group: "Level and Exp")]
        public long currentLevelExp;

        [LabelText("무기")]
        [LabelWidth(125)]
        [HorizontalGroup(group: "Equipments")]
        [ValidateInput(nameof(IsWeapon), "무기가 아닙니다.")]
#if UNITY_EDITOR
        [PreviewField(Alignment = ObjectFieldAlignment.Left, PreviewGetter = nameof(GetWeaponPreview))]
#endif
        public MechPartGameData weapon;

        [LabelText("방어구")]
        [LabelWidth(125)]
        [HorizontalGroup(group: "Equipments")]
        [ValidateInput(nameof(IsArmor), "방어구가 아닙니다.")]
#if UNITY_EDITOR
        [PreviewField(Alignment = ObjectFieldAlignment.Left, PreviewGetter = nameof(GetArmorPreview))]
#endif
        public MechPartGameData armor;

        [LabelText("아티팩트")]
        [LabelWidth(125)]
        [HorizontalGroup(group: "Equipments")]
        [ValidateInput(nameof(IsArtifact), "아티팩트가 아닙니다.")]
#if UNITY_EDITOR
        [PreviewField(Alignment = ObjectFieldAlignment.Left, PreviewGetter = nameof(GetArtifactPreview))]
#endif
        public MechPartGameData artifact;

#if UNITY_EDITOR
        MechPartGameData[] cachedArtifacts = new MechPartGameData[4]; 
        Texture2D[] cachedTextures = new Texture2D[4];

        [OnInspectorInit]
        public void OnInspectorInit()
        {
            cachedArtifacts = new MechPartGameData[4];
            cachedTextures = new Texture2D[4];
        }

        public Texture2D GetCharacterPreview()
        {
            if (vehicle != null)
                return vehicle.sprite.texture;

            return null;
        }

        public Texture2D GetWeaponPreview(MechPartGameData artifact)
        {
            return GetEquipmentPreview(artifact, 0);
        }

        public Texture2D GetArmorPreview(MechPartGameData artifact)
        {
            return GetEquipmentPreview(artifact, 1);
        }

        public Texture2D GetArtifactPreview(MechPartGameData artifact)
        {
            return GetEquipmentPreview(artifact, 2);
        }

        public Texture2D GetEquipmentPreview(MechPartGameData artifact, int index)
        {
            if (cachedArtifacts[index] == artifact)
            {
                return cachedTextures[index];
            }

            cachedArtifacts[index] = artifact;

            if (artifact == null)
            {
                cachedTextures[index] = null;
            }
            else
            {
                Rect texRect = artifact.icon.textureRect;
                cachedTextures[index] = artifact.icon.texture.CropTexture(texRect);
            }
            return cachedTextures[index];
        }

        public bool IsWeapon(MechPartGameData mechPart)
        {
            return mechPart == null || mechPart.type == EEquipmentType.Barrel;
        }

        public bool IsArmor(MechPartGameData mechPart)
        {
            return mechPart == null || mechPart.type == EEquipmentType.Armor;
        }

        public bool IsArtifact(MechPartGameData mechPart)
        {
            return mechPart == null || mechPart.type == EEquipmentType.Engine;
        }
#endif
    }
}
