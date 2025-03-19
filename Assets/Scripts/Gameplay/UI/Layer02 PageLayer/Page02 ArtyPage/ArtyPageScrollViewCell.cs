using System;
using Coffee.UIEffects;
using Cysharp.Threading.Tasks;
using Mathlife.ProjectL.Gameplay.UI.BatteryPagePopup;
using Mathlife.ProjectL.Gameplay.UI.Extension;
using Mathlife.ProjectL.Utils;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

namespace Mathlife.ProjectL.Gameplay.UI.ArtyPageView
{
    public class ArtyPageScrollViewCell
        : FancyCell<ItemData, Context>
    {
        // Alias
        private ArtyPage ArtyPage => Presenter.Find<ArtyPage>();

        // View
        private Canvas canvas;
        private CanvasRenderer canvasRenderer;
        private UIEffect uiEffect;
        private Animator animator;

        [SerializeField]
        private Button cellButton;

        [SerializeField]
        private Image portraitImage;

        [SerializeField]
        private TextMeshProUGUI levelText;

        [SerializeField]
        private TextMeshProUGUI nameText;

        // Field
        private ArtyModel arty;

        static class AnimatorHash
        {
            public static readonly int Scroll = Animator.StringToHash("ArtyPageScrollViewCell Scroll");
        }

        // Override
        public override void Initialize()
        {
            canvas = GetComponent<Canvas>();
            canvasRenderer = GetComponent<CanvasRenderer>();

            UniTask.WaitUntil(Presenter.Has<ArtyPage>)
                .ContinueWith(() => canvasRenderer.EnableRectClippingRecursive(ArtyPage.ScrollViewMaskRect))
                .Forget();
            
            uiEffect = GetComponent<UIEffect>();
            animator = GetComponent<Animator>();

            cellButton.OnClickAsObservable()
                .Subscribe(OnClick)
                .AddTo(gameObject);
        }

        public void OnDestroy()
        {
            canvasRenderer.DisableRectClippingRecursive();
        }

        public override void UpdateContent(ItemData itemData)
        {
            arty = itemData.arty;
            portraitImage.sprite = itemData.arty?.Sprite;
            levelText.text = itemData.arty?.levelRx.Value.ToString();
            nameText.text = itemData.arty?.DisplayName;
            
            uiEffect.LoadPreset(Context.selectedIndex == Index
                ? "GradientArtyCellSelected"
                : "GradientArtyCellDefault");
        }

        public override void UpdatePosition(float position)
        {
            animator.Play(AnimatorHash.Scroll, -1, position);
            animator.speed = 0;

            // Sorting Order
            // - 0 -> 200
            // - ...
            // - 3/7 -> 240
            // - ...
            // - 7/7 -> 200
            // - Lerp를 통해 Sorting Order를 결정한다. 

            int order = 0;
            const int minOrder = 200;
            const int maxOrder = 240;
            // const int minOrder = 0;
            // const int maxOrder = 100;
            float maximalPosition = Context.scrollOffset;

            if (position < maximalPosition)
            {
                float numerator = minOrder * (maximalPosition - position) + maxOrder * position;
                order = Mathf.FloorToInt(numerator / maximalPosition);
            }
            else
            {
                float numerator = maxOrder * (1 - position) + minOrder * (position - maximalPosition);
                order = Mathf.FloorToInt(numerator / (1 - maximalPosition));
            }

            ChangeSortingOrder(order);
        }

        // 상호작용 콜백
        private void OnClick(Unit _)
        {
            Context.onCellClickRx.OnNext(Index);
        }

        // 구현
        private void ChangeSortingOrder(int order)
        {
            canvas.sortingOrder = order;
        }
    }
}