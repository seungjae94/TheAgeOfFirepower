using Cysharp.Threading.Tasks;
using Mathlife.ProjectL.Gameplay.ObjectBase;

namespace Mathlife.ProjectL.Gameplay
{
    public class GameState : MonoSingleton<GameState>, IPersistable
    {
        protected override SingletonLifeTime LifeTime => SingletonLifeTime.App;
        
        public readonly GameDataLoader gameDataLoader = new();
        public readonly SaveDataManager saveDataManager = new();

        public readonly ArtyRosterState artyRosterState = new();
        public readonly InventoryState inventoryState = new();
        public readonly BattleState battleState = new();

        public async UniTask Load()
        {
            await gameDataLoader.Load();
            await saveDataManager.Load();

            await inventoryState.Load();
            await artyRosterState.Load();
        }

        public async UniTask Save()
        {
            await inventoryState.Save();
            await artyRosterState.Save();
        }
    }
}
