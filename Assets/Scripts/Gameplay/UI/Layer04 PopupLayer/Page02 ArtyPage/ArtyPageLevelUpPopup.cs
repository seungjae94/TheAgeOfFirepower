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

        // Component
        [SerializeField]
        private RectTransform windowTransform;

        [SerializeField]
        private Button closeButton;

        [SerializeField]
        private ArtyPageLevelUpStatView statView;

        [SerializeField]
        private List<ArtyPageLevelUpItemControlView> itemControlViews;

        // Field
        private Tween openTween;
        private Tween closeTween;
        private readonly CompositeDisposable disposables = new();

        public readonly ReactiveProperty<long> expGainRx = new(0L);

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
            //await openTween.AwaitForComplete();

            closeButton.OnClickAsObservable()
                .Subscribe(_ => CloseWithAnimation().Forget())
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

            // 블러 제거
            await Find<BlurPopup>().CloseWithAnimation();

            await base.CloseWithAnimation();
        }
    }
}