using System;
using Cysharp.Threading.Tasks;
using Mathlife.ProjectL.Gameplay.ObjectBase;
using Mathlife.ProjectL.Utils;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Mathlife.ProjectL.Gameplay
{
    public class TitleSceneGameMode : GameMode<TitleSceneGameMode>
    {
        protected override SingletonLifeTime LifeTime => SingletonLifeTime.Scene;

        [SerializeField]
        Button gameStartButton;

        CanvasGroup gameStartButtonCanvasGroup;

        void Awake()
        {
            gameStartButtonCanvasGroup = gameStartButton.GetComponent<CanvasGroup>();
        }

        public override UniTask PreInitializeGame()
        {
            gameStartButtonCanvasGroup.Hide();
            return UniTask.CompletedTask;
        }

        public override UniTask PostInitializeGame()
        {
            gameStartButtonCanvasGroup.Show();
            return UniTask.CompletedTask;
        }

        public override UniTask InitializeScene(IProgress<float> progress)
        {
            gameStartButton.OnClickAsObservable()
                .Subscribe(_ => OnClickGameStartButton())
                .AddTo(gameObject);
            return UniTask.CompletedTask;
        }

        private void OnClickGameStartButton()
        {
            GameManager.Inst.ChangeScene(SceneNames.LobbyScene).Forget();
        }
    }
}