using System;
using System.Diagnostics;
using System.Linq;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Mathlife.ProjectL.Gameplay.ObjectBase;
using Mathlife.ProjectL.Gameplay.UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using Debug = UnityEngine.Debug;

namespace Mathlife.ProjectL.Gameplay
{
    public class GameManager : MonoSingleton<GameManager>
    {
        // 상수
        // - 실제로 소요 시간을 측정해보고 설정한 값
        private const float INIT_GAME_TIME = 8f;            
        private const float SCENE_CLEAR_TIME = 2f;          
        private const float SCENE_UNLOAD_TIME = 2f;            
        private const float SCENE_LOAD_TIME = 5f;           
        private const float SCENE_INIT_TIME = 39f;
        private const float DUMMY_TIME = 1f;
        private const float GAME_START_TIME = SCENE_LOAD_TIME + INIT_GAME_TIME + SCENE_INIT_TIME;
        private const float CHANGE_SCENE_TIME = SCENE_CLEAR_TIME + SCENE_UNLOAD_TIME + SCENE_LOAD_TIME + SCENE_INIT_TIME + DUMMY_TIME;

        // 필드
        protected override SingletonLifeTime LifeTime => SingletonLifeTime.App;

        public string PrevSceneName { get; private set; } = null;
        private Scene currentScene;
        public IGameMode CurrentGameMode { get; private set; }
        public IMainCanvas CurrentCanvas { get; private set; }

        public Cameras CurrentCameras { get; private set; }

        private void Start()
        {
            Application.targetFrameRate = 30;
            GameStart().Forget();
        }

        private void RefreshCurrentSceneAndGameMode()
        {
            if (SceneManager.loadedSceneCount == 1)
            {
                Debug.LogError("[GameManager] Failed to refresh scene data. Current scene is null.");
                return;
            }
            
            PrevSceneName = currentScene.name;
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
            }
        }

        private async UniTask GameStart()
        {
            // AppScope 씬만 로딩되어 있는 경우 타이틀 씬을 로드한다.
            // - 눈에 보이는 씬이 없는 상태로 게임을 초기화하면 사용자 경험을 해칠 수 있다. 
            float currentProgress = 0f;
            if (SceneManager.loadedSceneCount == 1)
            {
                await SceneManager.LoadSceneAsync(SceneNames.TitleScene, LoadSceneMode.Additive)
                    .ToUniTask(CreateProgress(0f, SCENE_LOAD_TIME / GAME_START_TIME));
                currentProgress += SCENE_LOAD_TIME / GAME_START_TIME;
            }

            // 씬 데이터 갱신
            RefreshCurrentSceneAndGameMode();

            // 게임 초기화
            DOTween.Init();
            await GameState.Inst.Load(CreateProgress(currentProgress, currentProgress + INIT_GAME_TIME / GAME_START_TIME));
            currentProgress += INIT_GAME_TIME / GAME_START_TIME;
            
            // 씬 초기화
            await CurrentGameMode.InitializeScene(CreateProgress(currentProgress, 1f));
        }

        public async UniTask ChangeScene(string sceneName)
        {
            LoadingScreenOn();

            Stopwatch watch = new();
            watch.Start();

            // 기존 씬 정리
            float currentProgress = 0f;
            LoadingScreenManager.Inst.SetMessage("기존 씬을 정리하는 중...");
            await CurrentGameMode.ClearScene(CreateProgress(currentProgress, currentProgress + SCENE_CLEAR_TIME / CHANGE_SCENE_TIME));
            currentProgress += SCENE_CLEAR_TIME / CHANGE_SCENE_TIME;
            
            // 기존 씬 언로드
            LoadingScreenManager.Inst.SetMessage("기존 씬을 언로딩하는 중...");
            await SceneManager.UnloadSceneAsync(currentScene)
                .ToUniTask(CreateProgress(currentProgress, currentProgress + SCENE_UNLOAD_TIME / CHANGE_SCENE_TIME));
            currentProgress += SCENE_UNLOAD_TIME / CHANGE_SCENE_TIME;
            
            // 새로운 씬 로드
            LoadingScreenManager.Inst.SetMessage("새로운 씬을 로딩하는 중...");
            await SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive)
                .ToUniTask(CreateProgress(currentProgress, currentProgress + SCENE_LOAD_TIME / CHANGE_SCENE_TIME));
            currentProgress += SCENE_LOAD_TIME / CHANGE_SCENE_TIME;
            
            // 씬 데이터 업데이트
            RefreshCurrentSceneAndGameMode();

            // 새로운 씬 초기화
            LoadingScreenManager.Inst.SetMessage("새로운 씬을 초기화하는 중...");
            await CurrentGameMode.InitializeScene(CreateProgress(currentProgress, currentProgress + SCENE_INIT_TIME / CHANGE_SCENE_TIME));
            
            // 사용자 경험을 위해 로딩 시간이 최소 1초 이상이 되도록 설정
            watch.Stop();
            int elapsedMs = (int)watch.ElapsedMilliseconds;
            if (elapsedMs < 1000)
            {
                await UniTask.Delay(1000 - elapsedMs);
            }
            LoadingScreenManager.Inst.SetProgress(1f);
            
            LoadingScreenOff();

            CurrentGameMode.StartScene().Forget();
        }

        private IProgress<float> CreateProgress(float start, float end)
        {
            return Progress.Create<float>(value =>
                LoadingScreenManager.Inst.SetProgress(Mathf.Lerp(start, end, value)));
        }

        private void LoadingScreenOn()
        {
            LoadingScreenManager.Inst.Show();
            LoadingScreenManager.Inst.SetProgress(0f);
            AudioManager.Inst.MuteBGM();
            AudioManager.Inst.PauseBGM();
        }
        
        private void LoadingScreenOff()
        {
            LoadingScreenManager.Inst.Hide();
            AudioManager.Inst.StopAllSE();
            AudioManager.Inst.UnmuteBGM();
            AudioManager.Inst.ReplayBGM();
        }
    }
}