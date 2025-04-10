using Cysharp.Threading.Tasks;
using Mathlife.ProjectL.Gameplay.Gameplay.Data.Model;
using Mathlife.ProjectL.Gameplay.ObjectBase;

namespace Mathlife.ProjectL.Gameplay
{
    public class GameState : MonoSingleton<GameState>
    {
        protected override SingletonLifeTime LifeTime => SingletonLifeTime.App;
        
        public readonly GameDataLoader gameDataLoader = new();
        public readonly SaveDataManager saveDataManager = new();

        // Persistable
        public readonly ArtyRosterState artyRosterState = new();
        public readonly InventoryState inventoryState = new();
        public readonly GameProgressState gameProgressState = new();

        // Non-persistable
        public readonly BattleState battleState = new();
        private SaveFile snapShot;

        public async UniTask Load()
        {
            await gameDataLoader.Load();
            await saveDataManager.Load();

            await gameProgressState.Load();
            await inventoryState.Load();
            await artyRosterState.Load();
        }

        public async UniTask Save()
        {
            await inventoryState.Save();
            await artyRosterState.Save();
            await gameProgressState.Save();
        }
    }
}
