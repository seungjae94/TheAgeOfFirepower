using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Sirenix.Utilities;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Mathlife.ProjectL.Gameplay
{
    public class GameManager : MonoSingleton<GameManager>
    {
        protected override SingletonLifeTime LifeTime => SingletonLifeTime.App;

        private Scene currentScene;
        private IGameMode currentGameMode = null;

        private void Start()
        {
            InitializeGame().Forget();
        }

        private void RefreshCurrentSceneAndGameMode()
        {
            if (SceneManager.loadedSceneCount == 1)
            {
                Debug.LogError("[GameManager] Failed to refresh scene data. Current scene is null.");
                return;
            }
            
            currentScene = SceneManager.GetSceneAt(1);
            currentGameMode = currentScene.GetRootGameObjects()
                .FirstOrDefault(root => root.TryGetComponent(out IGameMode gameModeComponent))
                ?.GetComponent<IGameMode>();

            if (currentGameMode == null)
            {
                Debug.LogError("[GameManager] Failed to refresh scene data. Current GameMode is null.");
                return;
            }
        }

        private async UniTask InitializeGame()
        {
            // AppScope 씬만 로딩되어 있는 경우 타이틀 씬을 로드한다.
            if (SceneManager.loadedSceneCount == 1)
            {
                await SceneManager.LoadSceneAsync(SceneManager.GetSceneByBuildIndex(1).name, LoadSceneMode.Additive).ToUniTask();
            }
            
            // 씬 데이터 갱신
            RefreshCurrentSceneAndGameMode();

            // 게임 초기화
            await currentGameMode.PreInitializeGame();
            await GameState.Inst.Load();
            await currentGameMode.PostInitializeGame();

            // 씬 초기화
            LoadingScreenManager.Inst.Show();
            
            IProgress<float> progress = Progress.Create<float>(value => LoadingScreenManager.Inst.SetProgress(value));
            await currentGameMode.InitializeScene(progress);
            
            LoadingScreenManager.Inst.Hide();
        }

        public async UniTaskVoid ChangeScene(string sceneName)
        {
            LoadingScreenManager.Inst.Show();

            // 기존 씬 언로드 & 새로운 씬 로드
            await SceneManager.UnloadSceneAsync(currentScene).ToUniTask();
            await SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive).ToUniTask();

            // 씬 데이터 갱신
            RefreshCurrentSceneAndGameMode();
            
            // 새로운 씬 초기화
            IProgress<float> progress = Progress.Create<float>(value => LoadingScreenManager.Inst.SetProgress(value));
            await currentGameMode.InitializeScene(progress);

            LoadingScreenManager.Inst.Hide();
        }
    }
}