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

        public override void Activate()
        {
            base.Activate();

            SubscribeGoldChange();
            
            goldText.text = InventoryState.goldRx.Value.ToString();
            diamondText.text = 0.ToString();
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

        public async UniTask DOGold(long targetGold, float duration = 0.5f)
        {
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
        }
    }
}