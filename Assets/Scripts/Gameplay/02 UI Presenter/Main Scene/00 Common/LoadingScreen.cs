using Mathlife.ProjectL.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Mathlife.ProjectL.Gameplay
{
    [RequireComponent(typeof(CanvasGroup))]
    [RequireComponent(typeof(Slider))]
    public class LoadingScreen : MonoBehaviour
    {
        CanvasGroup m_canvasGroup;
        Slider m_progressBar;

        void Awake()
        {
            m_canvasGroup = GetComponent<CanvasGroup>();
            m_progressBar = transform.FindRecursive<Slider>();
        }

        public void Show()
        {
            m_canvasGroup.Show();
        }

        public void Hide()
        {
            m_canvasGroup.Hide();
        }

        public void SetProgress(float value)
        {
            m_progressBar.value = value;
        }
    }
}
