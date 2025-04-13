using Cysharp.Threading.Tasks;
using UniRx;

namespace Mathlife.ProjectL.Gameplay.Gameplay.Data.Model
{
    public class GameSettingState : PersistableStateBase
    {
        // Props
        public ReactiveProperty<bool> drawTrajectory = new(true);
        public ReactiveProperty<int> masterVolume = new(100);
        public ReactiveProperty<int> bgmVolume = new(50);
        public ReactiveProperty<int> seVolume = new(50);
        
        public override UniTask Load()
        {
            if (GameState.Inst.saveDataManager.CanLoad() && DebugSettings.Inst.UseSaveFileIfAvailable)
            {
                var saveFile = SaveDataManager.GameSetting;
                drawTrajectory.Value = saveFile.drawTrajectory;
                masterVolume.Value = saveFile.masterVolume;
                bgmVolume.Value = saveFile.bgmVolume;
                seVolume.Value = saveFile.seVolume;
            }
            
            return UniTask.CompletedTask;
        }

        protected override SaveFile SavedFile => SaveDataManager.GameSetting;
        
        protected override SaveFile TakeSnapShot()
        {
            return new GameSettingSaveFile
            {
                drawTrajectory = drawTrajectory.Value,
                masterVolume = masterVolume.Value,
                bgmVolume = bgmVolume.Value,
                seVolume = seVolume.Value
            };
        }
    }
}