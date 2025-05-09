using System;
using UnityEngine;

namespace Mathlife.ProjectL.Gameplay
{
    [Serializable]
    public class GameSettingSaveFile : SaveFile
    {
        public int resolutionOptionIndex = 0;
        public bool drawTrajectory = true;
        public float bgmVolume = 1;
        public float seVolume = 1;

        public override bool Equals(object obj)
        {
            if (obj is not GameSettingSaveFile other) return false;
            if (ReferenceEquals(this, other)) return true;
            return resolutionOptionIndex == other.resolutionOptionIndex 
                   && drawTrajectory == other.drawTrajectory
                   && Mathf.Approximately(bgmVolume, other.bgmVolume)
                   && Mathf.Approximately(seVolume, other.seVolume);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(resolutionOptionIndex, drawTrajectory, bgmVolume, seVolume);
        }
    }
}