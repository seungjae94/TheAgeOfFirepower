using System;

namespace Mathlife.ProjectL.Gameplay
{
    [Serializable]
    public class GameProgressSaveFile : SaveFile
    {
        public int unlockWorld = 1;
        public int unlockStage = 1;
        public string userName = "";
        
        public override bool Equals(object obj)
        {
            if (obj is not GameProgressSaveFile other) return false;
            if (ReferenceEquals(this, other)) return true;
            return unlockWorld == other.unlockWorld && unlockStage == other.unlockStage;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(unlockWorld, unlockStage, userName);
        }
    }
}