using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Cysharp.Threading.Tasks;
using UnityEngine.UI;

namespace Mathlife.ProjectL.Utils
{
    public static class UIExtensions
    {
        static Dictionary<CanvasGroup, Tween> tweens = new();

        public static void Show(this CanvasGroup group)
        {
            group.alpha = 1.0f;
            group.interactable = true;
            group.blocksRaycasts = true;
        }

        public static async UniTask Show(this CanvasGroup group, float duration)
        {
            if (tweens.ContainsKey(group))
            {
                tweens[group]?.Kill(false);
                tweens.Remove(group);
            }

            Tween tween = group.DOFade(1.0f, duration);
            await tween.ToUniTask();

            group.interactable = true;
            group.blocksRaycasts = true;
        }

        public static void Hide(this CanvasGroup group)
        {
            group.alpha = 0.0f;
            group.interactable = false;
            group.blocksRaycasts = false;
        }

        public static async UniTask Hide(this CanvasGroup group, float duration)
        {
            if (tweens.ContainsKey(group))
            {
                tweens[group]?.Kill(false);
                tweens.Remove(group);
            }

            Tween tween = group.DOFade(0.0f, duration);
            await tween.ToUniTask();

            group.interactable = false;
            group.blocksRaycasts = false;
        }

        public static void HideWithAlpha(this CanvasGroup group, float alpha)
        {
            group.alpha = alpha;
            group.interactable = false;
            group.blocksRaycasts = false;
        }

        public static void EnableRectClippingRecursive(this CanvasRenderer renderer, Rect clippingRect)
        {
            renderer.EnableRectClipping(clippingRect);
            EnableRectClippingRecursiveInternal(renderer.transform, clippingRect);
        }

        private static void EnableRectClippingRecursiveInternal(Transform parent, Rect clippingRect)
        {
            foreach (Transform child in parent)
            {
                if (child.TryGetComponent(out CanvasRenderer childRenderer))
                {
                    if (childRenderer.TryGetComponent(out MaskableGraphic graphic))
                    {
                        graphic.maskable = true;
                    }
                    
                    childRenderer.EnableRectClipping(clippingRect);
                }

                EnableRectClippingRecursiveInternal(child, clippingRect);
            }
        }
        
        public static void DisableRectClippingRecursive(this CanvasRenderer renderer)
        {
            renderer.DisableRectClipping();
            DisableRectClippingRecursiveInternal(renderer.transform);
        }
        
        private static void DisableRectClippingRecursiveInternal(Transform parent)
        {
            foreach (Transform child in parent)
            {
                if (child.TryGetComponent(out CanvasRenderer childRenderer))
                {
                    childRenderer.DisableRectClipping();
                }

                DisableRectClippingRecursiveInternal(child);
            }
        }
    }
}
