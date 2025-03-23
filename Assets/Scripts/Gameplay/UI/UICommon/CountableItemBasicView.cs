using Coffee.UIEffects;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Mathlife.ProjectL.Gameplay.UI
{
    public class CountableItemBasicView : AbstractView
    {
        // View
        [SerializeField]
        private UIEffect uiEffect;
        
        [SerializeField]
        private Image iconImage;
        
        [SerializeField]
        private TextMeshProUGUI nameText;
        
        [SerializeField]
        private TextMeshProUGUI descriptionText;

        // Field
        private ItemStackModel itemStack;
        private string emptyWarningMessage;
        
        public void Setup(ItemStackModel pItemStack, string pEmptyWarningMessage = "아이템을 선택하지 않았습니다.")
        {
            itemStack = pItemStack;
            emptyWarningMessage = pEmptyWarningMessage;
        }
        
        // Draw / Clear
        public override void Draw()
        {
            base.Draw();
            
            if (itemStack == null)
                DrawEmpty();
            else
                DrawNonEmpty();
        }

        private void DrawEmpty()
        {
            uiEffect.gameObject.SetActive(false);
            iconImage.enabled = false;
            iconImage.sprite = null;
            nameText.text = "";
            descriptionText.text = $"<style=\"WarningPrimaryColor\">{emptyWarningMessage}</style>";
        }
        
        private void DrawNonEmpty()
        {
            uiEffect.gameObject.SetActive(true);
            uiEffect.LoadPreset(itemStack.Rarity.ToGradientPresetName());
            
            iconImage.enabled = true;
            iconImage.sprite = itemStack.Icon;
            nameText.text = itemStack.DisplayName;
            descriptionText.text = itemStack.Description;
        }
    }
}