using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Mathlife.ProjectL.Gameplay.UI
{
    public class ArtyPageLevelUpPopup : PopupPresenter
    {
        private const float OPEN_DURATION = 0.25f;

        // Alias
        private static ArtyModel Arty => Presenter.Find<ArtyPage>().SelectedArty;
        
        // Component
        [SerializeField]
        private RectTransform windowTransform;

        [SerializeField]
        private Button closeButton;

        [SerializeField]
        private Button applyButton;
        
        [SerializeField]
        private Button clearButton;
        
        [SerializeField]
        private ArtyPageLevelUpStatView statView;

        [SerializeField]
        private List<ArtyPageLevelUpItemControlView> itemControlViews;

        // Field
        private Tween openTween;
        private Tween closeTween;
        private readonly CompositeDisposable disposables = new();

        public ReactiveProperty<long> ExpGainRx { get; private set; }

        public override void Initialize()
        {
            base.Initialize();

            openTween = windowTransform.DOScale(new Vector3(1f, 1f, 1f), OPEN_DURATION)
                .From(new Vector3(0f, 0f, 1f))
                .SetAutoKill(false)
                .Pause();

            closeTween = windowTransform.DOScale(new Vector3(0f, 0f, 1f), OPEN_DURATION)
                .From(new Vector3(1f, 1f, 1f))
                .SetAutoKill(false)
                .Pause();
        }

        void OnDestroy()
        {
            openTween.Kill();
            closeTween.Kill();

            disposables.Dispose();
            ExpGainRx?.Dispose();
        }

        public override async UniTask OpenWithAnimation()
        {
            // 블러 적용
            BlurPopup blurPopup = Find<BlurPopup>();
            blurPopup.transform.SetSiblingIndex(transform.GetSiblingIndex() - 1);
            await blurPopup.OpenWithAnimation();

            await base.OpenWithAnimation();

            // 애니메이션
            openTween.Restart();
            
            // 초기화
            ExpGainRx = new(0L);
            applyButton.interactable = true;
            
            closeButton.OnClickAsObservable()
                .Subscribe(_ => CloseWithAnimation().Forget())
                .AddTo(disposables);
            
            applyButton.OnClickAsObservable()
                .Subscribe(OnClickApplyButton)
                .AddTo(disposables);
            
            clearButton.OnClickAsObservable()
                .Subscribe(OnClickClearItemsButton)
                .AddTo(disposables);

            statView.Draw();
            for (int i = 0; i < itemControlViews.Count; i++)
            {
                var itemData = GameState.Inst.gameDataLoader.GetCountableItemData(EItemType.MaterialItem, i);
                if (itemData == null)
                {
                    Debug.LogError($"뷰의 개수({itemControlViews.Count})에 비해 아이템 데이터 개수({i})가 부족합니다.");
                    break;
                }

                var itemStack = GameState.Inst.inventoryState.GetMaterialItemStack(itemData.id);

                int itemCount = itemStack?.Amount ?? 0;
                itemControlViews[i].Setup((MaterialItemGameData)itemData, itemCount);
                itemControlViews[i].Draw();
            }
        }

        public override async UniTask CloseWithAnimation()
        {
            disposables.Clear();

            closeTween.Restart();
            await closeTween.AwaitForComplete();

            statView.Clear();
            itemControlViews.ForEach(view => view.Clear());
            
            // 블러 제거
            await Find<BlurPopup>().CloseWithAnimation();

            await base.CloseWithAnimation();
        }

        private void OnClickApplyButton(Unit _)
        {
            applyButton.interactable = false;
            
            // 경험치 획득
            long expGain = ExpGainRx.Value;
            ExpGainRx.Dispose();
            Arty.GainExp(expGain);
            
            // 아이템 소모
            for (int i = 0; i < itemControlViews.Count; i++)
            {
                GameState.Inst.inventoryState.LoseCountableItems(itemControlViews[i].ItemGameData, itemControlViews[i].CurrentAmount);
            }

            GameState.Inst.Save();
            
            Find<ArtyPage>().UpdateSelectedArtyView();
            CloseWithAnimation().Forget();
        }
        
        private void OnClickClearItemsButton(Unit _)
        {
            ExpGainRx.Value = 0L;
            
            statView.Clear();
            statView.Draw();
            
            foreach (var view in itemControlViews)
            {
                view.Clear();
                view.Draw();
            }
        }
    }
}