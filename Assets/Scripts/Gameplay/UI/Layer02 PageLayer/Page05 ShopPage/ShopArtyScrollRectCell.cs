using System;
using Coffee.UIEffects;
using Cysharp.Threading.Tasks;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Mathlife.ProjectL.Gameplay.UI
{
    public class ShopArtyScrollRectCell
        : SimpleScrollRectCell<ShopArtySaleInfo, SimpleScrollRectContext>
    {
        // Alias
        private static InventoryState InventoryState => GameState.Inst.inventoryState;

        // View
        [SerializeField]
        private Image spriteImage;

        [SerializeField]
        private TextMeshProUGUI nameText;

        [SerializeField]
        private Image shellIconImage;

        [SerializeField]
        private TextMeshProUGUI shellNameText;

        [SerializeField]
        private TextMeshProUGUI shellDescriptionText;

        [SerializeField]
        private TextMeshProUGUI priceText;

        [SerializeField]
        private Button buyButton;

        // Field
        private ShopArtySaleInfo saleInfo;
        private readonly CompositeDisposable disposables = new();

        public override void UpdateContent(ShopArtySaleInfo itemData)
        {
            saleInfo = itemData;

            if (saleInfo == null)
            {
                throw new ArgumentNullException();
            }

            spriteImage.sprite = saleInfo.arty.sprite;
            nameText.text = saleInfo.arty.displayName;
            shellIconImage.sprite = saleInfo.arty.shell.icon;
            shellNameText.text = $"포탄 - {saleInfo.arty.shell.displayName}";
            shellDescriptionText.text = saleInfo.arty.shell.description;

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

            await UniTask.WaitWhile(topBar, pCurrencyBar => pCurrencyBar.IsDiamondTweening == true);

            bool canBuy = InventoryState.CanBuyByDiamond(saleInfo.price, saleInfo.amount);
            if (canBuy)
            {
                long doTarget = InventoryState.Diamond - saleInfo.price;
                await topBar.DODiamond(doTarget);

                InventoryState.BuyArty(saleInfo);
                
                // 변경사항 저장
                GameState.Inst.Save();
                
                topBar.SubscribeDiamondChange();
            }
            else
            {
                OKPopup popup = Presenter.Find<OKPopup>();
                popup.Setup("구매 실패", "다이아몬드가 부족합니다.");
                popup.OpenWithAnimation().Forget();
            }
        }
    }
}