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
            currentAmount = amount;
        }
        
        public override void Draw()
        {
            base.Draw();

            amountText.text = $"{currentAmount}/{totalAmount}";
            uiEffect.LoadPreset(itemGameData.rarity.ToGradientPresetName());
            itemIcon.sprite = itemGameData.icon;
            
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

            popup.expGainRx.Value += itemGameData.gainValue;
            
            // TODO: 최대 개수일 때 비활성화
        }
        
        private void SubtractAmount(Unit _)
        {
            if (currentAmount == 0)
                return;

            --currentAmount;
            amountText.text = $"{currentAmount}/{totalAmount}";
            popup.expGainRx.Value -= itemGameData.gainValue;
            
            // TODO: 0개일 때 비활성화
        }
    }
}