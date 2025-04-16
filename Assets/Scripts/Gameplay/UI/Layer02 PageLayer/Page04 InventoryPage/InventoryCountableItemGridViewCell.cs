using System;
using Coffee.UIEffects;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Mathlife.ProjectL.Gameplay.UI
{
    public class InventoryCountableItemGridViewCell
        : SimpleGridViewCell<ItemStackModel, SimpleGridViewContext>
    {
        // View
        [SerializeField]
        private GameObject selectionGameObject;
        
        [SerializeField]
        private UIEffect uiEffect;
        
        [SerializeField]
        private Image iconImage;

        [SerializeField]
        private TextMeshProUGUI amountText;
        
        // Field
        private ItemStackModel itemStack;
        
        public override void UpdateContent(ItemStackModel itemData)
        {
            itemStack = itemData;
            
            if (itemStack == null)
            {
                throw new ArgumentNullException("[InventoryCountableItemGridViewCell] null 아이템 스택이 인자로 들어왔습니다.");
            }
            
            uiEffect.LoadPreset(itemStack.Rarity.ToGradientPresetName());
            iconImage.sprite = itemStack.Icon;
            selectionGameObject.SetActive(Context.selectedIndex == Index);
            amountText.text = itemStack.Amount.ToString();
        }
        
        protected override void OnClick(Unit _)
        {
            base.OnClick(_);
            AudioManager.Inst.PlaySE(ESoundEffectId.Ok);
        }
    }
}