using Cysharp.Threading.Tasks;
using Mathlife.ProjectL.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

namespace Mathlife.ProjectL.Gameplay
{
    [RequireComponent(typeof(CanvasGroup))]
    public class FadingScreen : MonoBehaviour
    {
        CanvasGroup m_canvasGroup;

        void Awake()
        {
            m_canvasGroup = GetComponent<CanvasGroup>();
        }

        public async UniTask Show(float duration)
        {
            await m_canvasGroup.Show(duration);
        }

        public async UniTask Hide(float duration)
        {
            await m_canvasGroup.Hide(duration);
        }
    }
}
