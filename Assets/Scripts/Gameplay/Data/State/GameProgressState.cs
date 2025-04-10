using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;

namespace Mathlife.ProjectL.Gameplay.Gameplay.Data.Model
{
    public class GameProgressState : PersistableStateBase
    {
        // Props
        public ReactiveProperty<int> unlockWorldRx = new(1);
        public ReactiveProperty<int> unlockStageRx = new(1);
        
        public override UniTask Load()
        {
            if (GameState.Inst.saveDataManager.DoesSaveFileExist() && GameSettings.Inst.UseSaveFileIfAvailable)
            {
                var saveFile = SaveDataManager.gameProgress;
                unlockWorldRx.Value = saveFile.unlockWorld;
                unlockStageRx.Value = saveFile.unlockStage;
                return UniTask.CompletedTask;
            }

            var starterData = GameDataLoader.GetStarterData();
            unlockWorldRx.Value = starterData.GetStarterUnlockWorldNo();
            unlockStageRx.Value = starterData.GetStarterUnlockStageNo();
            return UniTask.CompletedTask;
        }

        protected override SaveFile SavedFile => SaveDataManager.gameProgress;
        protected override SaveFile TakeSnapShot()
        {
            return new GameProgressSaveFile
            {
                unlockWorld = unlockWorldRx.Value,
                unlockStage = unlockStageRx.Value
            };
        }
    }
}