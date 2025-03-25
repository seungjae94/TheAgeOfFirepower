using Coffee.UIEffects;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Mathlife.ProjectL.Gameplay.UI
{
    public class StageInfoRewardScrollRectCell
    : SimpleScrollRectCell<Reward, SimpleScrollRectContext>
    {
        [SerializeField]
        private UIEffect uiEffect;
        
        [SerializeField]
        private Image iconImage;
        
        [SerializeField]
        private TextMeshProUGUI amountText;
        
        [SerializeField]
        private Sprite goldIcon;
        
        [SerializeField]
        private Sprite diamondIcon;
        
        public override void UpdateContent(Reward itemData)
        {
            if (itemData.gold > 0)
                DrawGold(itemData);
            else if (itemData.diamond > 0)
                DrawDiamond(itemData);
            else
                DrawItem(itemData);
        }

        private void DrawGold(Reward reward)
        {
            uiEffect.LoadPreset("GradientN");
            iconImage.sprite = goldIcon;
            amountText.text = reward.gold.ToString();
        }
        
        private void DrawDiamond(Reward reward)
        {
            uiEffect.LoadPreset("GradientR");
            iconImage.sprite = diamondIcon;
            amountText.text = reward.diamond.ToString();
        }
        
        private void DrawItem(Reward reward)
        {
            uiEffect.LoadPreset(reward.itemGameData.rarity.ToGradientPresetName());
            iconImage.sprite = reward.itemGameData.icon;

            if (reward.itemGameData is MechPartGameData)
            {
                amountText.text = "";
            }
            else
            {
                amountText.text = reward.itemAmount.ToString();
            }
        }
    }
}