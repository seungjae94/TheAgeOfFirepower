using System;
using Mathlife.ProjectL.Gameplay.Play;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Mathlife.ProjectL.Gameplay.UI
{
    public class FireHUD : Presenter, IInteractable
    {
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

        // Field
        private static ArtyController TurnOwner => PlaySceneGameMode.Inst.turnOwner;
        private readonly CompositeDisposable disposables = new();

        public override void Activate()
        {
            base.Activate();

            angleSlider.OnValueChangedAsObservable()
                .Subscribe(OnAngleSliderValueChanged)
                .AddTo(disposables);

            powerSlider.OnValueChangedAsObservable()
                .Subscribe(OnPowerSliderValueChanged)
                .AddTo(disposables);

            fireButton.OnClickAsObservable()
                .Subscribe(OnClickFireButton)
                .AddTo(disposables);
        }

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

            int iFireAngle = TurnOwner.FireAngle;
            int iFirePower = TurnOwner.FirePower;

            angleSlider.value = (iFireAngle + 30f) / 105f;
            powerSlider.value = (iFirePower - 1) / 99f;
        }

        public void Disable()
        {
            angleSlider.interactable = false;
            powerSlider.interactable = false;
            fireButton.interactable = false;
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

        private void OnPowerSliderValueChanged(float power)
        {
            // 1 ~ 100
            int iPower = Mathf.RoundToInt(1 + power * 99);
            powerText.text = iPower.ToString();

            TurnOwner?.SetFirePower(iPower);
        }

        private void OnClickFireButton(Unit _)
        {
            TurnOwner?.Fire();
        }
    }
}