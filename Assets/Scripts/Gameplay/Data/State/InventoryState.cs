using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;

namespace Mathlife.ProjectL.Gameplay
{
    public class InventoryState : IPersistable
    {
        // Access
        private SaveDataManager saveDataManager;
        private GameDataLoader gameDataLoader;
        
        public async UniTask Load()
        {
            saveDataManager = GameState.Inst.SaveDataManager;
            gameDataLoader = GameState.Inst.GameDataLoader;
            
            InitEquipmentDictionary();

            if (GameState.Inst.SaveDataManager.DoesSaveFileExist())
            {
                LoadFromSaveFile();
            }
            else
            {
                LoadFromStarterData();
            }
        }

        public async UniTask Save()
        {
            throw new NotImplementedException();
        }

        void LoadFromSaveFile()
        {
            GoldRx.Value = saveDataManager.inventory.gold;

            foreach (int equipmentId in saveDataManager.inventory.mechParts)
            {
                AddEquipment(equipmentId);
            }
        }

        void LoadFromStarterData()
        {
            StarterGameData starter = gameDataLoader.GetStarterData();

            GoldRx.Value = starter.GetStarterGold();

            var starterParty = starter.GetStarterBattery();
            foreach (var characterSlot in starterParty)
            {
                AddEquipment(characterSlot.barrel?.id ?? -1);
                AddEquipment(characterSlot.armor?.id ?? -1);
                AddEquipment(characterSlot.engine?.id ?? -1);
            }

            var starterCharactersNotInParty = starter.GetStarterRosterMinusBattery();
            foreach (var characterSlot in starterCharactersNotInParty)
            {
                AddEquipment(characterSlot.barrel?.id ?? -1);
                AddEquipment(characterSlot.armor?.id ?? -1);
                AddEquipment(characterSlot.engine?.id ?? -1);
            }

            var starterEquipmentsNotOwned = starter.GetStarterMechParts();
            foreach (var equipSlot in starterEquipmentsNotOwned)
            {
                for (int i = 0; i < equipSlot.count; ++i)
                {
                    AddEquipment(equipSlot.mechPart.id);
                }
            }
        }

        // 골드
        public readonly ReactiveProperty<long> GoldRx = new(0L);

        public void GainGold(long gain)
        {
            if (gain <= 0L)
                return;

            GoldRx.Value += gain;
        }

        public void LoseGold(long lose)
        {
            if (lose <= 0L)
                return;

            GoldRx.Value -= lose;
        }

        // 거래

        public bool BuyItem(int equipmentId)
        {
            MechPartGameData mechPartGameData = gameDataLoader.GetMechPartData(equipmentId);
        
            if (GoldRx.Value < mechPartGameData.shopPrice)
            {
                return false;
            }
        
            AddEquipment(equipmentId);
            LoseGold(mechPartGameData.shopPrice);
            return true;
        }

        public bool SellItem(int equipmentId)
        {
            return true;
        }

        // 장비
        ReactiveDictionary<EEquipmentType, List<MechPartModel>> m_equipments = new();

        void InitEquipmentDictionary()
        {
            foreach (var type in Enum.GetValues(typeof(EEquipmentType)))
            {
                m_equipments.Add((EEquipmentType)type, new());
            }
        }

        public List<MechPartModel> GetSortedEquipmentList(EEquipmentType type)
        {
            SortEquipmentList(type);
            return m_equipments[type];
        }

        public MechPartModel FindEquipment(EEquipmentType type, int id)
        {
            MechPartModel mechPart = m_equipments[type].FirstOrDefault(eq => eq.id == id);
            if (mechPart == null)
                return AddEquipment(id);
            return mechPart;
        }

        public MechPartModel AddEquipment(int id)
        {
            if (id < 0)
                return null;

            Debug.Log(id);
            MechPartModel mechPart = new(gameDataLoader.GetMechPartData(id));
            m_equipments[mechPart.type].Add(mechPart);
            return mechPart;
        }

        void SortEquipmentList(EEquipmentType type)
        {
            m_equipments[type] = m_equipments[type]
                .OrderBy(equip =>
                {
                    if (equip.owner != null)
                        return equip.owner.id;
                    return Int32.MaxValue;
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
