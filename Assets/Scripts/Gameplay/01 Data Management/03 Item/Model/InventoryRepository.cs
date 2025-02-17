using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;

namespace Mathlife.ProjectL.Gameplay
{
    public class InventoryRepository : AbstractRepository
    {
        public InventoryRepository(RuntimeDB runtimeDB, GameDataDB gameDataDB, SaveDataDB saveDataDB) : base(runtimeDB, gameDataDB, saveDataDB)
        {
            InitEquipmentDictionary();

            if (m_saveDataDB.DoesSaveFileExist())
            {
                ConstructFromSaveFile();
            }
            else
            {
                ConstructFromStarterData();
            }
        }

        void ConstructFromSaveFile()
        {
            foreach (EEquipmentId equipmentId in m_saveDataDB.inventory.equipments)
            {
                AddEquipment(equipmentId);
            }
        }

        void ConstructFromStarterData()
        {
            StarterDataAsset starter = m_gameDataDB.GetStarterData();

            var starterParty = starter.GetStarterParty();
            var starterCharactersNotInParty = starter.GetStarterCharactersNotInParty();
            var starterEquipmentsNotOwned = starter.GetStarterEquipmentsNotOwned();

            foreach (var characterSlot in starterParty)
            {
                AddEquipment(characterSlot.weapon?.id ?? EEquipmentId.None);
                AddEquipment(characterSlot.armor?.id ?? EEquipmentId.None);
                AddEquipment(characterSlot.artifact?.id ?? EEquipmentId.None);
            }

            foreach (var characterSlot in starterCharactersNotInParty)
            {
                AddEquipment(characterSlot.weapon?.id ?? EEquipmentId.None);
                AddEquipment(characterSlot.armor?.id ?? EEquipmentId.None);
                AddEquipment(characterSlot.artifact?.id ?? EEquipmentId.None);
            }

            foreach (var equipSlot in starterEquipmentsNotOwned)
            {
                for (int i = 0; i < equipSlot.count; ++i)
                {
                    AddEquipment(equipSlot.equipment.id);
                }
            }
        }

        // 골드
        LongReactiveProperty m_gold = new(0L);
        public long gold { get => m_gold.Value; private set => m_gold.Value = value; }

        public void GainGold(int gain)
        {
            if (gain <= 0L)
                return;

            gold += gain;
        }

        public void LoseGold(int lose)
        {
            if (lose <= 0L)
                return;

            gold -= lose;
        }

        public IDisposable SubscribeGoldChange(Action<long> action)
        {
            return m_gold.Subscribe(action);
        }

        // 장비
        ReactiveDictionary<EEquipmentType, List<EquipmentModel>> m_equipments = new();

        void InitEquipmentDictionary()
        {
            foreach (var type in Enum.GetValues(typeof(EEquipmentType)))
            {
                m_equipments.Add((EEquipmentType)type, new());
            }
        }

        public List<EquipmentModel> GetSortedEquipmentList(EEquipmentType type)
        {
            SortEquipmentList(type);
            return m_equipments[type];
        }

        public EquipmentModel FindEquipment(EEquipmentType type, EEquipmentId id)
        {
            EquipmentModel equipment = m_equipments[type].FirstOrDefault(eq => eq.id == id);
            if (equipment == null)
                return AddEquipment(id);
            return equipment;
        }

        public EquipmentModel AddEquipment(EEquipmentId id)
        {
            if (id == EEquipmentId.None)
                return null;

            EquipmentModel equipment = new(m_gameDataDB.GetEquipmentSO(id));
            m_equipments[equipment.type].Add(equipment);
            return equipment;
        }

        void SortEquipmentList(EEquipmentType type)
        {
            m_equipments[type] = m_equipments[type]
                .OrderBy(equip =>
                {
                    if (equip.owner != null)
                        return equip.owner.characterId;
                    return ECharacterId.Max;
                })
                .ThenBy(equip => equip.id)
                .ToList();
        }

        //public bool RemoveArtifact(EEquipmentId id)
        //{
        //    EquipmentModel artifact = m_artifacts.Where(artifact =>
        //    {
        //        return artifact.artifactId == id && artifact.owner == null;
        //    }).First();

        //    if (artifact == null)
        //    {
        //        return false;
        //    }

        //    m_artifacts.Remove(artifact);
        //    return true;
        //}
    }
}
