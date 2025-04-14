using Cysharp.Threading.Tasks;
using UniRx;

namespace Mathlife.ProjectL.Gameplay.Gameplay.Data.Model
{
    public class GameSettingState : PersistableStateBase
    {
        // Props
        public ReactiveProperty<bool> drawTrajectory = new(true);
        public ReactiveProperty<float> bgmVolume = new(0.5f);
        public ReactiveProperty<float> seVolume = new(0.5f);
        
        public override UniTask Load()
        {
            if (GameState.Inst.saveDataManager.CanLoad() && DebugSettings.Inst.UseSaveFileIfAvailable)
            {
                LoadFromSaveFile();
            }
            else
            {
                LoadFromStarter();
            }
            
            AudioManager.Inst.SetBGMVolume(bgmVolume.Value);
            AudioManager.Inst.SetSEVolume(seVolume.Value);
            
            return UniTask.CompletedTask;
        }

        private void LoadFromSaveFile()
        {
            var saveFile = SaveDataManager.GameSetting;
            drawTrajectory.Value = saveFile.drawTrajectory;
            bgmVolume.Value = saveFile.bgmVolume;
            seVolume.Value = saveFile.seVolume;
        }

        private void LoadFromStarter()
        {
            drawTrajectory.Value = true;
            bgmVolume.Value = 0.5f;
            seVolume.Value = 0.5f;
        }

        protected override SaveFile SavedFile => SaveDataManager.GameSetting;
        
        protected override SaveFile TakeSnapShot()
        {
            return new GameSettingSaveFile
            {
                drawTrajectory = drawTrajectory.Value,
                bgmVolume = bgmVolume.Value,
                seVolume = seVolume.Value
            };
        }
    }
}