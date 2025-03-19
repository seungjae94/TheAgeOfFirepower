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
        // Alias
        private SaveDataManager SaveDataManager => GameState.Inst.saveDataManager;
        private GameDataLoader GameDataLoader => GameState.Inst.gameDataLoader;

        public UniTask Load()
        {
            foreach (var type in Enum.GetValues(typeof(EMechPartType)))
            {
                mechPartInventory.Add((EMechPartType)type, new());
            }

            if (GameState.Inst.saveDataManager.DoesSaveFileExist())
            {
                LoadFromSaveFile();
            }
            else
            {
                LoadFromStarterData();
            }
            
            return UniTask.CompletedTask;
        }

        public UniTask Save()
        {
            throw new NotImplementedException();
        }

        void LoadFromSaveFile()
        {
            goldRx.Value = SaveDataManager.inventory.gold;

            foreach (int equipmentId in SaveDataManager.inventory.mechParts)
            {
                AddMechPart(equipmentId);
            }
        }

        void LoadFromStarterData()
        {
            StarterGameData starter = GameDataLoader.GetStarterData();

            goldRx.Value = starter.GetStarterGold();

            var starterParty = starter.GetStarterBattery();
            foreach (var batteryMemberPreset in starterParty)
            {
                AddMechPart((batteryMemberPreset.barrel != null) ? batteryMemberPreset.barrel.id : -1);
                AddMechPart((batteryMemberPreset.armor != null) ? batteryMemberPreset.armor.id : -1);
                AddMechPart((batteryMemberPreset.engine != null) ? batteryMemberPreset.engine.id : -1);
            }

            var starterCharactersNotInParty = starter.GetStarterBench();
            foreach (var rosterMemberPreset in starterCharactersNotInParty)
            {
                AddMechPart((rosterMemberPreset.barrel != null) ? rosterMemberPreset.barrel.id : -1);
                AddMechPart((rosterMemberPreset.armor != null) ? rosterMemberPreset.armor.id : -1);
                AddMechPart((rosterMemberPreset.engine != null) ? rosterMemberPreset.engine.id : -1);
            }

            var starterBackupMechParts = starter.GetStarterBackupMechParts();
            foreach (var mechPartStack in starterBackupMechParts)
            {
                for (int i = 0; i < mechPartStack.count; ++i)
                {
                    AddMechPart(mechPartStack.mechPart.id);
                }
            }
        }

        // 골드 (거래)
        public readonly ReactiveProperty<long> goldRx = new(0L);

        public void GainGold(long gain)
        {
            if (gain <= 0L)
                return;

            goldRx.Value += gain;
        }

        public void LoseGold(long lose)
        {
            if (lose <= 0L)
                return;

            goldRx.Value -= lose;
        }
        
        public bool BuyMechPart(int mechPartId)
        {
            MechPartGameData mechPartGameData = GameDataLoader.GetMechPartData(mechPartId);
        
            if (goldRx.Value < mechPartGameData.shopPrice)
            {
                return false;
            }
        
            AddMechPart(mechPartId);
            LoseGold(mechPartGameData.shopPrice);
            return true;
        }

        public bool SellMechPart(int mechPartId)
        {
            return true;
        }

        // 부품
        private readonly ReactiveDictionary<EMechPartType, List<MechPartModel>> mechPartInventory = new();
        
        public List<MechPartModel> GetSortedEquipmentList(EMechPartType type)
        {
            SortMechPartList(type);
            return mechPartInventory[type];
        }

        public MechPartModel FindBackupMechPart(EMechPartType type, int id)
        {
            return mechPartInventory[type].FirstOrDefault(eq => eq.Id == id && eq.Owner.Value == null);
        }

        public MechPartModel AddMechPart(int mechPartId)
        {
            if (mechPartId < 0)
                return null;
            
            MechPartModel mechPart = new(GameDataLoader.GetMechPartData(mechPartId));
            mechPartInventory[mechPart.Type].Add(mechPart);
            return mechPart;
        }

        void SortMechPartList(EMechPartType type)
        {
            mechPartInventory[type] = mechPartInventory[type]
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
