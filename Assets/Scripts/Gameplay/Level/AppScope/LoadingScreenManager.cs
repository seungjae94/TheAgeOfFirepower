using Mathlife.ProjectL.Utils;
using Mathlife.ProjectL.Gameplay.ObjectBase;
using TMPro;
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
        private TextMeshProUGUI messageText;

        protected override void OnRegistered()
        {
            loadingScreenCamera = transform.FindRecursive<Camera>();
            loadingScreenCanvasGroup = transform.FindRecursive<CanvasGroup>();
            progressBar = transform.FindRecursive<Slider>();
            messageText = transform.FindRecursive<TextMeshProUGUI>();
        }

        private void Start()
        {
            progressBar.value = 0f;
            loadingScreenCamera.enabled = false;
            loadingScreenCanvasGroup.Hide();
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

        public void SetMessage(string message)
        {
            messageText.text = message;
        }
    }
}
