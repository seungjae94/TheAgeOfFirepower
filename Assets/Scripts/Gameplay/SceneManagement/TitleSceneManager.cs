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

        // 인스턴스
        GameState gameState;

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

        async UniTaskVoid Start()
        {
            gameStartButton.OnClickAsObservable()
                .Subscribe(_ => OnClickGameStartButton());

            gameStartButtonCanvasGroup.Hide();

            await gameState.Load();

            gameStartButtonCanvasGroup.Show();
        }

        private void OnClickGameStartButton()
        {
            SceneManager.LoadScene("MainScene");
        }
    }
}
