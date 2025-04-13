using System;
using Cysharp.Threading.Tasks;
using Mathlife.ProjectL.Gameplay.Gameplay.Data.Model;
using Mathlife.ProjectL.Gameplay.ObjectBase;
using UnityEngine;

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

        public async UniTask Load(IProgress<float> progress)
        {
            await gameDataLoader.Load();
            progress.Report(0.4f);
            
            await saveDataManager.Load();
            progress.Report(0.8f);

            await gameProgressState.Load();
            await inventoryState.Load();
            await artyRosterState.Load();
            progress.Report(1f);
        }

        public void Save()
        {
            inventoryState.Save();
            artyRosterState.Save();
            gameProgressState.Save();
        }

        private void OnApplicationQuit()
        {
            
        }
    }
}
