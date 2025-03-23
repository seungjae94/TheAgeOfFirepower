using System;
using Coffee.UIEffects;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Mathlife.ProjectL.Gameplay.UI
{
    public class ShopItemScrollRectCell
        : SimpleScrollRectCell<ItemGameData, SimpleScrollRectContext>
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
        private Button buyButton;
        
        // Field
        private ItemGameData gameData;
        private readonly CompositeDisposable disposables = new();

        public override void UpdateContent(ItemGameData itemData)
        {
            gameData = itemData;

            if (gameData == null)
            {
                throw new ArgumentNullException("null 부품 데이터가 인자로 들어왔습니다.");
            }

            uiEffect.LoadPreset(gameData.rarity.ToGradientPresetName());
            iconImage.sprite = gameData.icon;
            nameText.text = gameData.displayName;

            if (gameData is MechPartGameData mechPartGameData)
            {
                descriptionText.text = mechPartGameData.stat.Description;    
            }
            else
            {
                descriptionText.text = gameData.description;
            }
            
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
            bool buyResult = InventoryState.BuyMechPart(gameData.id);
            
            if (buyResult)
            {
                // 구매 이펙트
                Debug.Log("구매 이펙트");
            }
            else
            {
                // 구매 실패 알림
                Debug.Log("구매 실패 알림");
            }
        }
    }
}