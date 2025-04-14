using Coffee.UIEffects;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Mathlife.ProjectL.Gameplay.UI
{
    public class MailScrollRectCell
        : SimpleScrollRectCell<Mail, MailScrollRectContext>
    {
        [SerializeField]
        private TextMeshProUGUI titleText;
        
        [SerializeField]
        private TextMeshProUGUI contentText;
        
        [SerializeField]
        private UIEffect rewardUIEffect;
        
        [SerializeField]
        private Image rewardIconImage;
        
        [SerializeField]
        private TextMeshProUGUI rewardAmountText;
        
        [SerializeField]
        private Button recvButton;
        
        [SerializeField]
        private Sprite goldIcon;
        
        [SerializeField]
        private Sprite diamondIcon;

        public override void Initialize()
        {
            base.Initialize();
            recvButton.onClick.AddListener(OnClickRecvButton);
        }

        private void OnClickRecvButton()
        {
            GameState.Inst.gameProgressState.ReceiveMailReward(Index);
            GameState.Inst.gameProgressState.Save();
            Context.updateContents?.Invoke();
        }

        public override void UpdateContent(Mail itemData)
        {
            titleText.text = itemData.title;
            contentText.text = itemData.content;

            if (itemData.reward.gold > 0)
            {
                rewardUIEffect.LoadPreset("GradientN");
                rewardIconImage.sprite = goldIcon;
                rewardAmountText.text = itemData.reward.gold.ToString();   
            }
            else if (itemData.reward.diamond > 0)
            {
                rewardUIEffect.LoadPreset("GradientR");
                rewardIconImage.sprite = diamondIcon;
                rewardAmountText.text = itemData.reward.diamond.ToString();   
            }
            else
            {
                var reward = itemData.reward;
                rewardUIEffect.LoadPreset(reward.itemGameData.rarity.ToGradientPresetName());
                rewardIconImage.sprite = reward.itemGameData.icon;
                rewardAmountText.text = reward.itemGameData is MechPartGameData ? "" : reward.itemAmount.ToString();
            }
        }
    }
}