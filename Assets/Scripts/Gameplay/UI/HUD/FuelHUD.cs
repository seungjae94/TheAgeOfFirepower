using Mathlife.ProjectL.Utils;
using TMPro;
using UnityEngine;

namespace Mathlife.ProjectL.Gameplay.UI
{
    public class FuelHUD : Presenter
    {
        [SerializeField]
        private SlicedFilledImage fuelSlider;

        [SerializeField]
        private TextMeshProUGUI fuelText;

        public void SetFuel(float currentFuel, int maxFuel)
        {
            fuelSlider.fillAmount = currentFuel / maxFuel;
            fuelText.text = $"{Mathf.CeilToInt(currentFuel)}<space=0.2em>/<space=0.2em>{maxFuel}";
        }
    }
}