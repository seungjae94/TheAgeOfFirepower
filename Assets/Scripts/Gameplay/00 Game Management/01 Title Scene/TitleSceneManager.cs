using Cysharp.Threading.Tasks;
using Mathlife.ProjectL.Utils;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using VContainer;

namespace Mathlife.ProjectL.Gameplay
{
    public class TitleSceneManager : MonoBehaviour
    {
        [SerializeField] Button m_gameStartButton;
        CanvasGroup m_gameStartButtonCanvasGroup;

        [SerializeField] AppLifetimeScope m_appScope;

        // 인스턴스
        RuntimeDB m_runtimeDB;

        public bool isGameLoaded { get; private set; } = false;

        void Awake()
        {
            GameDataDB gameDataDB = new();
            SaveDataDB saveDataDB = new();
            m_appScope.gameDataDB = gameDataDB;
            m_appScope.saveDataDB = saveDataDB;

            m_runtimeDB = new(gameDataDB, saveDataDB);

            m_gameStartButtonCanvasGroup = m_gameStartButton.GetComponent<CanvasGroup>();
        }

        async void Start()
        {
            m_gameStartButton.OnClickAsObservable()
                .Subscribe(_ => OnClickGameStartButton());

            m_gameStartButtonCanvasGroup.Hide();

            await LoadGame();

            m_gameStartButtonCanvasGroup.Show();
        }

        public void OnClickGameStartButton()
        {
            SceneManager.LoadScene("Main Scene");
        }

        async UniTask LoadGame()
        {
            // Build Repositories
            await m_runtimeDB.Build();

            // Register Repositories
            m_appScope.characterRepository = m_runtimeDB.characterRepository;
            m_appScope.inventoryRepository = m_runtimeDB.inventoryRepository;

            // Build Container
            m_appScope.Build();
        }
    }
}
