using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniRx;
using UnityEditor.Experimental;
using UnityEditor.Overlays;

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

#if UNITY_EDITOR
            var starterMembers = starter.editorStarterMembers;
            var starterNonMemberCharacters = starter.editorStarterNonMemberCharacters;
            var starterUnequippedEquipments = starter.editorStarterUnequippedEquipments;
#else
            var starterUnequippedEquipments = starter.starterUnequippedEquipments;
            var starterMembers = starter.starterMembers;
            var starterNonMemberCharacters = starter.starterNonMemberCharacters;
#endif

            foreach (var characterSlot in starterMembers)
            {
                AddEquipment(characterSlot.weapon?.id ?? EEquipmentId.None);
                AddEquipment(characterSlot.armor?.id ?? EEquipmentId.None);
                AddEquipment(characterSlot.artifact?.id ?? EEquipmentId.None);
            }

            foreach (var characterSlot in starterNonMemberCharacters)
            {
                AddEquipment(characterSlot.weapon?.id ?? EEquipmentId.None);
                AddEquipment(characterSlot.armor?.id ?? EEquipmentId.None);
                AddEquipment(characterSlot.artifact?.id ?? EEquipmentId.None);
            }

            foreach (var equipSlot in starterUnequippedEquipments)
            {
                for (int i = 0; i < equipSlot.count; ++i)
                {
                    AddEquipment(equipSlot.equipment.id);
                }
            }
        }

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

            //m_equipments[type].Sort((eq1, eq2) => eq1.id - eq2.id);
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
