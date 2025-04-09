using Cysharp.Threading.Tasks;
using UniRx;

namespace Mathlife.ProjectL.Gameplay.Gameplay.Data.Model
{
    public class GameProgressState : IPersistable
    {
        // Alias
        private SaveDataManager SaveDataManager => GameState.Inst.saveDataManager;
        
        // Props
        public ReactiveProperty<int> unlockWorldRx = new(1);
        public ReactiveProperty<int> unlockStageRx = new(1);
        
        public UniTask Load()
        {
            var saveFile = SaveDataManager.gameProgress;
            unlockWorldRx.Value = saveFile.unlockWorld;
            unlockStageRx.Value = saveFile.unlockStage;
            return UniTask.CompletedTask;
        }

        public UniTask Save()
        {
            throw new System.NotImplementedException();
        }
    }
}