using Cysharp.Threading.Tasks;

namespace Mathlife.ProjectL.Gameplay
{
    public class GameState : MonoSingleton<GameState>, IState
    {
        public readonly GameDataLoader GameDataLoader = new();
        public readonly SaveDataManager SaveDataManager = new();

        public readonly CharacterRosterState CharacterRosterState = new();
        public readonly InventoryState InventoryState = new();

        public async UniTask Load()
        {
            await GameDataLoader.Load();
            await SaveDataManager.Load();

            await InventoryState.Load();
            await CharacterRosterState.Load();
        }

        public async UniTask Save()
        {
            await InventoryState.Save();
            await CharacterRosterState.Save();
        }
    }
}
