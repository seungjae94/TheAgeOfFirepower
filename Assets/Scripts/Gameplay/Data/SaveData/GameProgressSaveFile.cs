using System;

namespace Mathlife.ProjectL.Gameplay
{
    [Serializable]
    public class GameProgressSaveFile : SaveFile
    {
        public int unlockWorld = 1;
        public int unlockStage = 1;
    }
}