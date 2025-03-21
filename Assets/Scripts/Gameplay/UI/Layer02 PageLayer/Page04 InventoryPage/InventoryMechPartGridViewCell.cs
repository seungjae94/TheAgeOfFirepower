using System;
using Coffee.UIEffects;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

namespace Mathlife.ProjectL.Gameplay.UI
{
    public class InventoryMechPartGridViewCell 
        : SimpleGridViewCell<MechPartModel, SimpleGridViewContext>
    {
        // View
        [SerializeField]
        private GameObject selectionGameObject;
        
        [SerializeField]
        private GameObject equippedMarkGameObject;
        
        [SerializeField]
        private UIEffect uiEffect;
        
        [SerializeField]
        private Image iconImage;

        // Field
        private MechPartModel mechPart;
        
        public override void UpdateContent(MechPartModel itemData)
        {
            mechPart = itemData;
            
            if (mechPart == null)
            {
                throw new ArgumentNullException("[InventoryMechPartGridViewCell] null 부품 데이터가 인자로 들어왔습니다.");
            }
            
            uiEffect.LoadPreset(mechPart.Rarity.ToGradientPresetName());
            iconImage.sprite = mechPart.Icon;

            if (Context.selectedIndex == Index)
            {
                selectionGameObject.SetActive(true);
                equippedMarkGameObject.SetActive(false);
            }
            else
            {
                selectionGameObject.SetActive(false);
                equippedMarkGameObject.SetActive(mechPart.Owner.Value != null);
            }
        }
    }
}