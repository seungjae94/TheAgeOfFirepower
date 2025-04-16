using System;
using Coffee.UIEffects;
using Cysharp.Threading.Tasks;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Mathlife.ProjectL.Gameplay.UI
{
    public class ShopItemScrollRectCell
        : SimpleScrollRectCell<ShopItemSaleInfo, SimpleScrollRectContext>
    {
        // Alias
        private static InventoryState InventoryState => GameState.Inst.inventoryState;
        
        // View
        [SerializeField]
        private UIEffect uiEffect;

        [SerializeField]
        private Image iconImage;

        [SerializeField]
        private TextMeshProUGUI nameText;
        
        [SerializeField]
        private TextMeshProUGUI descriptionText;

        [SerializeField]
        private TextMeshProUGUI priceText;
        
        [SerializeField]
        private Button buyButton;
        
        // Field
        private ShopItemSaleInfo saleInfo;
        private readonly CompositeDisposable disposables = new();

        public override void UpdateContent(ShopItemSaleInfo itemData)
        {
            saleInfo = itemData;

            if (saleInfo == null)
            {
                throw new ArgumentNullException();
            }

            uiEffect.LoadPreset(saleInfo.item.rarity.ToGradientPresetName());
            iconImage.sprite = saleInfo.item.icon;
            
            nameText.text = saleInfo.item.displayName;
            if (saleInfo.amount > 1)
            {
                nameText.text += $" X {saleInfo.amount}";
            }

            if (saleInfo.item is MechPartGameData mechPartGameData)
            {
                descriptionText.text = mechPartGameData.stat.Description;    
            }
            else
            {
                descriptionText.text = saleInfo.item.description;
            }

            priceText.text = (saleInfo.price * saleInfo.amount).ToString();
            
            disposables.Clear();
            buyButton.OnClickAsObservable()
                .Subscribe(OnClickBuyButton)
                .AddTo(disposables);
        }

        private void OnDestroy()
        {
            disposables.Dispose();
        }

        private void OnClickBuyButton(Unit _)
        {
            AudioManager.Inst.PlaySE(ESoundEffectId.Buy);
            BuyItemAsync().Forget();
        }

        private async UniTaskVoid BuyItemAsync()
        {
            TopBar topBar = Presenter.Find<TopBar>();
            
            await UniTask.WaitWhile(topBar, pCurrencyBar => pCurrencyBar.IsGoldTweening == true);
            
            bool canBuy = InventoryState.CanBuyByGold(saleInfo.price, saleInfo.amount);
            if (canBuy)
            {

                long doTarget = InventoryState.Gold - saleInfo.price;
                await topBar.DOGold(doTarget);

                InventoryState.BuyItem(saleInfo);
                
                // 변경사항 저장
                GameState.Inst.Save();
                
                topBar.SubscribeGoldChange();
            }
            else
            {
                OKPopup popup = Presenter.Find<OKPopup>();
                popup.Setup("구매 실패", "골드가 부족합니다.");
                popup.OpenWithAnimation().Forget();
            }
        }
    }
}