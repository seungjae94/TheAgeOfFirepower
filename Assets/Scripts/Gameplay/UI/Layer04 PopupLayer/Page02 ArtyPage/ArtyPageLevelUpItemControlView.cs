using System;
using Coffee.UIEffects;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Mathlife.ProjectL.Gameplay.UI
{
    public class ArtyPageLevelUpItemControlView : AbstractView
    {
        // Alias
        private static ArtyPageLevelUpPopup popup => Presenter.Find<ArtyPageLevelUpPopup>();
        
        // Component
        [SerializeField]
        private Button addButton;
        
        [SerializeField]
        private Button subtractButton;

        [SerializeField]
        private UIEffect uiEffect;
        
        [SerializeField]
        private Image itemIcon;
        
        [SerializeField]
        private TextMeshProUGUI amountText;

        // Field
        private readonly CompositeDisposable disposables = new();
        private MaterialItemGameData itemGameData;

        private int currentAmount;
        private int totalAmount;
        
        public void Setup(MaterialItemGameData itemGameData, int amount)
        {
            this.itemGameData = itemGameData;
            totalAmount = amount;
            
        }
        
        public override void Draw()
        {
            base.Draw();

            currentAmount = 0;
            amountText.text = $"{currentAmount}/{totalAmount}";
            uiEffect.LoadPreset(itemGameData.rarity.ToGradientPresetName());
            itemIcon.sprite = itemGameData.icon;
            addButton.interactable = true;
            subtractButton.gameObject.SetActive(false);
            
            addButton.OnClickAsObservable()
                .Subscribe(AddAmount)
                .AddTo(disposables);
            
            subtractButton.OnClickAsObservable()
                .Subscribe(SubtractAmount)
                .AddTo(disposables);
        }

        public override void Clear()
        {
            base.Clear();

            disposables.Clear();
        }

        private void OnDestroy()
        {
            disposables.Dispose();
        }

        private void AddAmount(Unit _)
        {
            if (currentAmount == totalAmount)
                return;

            ++currentAmount;
            amountText.text = $"{currentAmount}/{totalAmount}";

            popup.ExpGainRx.Value += itemGameData.gainValue;

            if (currentAmount == totalAmount)
            {
                addButton.interactable = false;
            }
            
            if (subtractButton.gameObject.activeSelf == false)
            {
                subtractButton.gameObject.SetActive(true);
            }
        }
        
        private void SubtractAmount(Unit _)
        {
            if (currentAmount == 0)
                return;

            --currentAmount;
            amountText.text = $"{currentAmount}/{totalAmount}";
            popup.ExpGainRx.Value -= itemGameData.gainValue;

            if (currentAmount == 0)
            {
                subtractButton.gameObject.SetActive(false);
            }
            
            if (addButton.interactable == false)
            {
                addButton.interactable = true;
            }
        }
    }
}