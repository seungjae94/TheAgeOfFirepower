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

        [SerializeField]
        private Slider expSlider;
        
        // Field
        private ArtyModel arty;

        private int prevLevel;
        private long prevExpAtLevel;

        private int afterLevel;
        private long afterExpAtLevel;
        
        public void Setup(ArtyModel arty, long expGain)
        {
            if (arty == null)
            {
                levelText.gameObject.SetActive(false);
                expSlider.gameObject.SetActive(false);
                return;
            }
            
            this.arty = arty;
            prevLevel = arty.levelRx.Value;
            prevExpAtLevel = arty.CurrentLevelExp;
            arty.GainExp(expGain);
            afterLevel = arty.levelRx.Value;
            afterExpAtLevel = arty.CurrentLevelExp;
        }
        
        public override void Draw()
        {
            base.Draw();

            if (arty == null)
            {
                nameText.gameObject.SetActive(false);
                portraitImage.gameObject.SetActive(false);
                expSlider.gameObject.SetActive(false);
                return;
            }
                

            // 애니메이션 시작
            //levelText.text = $"Lv. {afterLevel}";
            nameText.text = arty.DisplayName;
            portraitImage.sprite = arty.Sprite;
            levelText.text = $"Lv. {prevLevel} -> {afterLevel}";
            expSlider.value = (float) afterExpAtLevel / arty.NeedExp;
        }
        
        public override void Clear()
        {
            base.Clear();
            // 트윈 제거
        }
    }
}