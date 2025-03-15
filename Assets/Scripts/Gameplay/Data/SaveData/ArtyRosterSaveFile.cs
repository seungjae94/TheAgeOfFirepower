using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Mathlife.ProjectL.Gameplay
{
    [Serializable]
    public class ArtySaveData
    {
        public int id = -1;
        public int level = 1;
        public int totalExp = 0;

        public int weaponId;
        public int armorId;
        public int artifactId;
    }

    [Serializable]
    public class BatterySaveData : IHealthCheckable
    {
        public List<int> members = new();

        public bool IsHealthy()
        {
            int memberCount = 0;
            foreach (int memberId in members)
            {
                if (memberId < 0)
                    continue;
                ++memberCount;
            }

            if (memberCount == 0)
                return false;

            return true;
        }
    }

    [Serializable]
    public class ArtyRosterSaveFile : SaveFile
    {
        public BatterySaveData battery = new();
        public List<ArtySaveData> artyRoster = new();
    }
}