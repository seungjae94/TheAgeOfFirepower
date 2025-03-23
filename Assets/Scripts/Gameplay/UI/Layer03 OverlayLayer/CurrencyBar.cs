using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.Serialization;

namespace Mathlife.ProjectL.Gameplay.UI
{
    public class CurrencyBar : OverlayPresenter
    {
        InventoryState InventoryState => GameState.Inst.inventoryState;

        [SerializeField]
        TextMeshProUGUI goldText;

        [SerializeField]
        TextMeshProUGUI diamondText;

        private readonly CompositeDisposable goldSubscription = new();
        private readonly CompositeDisposable diamondSubscription = new();

        public bool IsGoldTweening { get; private set; } = false;
        public bool IsDiamondTweening { get; private set; } = false;
        
        public override void Activate()
        {
            base.Activate();

            SubscribeGoldChange();
            SubscribeDiamondChange();
            
            goldText.text = InventoryState.goldRx.Value.ToString();
            diamondText.text = InventoryState.diamondRx.Value.ToString();
        }

        public override void Deactivate()
        {
            base.Deactivate();

            goldSubscription.Clear();
        }

        private void OnDestroy()
        {
            goldSubscription.Dispose();
        }

        public void SubscribeGoldChange()
        {
            goldSubscription.Clear();
            
            InventoryState
                .goldRx
                .Subscribe(gold => goldText.text = gold.ToString())
                .AddTo(goldSubscription);
        }
        
        public void SubscribeDiamondChange()
        {
            diamondSubscription.Clear();
            
            InventoryState
                .diamondRx
                .Subscribe(diamond => diamondText.text = diamond.ToString())
                .AddTo(diamondSubscription);
        }

        public async UniTask DOGold(long targetGold, float duration = 0.25f)
        {
            IsGoldTweening = true;
            
            goldSubscription.Clear();
            
            long curValue = InventoryState.goldRx.Value;
            Tween tween = DOTween.To(
                    () => curValue,
                    x =>
                    {
                        curValue = x;
                        goldText.text = curValue.ToString();
                    },
                    targetGold, duration)
                .SetEase(Ease.OutQuad);
            
            await tween.AwaitForComplete();

            IsGoldTweening = false;
        }
        
        public async UniTask DODiamond(long targetDiamond, float duration = 0.25f)
        {
            IsDiamondTweening = true;
            
            diamondSubscription.Clear();
            
            long curValue = InventoryState.diamondRx.Value;
            Tween tween = DOTween.To(
                    () => curValue,
                    x =>
                    {
                        curValue = x;
                        diamondText.text = curValue.ToString();
                    },
                    targetDiamond, duration)
                .SetEase(Ease.OutQuad);
            
            await tween.AwaitForComplete();

            IsDiamondTweening = false;
        }
    }
}