using System;
using System.Collections.Generic;
using UnityEngine;

namespace Mathlife.ProjectL.Gameplay
{
    [Serializable]
    public class CharacterSaveData
    {
        public ECharacterId id = ECharacterId.None;
        public int level = 1;
        public int totalExp = 0;

        public EEquipmentId weapon;
        public EEquipmentId armor;
        public EEquipmentId artifact;
    }

    [Serializable]
    public class TeamSaveData : IHealthCheckable
    {
        public ECharacterId leader = ECharacterId.None;
        public List<ECharacterId> members = new();

        public bool IsHealthy()
        {
            int memberCount = 0;
            foreach (ECharacterId memberId in members)
            {
                if (memberId == ECharacterId.None)
                    continue;
                ++memberCount;
            }

            if (memberCount == 0)
                return false;

            if (leader == ECharacterId.None)
                return false;

            return true;
        }
    }

    [Serializable]
    public class CharacterSaveFile : SaveFile
    {
        public TeamSaveData team = new();
        public List<CharacterSaveData> characters = new();
    }
}