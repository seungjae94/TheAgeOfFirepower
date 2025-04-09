using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Mathlife.ProjectL.Gameplay.UI
{
    public class BattleResultArtyView : AbstractView
    {
        [SerializeField]
        private TextMeshProUGUI nameText;
        
        [SerializeField]
        private Image portraitImage;
        
        [SerializeField]
        private TextMeshProUGUI levelText;
        
        // Field
        private ArtyModel arty;
        
        public void Setup(ArtyModel arty)
        {
            if (arty == null)
            {
                levelText.gameObject.SetActive(false);
                return;
            }

            this.arty = arty;
        }
        
        public override void Draw()
        {
            base.Draw();

            if (arty == null)
            {
                nameText.gameObject.SetActive(false);
                portraitImage.gameObject.SetActive(false);
                return;
            }
            
            nameText.text = arty.DisplayName;
            portraitImage.sprite = arty.Sprite;
            levelText.text = $"Lv. {arty.levelRx.Value}";
        }
    }
}