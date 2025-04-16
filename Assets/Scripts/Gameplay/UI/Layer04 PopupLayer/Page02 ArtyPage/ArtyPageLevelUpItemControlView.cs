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
        public MaterialItemGameData ItemGameData { get; private set; }

        public int CurrentAmount { get; private set; }
        private int totalAmount;
        
        public void Setup(MaterialItemGameData itemGameData, int amount)
        {
            ItemGameData = itemGameData;
            totalAmount = amount;
        }
        
        public override void Draw()
        {
            base.Draw();

            CurrentAmount = 0;
            amountText.text = $"{CurrentAmount}/{totalAmount}";
            uiEffect.LoadPreset(ItemGameData.rarity.ToGradientPresetName());
            itemIcon.sprite = ItemGameData.icon;
            EnableOrDisableButtons();
            
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
            if (CurrentAmount == totalAmount)
                return;

            AudioManager.Inst.PlaySE(ESoundEffectId.Ok);
            
            ++CurrentAmount;
            amountText.text = $"{CurrentAmount}/{totalAmount}";

            popup.ExpGainRx.Value += ItemGameData.gainValue;

            EnableOrDisableButtons();
        }
        
        private void SubtractAmount(Unit _)
        {
            if (CurrentAmount == 0)
                return;

            AudioManager.Inst.PlaySE(ESoundEffectId.Ok);
            
            --CurrentAmount;
            amountText.text = $"{CurrentAmount}/{totalAmount}";
            popup.ExpGainRx.Value -= ItemGameData.gainValue;

            EnableOrDisableButtons();
        }

        private void EnableOrDisableButtons()
        {
            addButton.interactable = CurrentAmount != totalAmount;
            subtractButton.gameObject.SetActive(CurrentAmount != 0);
        }
    }
}