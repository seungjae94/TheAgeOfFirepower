using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using DG.Tweening;
using Cysharp.Threading.Tasks;

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
    }
}
