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
                throw new ArgumentNullException("null 부품 데이터가 인자로 들어왔습니다.");
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
            BuyItemAsync().Forget();
        }

        private async UniTaskVoid BuyItemAsync()
        {
            CurrencyBar currencyBar = Presenter.Find<CurrencyBar>();
            
            await UniTask.WaitWhile(currencyBar, pCurrencyBar => pCurrencyBar.IsTweening == true);
            
            bool canBuy = InventoryState.CanBuyItem(saleInfo.price, saleInfo.amount);
            if (canBuy)
            {

                long doTarget = InventoryState.goldRx.Value - saleInfo.price;
                await currencyBar.DOGold(doTarget);

                InventoryState.BuyItem(saleInfo);
                currencyBar.SubscribeGoldChange();
            }
            else
            {
                Debug.Log("구매 실패 알림창");
            }
        }
    }
}