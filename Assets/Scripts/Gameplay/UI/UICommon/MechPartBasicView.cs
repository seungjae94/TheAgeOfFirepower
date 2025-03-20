using Coffee.UIEffects;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Mathlife.ProjectL.Gameplay.UI
{
    public class MechPartBasicView : AbstractView
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
        
        [SerializeField]
        private GameObject ownerViewGameObject;
        
        [SerializeField]
        private Image ownerPortraitImage;
        
        [SerializeField]
        private TextMeshProUGUI ownerNameText;
        
        [SerializeField]
        private TextMeshProUGUI ownerLevelText;
        
        // Field
        private MechPartModel mechPart;
        private string emptyWarningMessage;
        
        public void Setup(MechPartModel pMechPart, string pEmptyWarningMessage = "부품을 선택하지 않았습니다.")
        {
            mechPart = pMechPart;
            emptyWarningMessage = pEmptyWarningMessage;
        }
        
        // Draw / Clear
        public override void Draw()
        {
            base.Draw();
            
            if (mechPart == null)
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
            ownerViewGameObject.SetActive(false);
            descriptionText.text = $"<style=\"WarningPrimaryColor\">{emptyWarningMessage}</style>";
        }
        
        private void DrawNonEmpty()
        {
            uiEffect.gameObject.SetActive(true);
            uiEffect.LoadPreset(mechPart.Rarity.ToGradientPresetName());
            
            iconImage.enabled = true;
            iconImage.sprite = mechPart.Icon;
            nameText.text = mechPart.DisplayName;
            
            if (mechPart.Owner.Value != null)
            {
                ownerViewGameObject.SetActive(true);
                ownerPortraitImage.sprite = mechPart.Owner.Value.Sprite;
                ownerNameText.text = mechPart.Owner.Value.DisplayName;
                ownerLevelText.text = mechPart.Owner.Value.levelRx.Value.ToString();
            }
            else
            {
                ownerViewGameObject.SetActive(false);
            }
            descriptionText.text = mechPart.Description;
        }

        public override void Clear()
        {
            base.Clear();
        }
    }
}