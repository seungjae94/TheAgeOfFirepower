using System;
using System.Collections.Generic;
using System.Linq;

namespace Mathlife.ProjectL.Gameplay
{
    [Serializable]
    public class ArtySaveData
    {
        public int artyId = -1;
        public int level = 1;
        public long totalExp = 0;

        public int barrelId;
        public int armorId;
        public int engineId;

        public override bool Equals(object obj)
        {
            if (obj is not ArtySaveData other) return false;
            if (ReferenceEquals(this, other)) return true;
            return artyId == other.artyId && level == other.level && totalExp == other.totalExp && barrelId == other.barrelId && armorId == other.armorId && engineId == other.engineId;
        }
        
        public override int GetHashCode()
        {
            return HashCode.Combine(artyId, level, totalExp, barrelId, armorId, engineId);
        }
    }

    [Serializable]
    public class BatterySaveData : IHealthCheckable
    {
        public List<int> memberIndexes = new();

        public bool IsHealthy()
        {
            int memberCount = 0;
            foreach (int memberIndex in memberIndexes)
            {
                if (memberIndex < 0)
                    continue;
                ++memberCount;
            }

            if (memberCount == 0)
                return false;

            return true;
        }
        
        public override bool Equals(object obj)
        {
            if (obj is not BatterySaveData other) return false;
            if (ReferenceEquals(this, other)) return true;
            return memberIndexes.SequenceEqual(other.memberIndexes);
        }
        
        public override int GetHashCode()
        {
            var hash = new HashCode();
            foreach (int index in memberIndexes)
            {
                hash.Add(index);
            }
            return hash.ToHashCode();
        }
    }

    [Serializable]
    public class ArtyRosterSaveFile : SaveFile
    {
        public BatterySaveData battery = new();
        public List<ArtySaveData> artyRoster = new();
        
        public override bool Equals(object obj)
        {
            if (obj is not ArtyRosterSaveFile other) return false;
            if (ReferenceEquals(this, other)) return true;
            return battery.Equals(other.battery) && artyRoster.SequenceEqual(other.artyRoster);
        }
        
        public override int GetHashCode()
        {
            return HashCode.Combine(battery, artyRoster);
        }
    }
}