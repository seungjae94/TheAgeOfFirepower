using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;

namespace Mathlife.ProjectL.Gameplay
{
    public class InventoryState : IState
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
            gold = saveDataManager.inventory.gold;

            foreach (int equipmentId in saveDataManager.inventory.mechParts)
            {
                AddEquipment(equipmentId);
            }
        }

        void LoadFromStarterData()
        {
            StarterGameData starter = gameDataLoader.GetStarterSO();

            gold = starter.GetStarterGold();

            var starterParty = starter.GetStarterParty();
            foreach (var characterSlot in starterParty)
            {
                AddEquipment(characterSlot.weapon?.id ?? -1);
                AddEquipment(characterSlot.armor?.id ?? -1);
                AddEquipment(characterSlot.artifact?.id ?? -1);
            }

            var starterCharactersNotInParty = starter.GetStarterMechParts();
            foreach (var characterSlot in starterCharactersNotInParty)
            {
                AddEquipment(characterSlot.weapon?.id ?? -1);
                AddEquipment(characterSlot.armor?.id ?? -1);
                AddEquipment(characterSlot.artifact?.id ?? -1);
            }

            var starterEquipmentsNotOwned = starter.GetStarterRoster();
            foreach (var equipSlot in starterEquipmentsNotOwned)
            {
                for (int i = 0; i < equipSlot.count; ++i)
                {
                    AddEquipment(equipSlot.mechPart.id);
                }
            }
        }

        // 골드
        LongReactiveProperty m_gold = new(0L);
        public long gold { get => m_gold.Value; private set => m_gold.Value = value; }

        public void GainGold(long gain)
        {
            if (gain <= 0L)
                return;

            gold += gain;
        }

        public void LoseGold(long lose)
        {
            if (lose <= 0L)
                return;

            gold -= lose;
        }

        public IDisposable SubscribeGoldChange(Action<long> action)
        {
            return m_gold.Subscribe(action);
        }

        // 거래

        public bool BuyItem(int equipmentId)
        {
            MechPartGameData mechPartGameData = gameDataLoader.GetEquipmentSO(equipmentId);
        
            if (gold < mechPartGameData.shopPrice)
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

        public EquipmentModel FindEquipment(EEquipmentType type, int id)
        {
            EquipmentModel equipment = m_equipments[type].FirstOrDefault(eq => eq.id == id);
            if (equipment == null)
                return AddEquipment(id);
            return equipment;
        }

        public EquipmentModel AddEquipment(int id)
        {
            if (id < 0)
                return null;

            Debug.Log(id);
            EquipmentModel equipment = new(gameDataLoader.GetEquipmentSO(id));
            m_equipments[equipment.type].Add(equipment);
            return equipment;
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
