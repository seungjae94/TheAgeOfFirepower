using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Mathlife.ProjectL.Gameplay.UI;
using Sirenix.Utilities;
using UnityEngine;

namespace Mathlife.ProjectL.Gameplay.ObjectBase
{
    public interface IGameMode
    {
        public UniTask InitializeScene(IProgress<float> progress);

        public UniTask ClearScene(IProgress<float> progress);

        public UniTask StartScene();
    }

    /// <typeparam name="TGameMode">게임 모드 타입 (ex: TitleSceneGameMode)</typeparam>
    /// <summary>
    /// 게임 모드는 게임 초기화 및 씬 초기화를 담당한다.
    /// </summary>
    /// <example>
    /// 타이틀 씬에서 게임을 시작한다고 가정하자. <br/>
    /// 타이틀 씬 게임 모드는 아래와 같은 순서로 게임을 초기화한다. <br/>
    /// <list type="number">
    /// <item> PreInitializeGame </item>
    /// <item> GameManager.InitializeGame </item>
    /// <item> PostInitializeGame </item>
    /// </list>
    /// 다음으로 타이틀 씬 게임 모드는 InitializeScene 함수를 호출해 타이틀 씬을 초기화한다. <br/><br/>
    /// 로비 씬으로 이동하면 로비 씬 게임 모드는 InitializeScene 함수를 호출해 로비 씬을 초기화한다. 
    /// </example>
    public abstract class GameMode<TGameMode> : MonoSingleton<TGameMode>, IGameMode
        where TGameMode : GameMode<TGameMode>
    {
        public virtual async UniTask InitializeScene(IProgress<float> progress)
        {
            DisplayManager adapter = GameObject.FindAnyObjectByType<DisplayManager>();
            if (adapter != null)
            {
                await adapter.Adapt();
            }
            
            IEnumerable<ISceneBehaviour> initializables = GameObject
                .FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Include, FindObjectsSortMode.InstanceID)
                .OfType<ISceneBehaviour>();

            initializables.ForEach(initializable => initializable.OnSceneInitialize());
        }

        // Initialize Scene은 LoadingScreen이 떠있는 동안 뒤에서 몰래 씬을 초기화
        // Start Scene은 LoadingScreen이 사라지고 나서 씬을 초기화
        public virtual UniTask StartScene()
        {
            return UniTask.CompletedTask;
        }

        public virtual UniTask ClearScene(IProgress<float> progress)
        {
            Presenter.Clear();
            
            IEnumerable<ISceneBehaviour> clearables = GameObject
                .FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Include, FindObjectsSortMode.InstanceID)
                .OfType<ISceneBehaviour>();

            clearables.ForEach(clearable => clearable.OnSceneClear());
            
            return UniTask.CompletedTask;
        }
    }
}