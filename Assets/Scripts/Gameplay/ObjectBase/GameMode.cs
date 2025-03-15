using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Mathlife.ProjectL.Gameplay.UI;
using UnityEngine;

namespace Mathlife.ProjectL.Gameplay.ObjectBase
{
    public interface IGameMode
    {
        public UniTask PreInitializeGame();

        public UniTask PostInitializeGame();

        public UniTask InitializeScene(IProgress<float> progress);
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
        public virtual UniTask PreInitializeGame()
        {
            return UniTask.CompletedTask;
        }

        public virtual UniTask PostInitializeGame()
        {
            return UniTask.CompletedTask;
        }

        public virtual UniTask InitializeScene(IProgress<float> progress)
        {
            return UniTask.CompletedTask;
        }
    }
}