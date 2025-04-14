using Coffee.UIEffects;
using Mathlife.ProjectL.Gameplay.Play;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

namespace Mathlife.ProjectL.Gameplay.UI
{
    public class BattleItemButton : MonoBehaviour, IInteractable
    {
        // Alias
        private static InventoryState InventoryState => GameState.Inst.inventoryState;
        private static ArtyController TurnOwner => PlaySceneGameMode.Inst.turnOwner;
        
        // Component
        [SerializeField]
        private Button button;

        [SerializeField]
        private Graphic[] graphics;

        [SerializeField]
        private Image iconImage;

        [SerializeField]
        private UIEffect uiEffect;
        
        [SerializeField]
        private TextMeshProUGUI amountText;

        [SerializeField]
        private Color disabledFontColor;
        
        [SerializeField]
        private AssetReferenceT<BattleItemGameData> itemDataRef;
        
        // Field
        private Color[] colors;
        private Color fontColor;
        
        private BattleItemGameData itemData;
        
        private readonly CompositeDisposable disposables = new();

        public void Start()
        {
            colors = new Color[graphics.Length];

            for (int i = 0; i < graphics.Length; i++)
            {
                colors[i] =  graphics[i].color;
            }

            fontColor = amountText.color;
        }
        
        public void Setup()
        {
            AsyncOperationHandle<BattleItemGameData> handle = itemDataRef.LoadAssetAsync();
            itemData = handle.WaitForCompletion();
            
            iconImage.sprite = itemData.icon;
            uiEffect.LoadPreset(itemData.rarity.ToGradientPresetName());
        }

        public void Enable()
        {
            ItemStackModel stack = InventoryState.GetBattleItemStack(itemData.id);

            if (stack == null || stack.Amount <= 0)
            {
                button.interactable = false;
                SetAmount(0);
                return;
            }

            button.interactable = true;
            button.OnClickAsObservable()
                .Subscribe(OnClick)
                .AddTo(disposables);
            SetAmount(stack.Amount);
        }
        
        public void Disable()
        {
            button.interactable = false;
            disposables.Clear();
        }

        private void OnDestroy()
        {
            disposables.Dispose();
        }

        private void SetAmount(int amount)
        {
            amount = Mathf.Max(amount, 0);
            amountText.text = amount.ToString();

            if (amount == 0)
            {
                Disable();
            }
        }
        
        private void NormalCallback()
        {
            GrayTintGraphics(1f);
            amountText.color = fontColor;
        }
        
        private void SelectedCallback()
        {
            GrayTintGraphics(1f);
            amountText.color = fontColor;
        }
        
        private void HighlightedCallback()
        {
            GrayTintGraphics(1f);
            amountText.color = fontColor;
        }
        
        private void PressedCallback()
        {
            GrayTintGraphics(0.5f);
            amountText.color = fontColor;
        }
        
        private void DisabledCallback()
        {
            GrayTintGraphics(0.5f);
            amountText.color = disabledFontColor;
        }

        private void GrayTintGraphics(float t)
        {
            for (int i = 0; i < graphics.Length; i++)
            {
                Color newColor = colors[i] * t;
                newColor.a = 1f;
                graphics[i].color = newColor;
            }
        }

        private void OnClick(Unit _)
        {
            InventoryState.UseBattleItem(itemData.id, TurnOwner);
            
            ItemStackModel stack = InventoryState.GetBattleItemStack(itemData.id);
            button.interactable = false;
            SetAmount(stack?.Amount ?? 0);
            
            Presenter.Find<ItemHUD>().Disable();
        }
    }
}