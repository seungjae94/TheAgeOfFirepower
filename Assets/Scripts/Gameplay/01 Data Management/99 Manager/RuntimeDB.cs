using Cysharp.Threading.Tasks;

namespace Mathlife.ProjectL.Gameplay
{
    public class RuntimeDB
    {
        GameDataDB m_gameDataDB;
        SaveDataDB m_saveDataDB;

        public RuntimeDB(GameDataDB gameDataDB, SaveDataDB saveDataDB)
        {
            m_gameDataDB = gameDataDB;
            m_saveDataDB = saveDataDB;
        }

        public CharacterRepository characterRepository;
        public InventoryRepository inventoryRepository;

        public async UniTask Build()
        {
            await m_gameDataDB.Load();

            if (m_saveDataDB.DoesSaveFileExist())
            {
                await m_saveDataDB.Load();
            }

            inventoryRepository = new(this, m_gameDataDB, m_saveDataDB);
            characterRepository = new(this, m_gameDataDB, m_saveDataDB);
        }

        public void Save()
        {
            // Create save files from models
            //CharacterListSaveFile saveFile;

            // Save progress files
            //m_fileSaveLoader.Save(saveFile);
        }
    }
}
