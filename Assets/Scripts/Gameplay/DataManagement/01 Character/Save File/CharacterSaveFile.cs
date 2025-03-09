using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Mathlife.ProjectL.Gameplay
{
    [Serializable]
    public class CharacterSaveData
    {
        public int id = -1;
        public int level = 1;
        public int totalExp = 0;

        public int weaponId;
        public int armorId;
        public int artifactId;
    }

    [Serializable]
    public class PartySaveData : IHealthCheckable
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
    public class CharacterSaveFile : SaveFile
    {
        public PartySaveData party = new();
        public List<CharacterSaveData> characters = new();
    }
}