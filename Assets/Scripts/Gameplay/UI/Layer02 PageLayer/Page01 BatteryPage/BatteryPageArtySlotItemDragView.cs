using Mathlife.ProjectL.Gameplay.UI;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Mathlife.ProjectL.Gameplay
{
    public class BatteryPageArtySlotItemDragView : AbstractView
    {
        [SerializeField] Image portraitImage;
        [SerializeField] TextMeshProUGUI levelText;
        [SerializeField] TextMeshProUGUI nameText;

        private ArtyModel arty = null;

        public void Setup(ArtyModel pArty)
        {
            arty = pArty;
        }

        public override void Draw()
        {
            portraitImage.sprite = arty.Sprite;
            levelText.text = arty.levelRx.ToString();
            nameText.text = arty.displayName;
        }
    }
}