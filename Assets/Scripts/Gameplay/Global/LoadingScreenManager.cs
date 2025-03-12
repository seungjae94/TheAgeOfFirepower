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
    public class LoadingScreenManager : MonoSingleton<LoadingScreenManager>
    {
        protected override SingletonLifeTime LifeTime => SingletonLifeTime.App;
        
        private Camera loadingScreenCamera;
        private CanvasGroup loadingScreenCanvasGroup;
        private Slider progressBar;

        protected override void OnRegistered()
        {
            loadingScreenCamera = transform.FindRecursive<Camera>();
            loadingScreenCanvasGroup = transform.FindRecursive<CanvasGroup>();
            progressBar = transform.FindRecursive<Slider>();
        }

        public void Show()
        {
            loadingScreenCamera.enabled = true;
            loadingScreenCanvasGroup.Show();
        }

        public void Hide()
        {
            loadingScreenCamera.enabled = false;
            loadingScreenCanvasGroup.Hide();
        }

        public void SetProgress(float value)
        {
            progressBar.value = value;
        }
    }
}
