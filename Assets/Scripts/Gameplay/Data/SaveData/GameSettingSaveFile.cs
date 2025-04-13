using System;

namespace Mathlife.ProjectL.Gameplay
{
    [Serializable]
    public class GameSettingSaveFile : SaveFile
    {
        public bool drawTrajectory = true;
        public int masterVolume = 1;
        public int bgmVolume = 1;
        public int seVolume = 1;

        public override bool Equals(object obj)
        {
            if (obj is not GameSettingSaveFile other) return false;
            if (ReferenceEquals(this, other)) return true;
            return drawTrajectory == other.drawTrajectory
                   && masterVolume == other.masterVolume
                   && bgmVolume == other.bgmVolume
                   && seVolume == other.seVolume;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(drawTrajectory, masterVolume, bgmVolume, seVolume);
        }
    }
}