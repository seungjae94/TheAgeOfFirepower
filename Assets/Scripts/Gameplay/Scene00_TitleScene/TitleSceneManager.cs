using Cysharp.Threading.Tasks;
using Mathlife.ProjectL.Utils;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Mathlife.ProjectL.Gameplay
{
    public class TitleSceneManager : MonoSingleton<TitleSceneManager>
    {
        [FormerlySerializedAs("m_gameStartButton")] [SerializeField] Button gameStartButton;
        CanvasGroup gameStartButtonCanvasGroup;

        [SerializeField] GameState gameState;

        void Awake()
        {
            // GameDataLoader gameDataLoader = new();
            // SaveDataManager saveDataManager = new();
            // m_appScope.gameDataDB = gameDataLoader;
            // m_appScope.saveDataDB = saveDataManager;
            //
            // gameState = new(gameDataLoader, saveDataManager);

            gameStartButtonCanvasGroup = gameStartButton.GetComponent<CanvasGroup>();
        }

        void Start()
        {
            InitializeScene().Forget();
        }

        private async UniTaskVoid InitializeScene()
        {
            gameStartButton.OnClickAsObservable()
                .Subscribe(_ => OnClickGameStartButton())
                .AddTo(gameObject);

            gameStartButtonCanvasGroup.Hide();
            await gameState.Load();
            await UniTask.SwitchToMainThread();
            gameStartButtonCanvasGroup.Show();
        }

        private void OnClickGameStartButton()
        {
            SceneManager.LoadScene("MainScene");
        }
    }
}
