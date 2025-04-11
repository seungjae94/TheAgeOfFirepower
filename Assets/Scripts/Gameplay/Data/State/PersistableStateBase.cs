using Cysharp.Threading.Tasks;

namespace Mathlife.ProjectL.Gameplay
{
    public abstract class PersistableStateBase
    {
        // Alias
        protected SaveDataManager SaveDataManager => GameState.Inst.saveDataManager;
        protected GameDataLoader GameDataLoader => GameState.Inst.gameDataLoader;

        // Methods
        public abstract UniTask Load();

        public void Save()
        {
            SaveFile snapShot = TakeSnapShot();

            if (SavedFile?.Equals(snapShot) ?? false)
                return;

            SaveDataManager.Save(snapShot).Forget();
        }

        protected abstract SaveFile SavedFile { get; }

        protected abstract SaveFile TakeSnapShot();
    }
}