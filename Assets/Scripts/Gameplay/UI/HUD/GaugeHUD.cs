using System;
using Mathlife.ProjectL.Gameplay.Play;
using Mathlife.ProjectL.Utils;
using TMPro;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Mathlife.ProjectL.Gameplay.UI
{
    public class GaugeHUD : Presenter, IInteractable
    {
        // Alias
        private static ArtyController TurnOwner => PlaySceneGameMode.Inst.turnOwner;

        // Component
        [SerializeField]
        private Slider angleSlider;

        [SerializeField]
        private Slider powerSlider;

        [SerializeField]
        private TextMeshProUGUI angleText;

        [SerializeField]
        private TextMeshProUGUI powerText;

        [SerializeField]
        private Button fireButton;

        [SerializeField]
        private Button skipButton;

        [SerializeField]
        private SlicedFilledImage fuelSlider;

        [SerializeField]
        private TextMeshProUGUI fuelText;

        // Field
        private SliderObservable angleSliderObservable;
        private SliderObservable powerSliderObservable;
        private readonly CompositeDisposable disposables = new();

        public override void Deactivate()
        {
            base.Deactivate();

            disposables.Clear();
        }

        public void Enable()
        {
            angleSlider.interactable = true;
            powerSlider.interactable = true;
            fireButton.interactable = true;
            skipButton.interactable = true;

            int iFireAngle = TurnOwner.FireAngle;
            int iFirePower = TurnOwner.FirePower;

            angleSlider.value = (iFireAngle + 30f) / 105f;
            powerSlider.value = (iFirePower - 10) / 90f;

            disposables.Clear();
            
            angleSliderObservable = new(angleSlider, 
                OnAngleSliderValueChanged, 
                OnSliderStartEdit,
                OnSliderEndEdit);
            disposables.Add(angleSliderObservable.disposables);
            
            powerSliderObservable = new(powerSlider, 
                OnPowerSliderValueChanged, 
                OnSliderStartEdit,
                OnSliderEndEdit);
            disposables.Add(powerSliderObservable.disposables);

            fireButton.OnClickAsObservable()
                .Subscribe(_ => TurnOwner?.Fire())
                .AddTo(disposables);

            skipButton.OnClickAsObservable()
                .Subscribe(_ => TurnOwner?.Skip())
                .AddTo(disposables);
        }

        public void Disable()
        {
            angleSlider.interactable = false;
            powerSlider.interactable = false;
            fireButton.interactable = false;
            skipButton.interactable = false;

            disposables.Clear();
        }

        private void OnDestroy()
        {
            disposables.Dispose();
        }

        private void OnAngleSliderValueChanged(float angle)
        {
            // -30도 ~ +75도
            int iAngle = Mathf.RoundToInt(-30 + angle * 105);

            string text = (iAngle >= 0) ? "+" : "";
            text += iAngle.ToString();
            angleText.text = text;

            TurnOwner?.SetFireAngle(iAngle);
        }

        private void OnSliderStartEdit(float value)
        {
            AudioManager.Inst.PlaySE(ESoundEffectId.Aim, true);
        }

        private void OnSliderEndEdit(float angle)
        {
            AudioManager.Inst.StopSE(true);
        }

        private void OnPowerSliderValueChanged(float power)
        {
            // 10 ~ 100
            int iPower = Mathf.RoundToInt(10 + power * 90);
            powerText.text = iPower.ToString();

            TurnOwner?.SetFirePower(iPower);
        }

        public void SetFuel(float currentFuel, int maxFuel)
        {
            fuelSlider.fillAmount = Mathf.Min(currentFuel, maxFuel) / maxFuel;
            fuelText.text = $"{Mathf.CeilToInt(currentFuel)}";
        }
    }
}