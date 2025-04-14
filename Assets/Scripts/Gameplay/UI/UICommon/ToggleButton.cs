using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Mathlife.ProjectL.Gameplay.UI
{
    [Serializable]
    public class ToggleButtonPreset
    {
        public Color imageColor;
        public Color fontColor;
        public string text;
    }

    public class ToggleButton : Button
    {
        [SerializeField]
        private Image targetImage;
        
        [SerializeField]
        private TextMeshProUGUI targetText;

        [SerializeField]
        private ToggleButtonPreset onPreset = new();
        
        [SerializeField]
        private ToggleButtonPreset offPreset = new();

        private bool isOn = true;

        public bool IsOn
        {
            get => isOn;
            set
            {
                isOn = value;
                ApplyTogglePreset(isOn ? onPreset : offPreset);
            }
        }
        
        protected override void Awake()
        {
            base.Awake();

            targetImage = GetComponent<Image>();
            targetText = GetComponentInChildren<TextMeshProUGUI>();
            
            transition = Selectable.Transition.None;
            onClick.AddListener(DoToggle);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            
            onClick.RemoveAllListeners();
        }

        private void DoToggle()
        {
            IsOn = !IsOn;
        }

        private void ApplyTogglePreset(ToggleButtonPreset preset)
        {
            targetImage.color = preset.imageColor;
            targetText.color = preset.fontColor;
            targetText.text = preset.text;
        }
    }
}