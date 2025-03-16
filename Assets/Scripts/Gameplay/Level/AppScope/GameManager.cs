using System;
using System.Linq;
using Cysharp.Threading.Tasks;
using Mathlife.ProjectL.Gameplay.ObjectBase;
using Mathlife.ProjectL.Gameplay.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Mathlife.ProjectL.Gameplay
{
    public class GameManager : MonoSingleton<GameManager>
    {
        protected override SingletonLifeTime LifeTime => SingletonLifeTime.App;

        private Scene currentScene;
        public IGameMode CurrentGameMode { get; private set; }
        public IMainCanvas CurrentCanvas { get; private set; }
        
        public Cameras CurrentCameras { get; private set; }
        
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
            CurrentGameMode = currentScene.GetRootGameObjects()
                .FirstOrDefault(root => root.TryGetComponent(out IGameMode _))
                ?.GetComponent<IGameMode>();
            CurrentCanvas = currentScene.GetRootGameObjects()
                .FirstOrDefault(root => root.TryGetComponent(out IMainCanvas _))
                ?.GetComponent<IMainCanvas>();
            CurrentCameras = currentScene.GetRootGameObjects()
                .FirstOrDefault(root => root.TryGetComponent(out Cameras _))
                ?.GetComponent<Cameras>();
            
            if (CurrentGameMode == null)
            {
                Debug.LogError("[GameManager] Failed to refresh scene data. Current GameMode is null.");
                return;
            }
        }

        private async UniTask InitializeGame()
        {
            bool isProgressSeparated = false; 
            
            // AppScope 씬만 로딩되어 있는 경우 타이틀 씬을 로드한다.
            if (SceneManager.loadedSceneCount == 1)
            {
                IProgress<float> sceneLoadingProgress = CreateProgress(0.0f, 0.5f);
                LoadingScreenManager.Inst.SetMessage("씬을 로딩하는 중...");
                await SceneManager.LoadSceneAsync(SceneNames.TitleScene, LoadSceneMode.Additive).ToUniTask(sceneLoadingProgress);

                isProgressSeparated = true;
            }
            
            // 씬 데이터 갱신
            RefreshCurrentSceneAndGameMode();

            // 게임 초기화
            await CurrentGameMode.PreInitializeGame();
            await GameState.Inst.Load();
            await CurrentGameMode.PostInitializeGame();

            // 씬 초기화
            LoadingScreenManager.Inst.Show();

            IProgress<float> progress = isProgressSeparated ? CreateProgress(0.5f, 1.0f) : CreateProgress(0.0f, 1.0f);
            await CurrentGameMode.InitializeScene(progress);
            
            LoadingScreenManager.Inst.Hide();
        }

        public async UniTaskVoid ChangeScene(string sceneName)
        {
            LoadingScreenManager.Inst.Show();

            // 기존 씬 언로드 & 새로운 씬 로드
            await SceneManager.UnloadSceneAsync(currentScene).ToUniTask();

            IProgress<float> sceneLoadingProgress = CreateProgress(0.0f, 0.5f);
            LoadingScreenManager.Inst.SetMessage("씬을 로딩하는 중...");
            await SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive).ToUniTask(sceneLoadingProgress);
            
            // 씬 데이터 갱신
            RefreshCurrentSceneAndGameMode();
            
            // 새로운 씬 초기화
            IProgress<float> sceneInitProgress = CreateProgress(0.5f, 1.0f);
            LoadingScreenManager.Inst.SetMessage("씬을 초기화하는 중...");
            await CurrentGameMode.InitializeScene(sceneInitProgress);

            LoadingScreenManager.Inst.Hide();
        }

        private IProgress<float> CreateProgress(float start, float end)
        {
            return Progress.Create<float>(value => LoadingScreenManager.Inst.SetProgress(start + (end - start) * value));
        }
    }
}