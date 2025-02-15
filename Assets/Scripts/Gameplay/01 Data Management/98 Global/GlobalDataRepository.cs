using Sirenix.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Mathlife.ProjectL.Gameplay
{
    public class GlobalDataRepository
    {
        GlobalDataAsset m_constantDataAsset;
        ExpDataAsset m_expDataAsset;
        StarterDataAsset m_starterDataAsset;

        public GlobalDataRepository(
            GlobalDataAsset constantDataAsset,
            ExpDataAsset expDataAsset,
            StarterDataAsset starterDataAsset
        )
        {
            m_constantDataAsset = constantDataAsset;
            m_expDataAsset = expDataAsset;
            m_starterDataAsset = starterDataAsset;
        }

        public int maxLevel => m_constantDataAsset.maxLevel;
        
        public long GetTotalExp(int level)
        {
            if (level < 0 || level >= m_expDataAsset.characterTotalExpAtLevelList.Count)
            {
                Debug.LogError("level < 0 or level > exp list length");
                return -1;
            }

            return m_expDataAsset.characterTotalExpAtLevelList[level];
        }
        
    }
}
