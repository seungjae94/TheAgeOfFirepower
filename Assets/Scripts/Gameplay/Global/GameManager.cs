using Cysharp.Threading.Tasks;

namespace Mathlife.ProjectL.Gameplay
{
    public class GameManager : MonoSingleton<GameManager>
    {
        public bool IsGameInitialized { get; private set; }
        protected override SingletonLifeTime LifeTime => SingletonLifeTime.App;
        
        public async UniTask InitializeGame()
        {
            await GameState.Inst.Load();
            IsGameInitialized = true;
        }
        
        // ChangeScene
        
    }
}