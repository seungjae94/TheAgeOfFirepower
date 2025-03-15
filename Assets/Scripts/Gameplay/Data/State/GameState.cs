using Cysharp.Threading.Tasks;
using Mathlife.ProjectL.Gameplay.ObjectBase;

namespace Mathlife.ProjectL.Gameplay
{
    public class GameState : MonoSingleton<GameState>, IPersistable
    {
        protected override SingletonLifeTime LifeTime => SingletonLifeTime.App;
        
        public readonly GameDataLoader GameDataLoader = new();
        public readonly SaveDataManager SaveDataManager = new();

        public readonly ArtyRosterState artyRosterState = new();
        public readonly InventoryState InventoryState = new();

        public async UniTask Load()
        {
            await GameDataLoader.Load();
            await SaveDataManager.Load();

            await InventoryState.Load();
            await artyRosterState.Load();
        }

        public async UniTask Save()
        {
            await InventoryState.Save();
            await artyRosterState.Save();
        }
    }
}
