using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mathlife.ProjectL.Gameplay
{
    public abstract class AbstractRepository
    {
        protected RuntimeDB m_runtimeDB;
        protected GameDataDB m_gameDataDB;
        protected SaveDataDB m_saveDataDB;

        public AbstractRepository(RuntimeDB runtimeDB, GameDataDB gameDataDB, SaveDataDB saveDataDB)
        {
            m_runtimeDB = runtimeDB;
            m_gameDataDB = gameDataDB;
            m_saveDataDB = saveDataDB;
        }
    }
}
