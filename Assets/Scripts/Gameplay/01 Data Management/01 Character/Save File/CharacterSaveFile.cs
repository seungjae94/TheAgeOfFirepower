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
    public class PartySaveData : IHealthCheckable
    {
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