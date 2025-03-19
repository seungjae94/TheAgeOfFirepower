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
        ReactiveDictionary<EMechPartType, List<MechPartModel>> m_equipments = new();

        void InitEquipmentDictionary()
        {
            foreach (var type in Enum.GetValues(typeof(EMechPartType)))
            {
                m_equipments.Add((EMechPartType)type, new());
            }
        }

        public List<MechPartModel> GetSortedEquipmentList(EMechPartType type)
        {
            SortEquipmentList(type);
            return m_equipments[type];
        }

        public MechPartModel FindEquipment(EMechPartType type, int id)
        {
            MechPartModel mechPart = m_equipments[type].FirstOrDefault(eq => eq.Id == id);
            if (mechPart == null)
                return AddEquipment(id);
            return mechPart;
        }

        public MechPartModel AddEquipment(int id)
        {
            if (id < 0)
                return null;
            
            MechPartModel mechPart = new(gameDataLoader.GetMechPartData(id));
            m_equipments[mechPart.Type].Add(mechPart);
            return mechPart;
        }

        void SortEquipmentList(EMechPartType type)
        {
            m_equipments[type] = m_equipments[type]
                .OrderBy(equip =>
                {
                    if (equip.Owner != null)
                        return equip.Owner.Value.Id;
                    return Int32.MaxValue;
                })
                .ThenBy(equip => equip.Id)
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
