using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
        private List<PersistableStateBase> persistableStates = new();
        public readonly InventoryState inventoryState = new();
        public readonly ArtyRosterState artyRosterState = new();
        public readonly GameProgressState gameProgressState = new();
        public readonly GameSettingState gameSettingState = new();
        
        
        // Non-persistable
        public readonly BattleState battleState = new();
        private SaveFile snapShot;

        public async UniTask Load(IProgress<float> progress)
        {
            await gameDataLoader.Load();
            progress.Report(0.4f);
            
            await saveDataManager.Load();
            progress.Report(0.8f);
            
            foreach (FieldInfo field in typeof(GameState).GetFields().OrderBy(field => field.MetadataToken))
            {
                if (false == field.FieldType.IsSubclassOf(typeof(PersistableStateBase)))
                    continue;

                var state = (PersistableStateBase) field.GetValue(this);
                await state.Load();
                persistableStates.Add(state);
            }
            
            progress.Report(1f);
        }

        public void Save()
        {
            if (DebugSettings.Inst.UseSaveFileIfAvailable == false)
                return;
            
            foreach (var state in persistableStates)
            {
                state.Save();
            }
        }
    }
}
