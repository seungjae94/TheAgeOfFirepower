using Coffee.UIEffects;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Mathlife.ProjectL.Gameplay.UI
{
    public class BattleRewardScrollRectCell
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
        private Sprite expIcon;
        
        [SerializeField]
        private Sprite diamondIcon;
        
        public override void UpdateContent(Reward reward)
        {
            if (reward.gold > 0)
                DrawGold(reward.gold);
            else if (reward.diamond > 0)
                DrawDiamond(reward.diamond);
            else
                DrawItem(reward);
        }

        private void DrawExp(long exp)
        {
            uiEffect.LoadPreset("GradientN");
            iconImage.sprite = expIcon;
            amountText.text = exp.ToString();
        }

        private void DrawGold(int gold)
        {
            uiEffect.LoadPreset("GradientN");
            iconImage.sprite = goldIcon;
            amountText.text = gold.ToString();
        }
        
        private void DrawDiamond(int diamond)
        {
            uiEffect.LoadPreset("GradientR");
            iconImage.sprite = diamondIcon;
            amountText.text = diamond.ToString();
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