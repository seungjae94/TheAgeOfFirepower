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
    public class LoadingScreenManager : MonoSingleton<LoadingScreenManager>
    {
        protected override SingletonLifeTime LifeTime => SingletonLifeTime.App;
        
        private Camera loadingScreenCamera;
        private Slider progressBar;

        protected override void OnRegistered()
        {
            loadingScreenCamera = transform.FindRecursive<Camera>();
            progressBar = transform.FindRecursive<Slider>();
        }

        private void Start()
        {
            Show();
        }

        public void Show()
        {
            loadingScreenCamera.enabled = true;
        }

        public void Hide()
        {
            loadingScreenCamera.enabled = false;
        }

        public void SetProgress(float value)
        {
            progressBar.value = value;
        }
    }
}
