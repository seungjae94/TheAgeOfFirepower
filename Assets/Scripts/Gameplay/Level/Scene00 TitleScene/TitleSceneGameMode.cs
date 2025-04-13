using System;
using Cysharp.Threading.Tasks;
using Mathlife.ProjectL.Gameplay.ObjectBase;
using Mathlife.ProjectL.Utils;
using UniRx;
using UnityEngine;
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

        void Start()
        {
            gameStartButtonCanvasGroup.Hide();
        }
        
        public override UniTask InitializeScene(IProgress<float> progress)
        {
            gameStartButton.OnClickAsObservable()
                .Subscribe(OnClickGameStartButton)
                .AddTo(gameObject);
            gameStartButtonCanvasGroup.Show();
            
            progress.Report(1f);
            return UniTask.CompletedTask;
        }

        private void OnClickGameStartButton(Unit _)
        {
            Debug.Log("클릭");
            
            GameManager.Inst.ChangeScene(SceneNames.LobbyScene)
                .Forget();
        }
    }
}