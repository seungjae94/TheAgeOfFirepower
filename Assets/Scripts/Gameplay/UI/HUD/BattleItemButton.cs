using DG.DemiEditor;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

namespace Mathlife.ProjectL.Gameplay.UI
{
    public class BattleItemButton : MonoBehaviour
    {
        [SerializeField]
        private Button button;

        [SerializeField]
        private Graphic[] graphics;

        [SerializeField]
        private TextMeshProUGUI amountText;

        [SerializeField]
        private Color disabledFontColor;
        
        // Field
        private Color[] colors;
        private Color fontColor;

        private void Start()
        {
            colors = new Color[graphics.Length];

            for (int i = 0; i < graphics.Length; i++)
            {
                colors[i] =  graphics[i].color;
            }

            fontColor = amountText.color;
        }

        public void Enable()
        {
            button.interactable = true;
        }
        
        public void Disable()
        {
            button.interactable = false;
        }
        
        private void NormalCallback()
        {
            GrayTintGraphics(1f);
            amountText.color = fontColor;
        }
        
        private void SelectedCallback()
        {
            GrayTintGraphics(1f);
            amountText.color = fontColor;
        }
        
        private void HighlightedCallback()
        {
            GrayTintGraphics(1f);
            amountText.color = fontColor;
        }
        
        private void PressedCallback()
        {
            GrayTintGraphics(0.5f);
            amountText.color = fontColor;
        }
        
        private void DisabledCallback()
        {
            GrayTintGraphics(0.5f);
            amountText.color = disabledFontColor;
        }

        private void GrayTintGraphics(float t)
        {
            for (int i = 0; i < graphics.Length; i++)
            {
                graphics[i].color = colors[i] * t;
                graphics[i].color.SetAlpha(1f);
            }
        }
    }
}