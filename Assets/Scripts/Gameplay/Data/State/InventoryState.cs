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

        private void LoadFromSaveFile()
        {
            goldRx.Value = SaveDataManager.inventory.gold;

            foreach (int equipmentId in SaveDataManager.inventory.mechParts)
            {
                AddMechPart(equipmentId);
            }

            foreach (var itemStack in SaveDataManager.inventory.materialItems)
            {
                AddCountableItemStack(EItemType.MaterialItem, itemStack.id, itemStack.amount);
            }
            
            foreach (var itemStack in SaveDataManager.inventory.battleItems)
            {
                AddCountableItemStack(EItemType.BattleItem, itemStack.id, itemStack.amount);
            }
        }

        private void LoadFromStarterData()
        {
            StarterGameData starter = GameDataLoader.GetStarterData();

            goldRx.Value = starter.GetStarterGold();
            
            foreach (var batteryMemberPreset in starter.GetStarterBattery())
            {
                AddMechPart((batteryMemberPreset.barrel != null) ? batteryMemberPreset.barrel.id : -1);
                AddMechPart((batteryMemberPreset.armor != null) ? batteryMemberPreset.armor.id : -1);
                AddMechPart((batteryMemberPreset.engine != null) ? batteryMemberPreset.engine.id : -1);
            }
            
            foreach (var rosterMemberPreset in starter.GetStarterBench())
            {
                AddMechPart((rosterMemberPreset.barrel != null) ? rosterMemberPreset.barrel.id : -1);
                AddMechPart((rosterMemberPreset.armor != null) ? rosterMemberPreset.armor.id : -1);
                AddMechPart((rosterMemberPreset.engine != null) ? rosterMemberPreset.engine.id : -1);
            }
            
            foreach (var mechPartStack in starter.GetStarterBackupMechParts())
            {
                for (int i = 0; i < mechPartStack.count; ++i)
                {
                    AddMechPart(mechPartStack.mechPart.id);
                }
            }

            foreach (var itemStack in starter.GetStarterItemStacks())
            {
                AddCountableItemStack(itemStack.item, itemStack.count);
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

        // 아이템 관리
        private readonly ReactiveDictionary<EMechPartType, List<MechPartModel>> mechPartInventory = new();
        private readonly ReactiveCollection<ItemStackModel> materialItemInventory = new();
        private readonly ReactiveCollection<ItemStackModel> battleItemInventory = new();

        public List<MechPartModel> GetSortedMechPartListOfType(EMechPartType type,
            Func<MechPartModel, bool> excludeFilter = null)
        {
            excludeFilter ??= (mechPart) => false;

            SortMechPartList(type);
            return mechPartInventory[type]
                .Where((mechPart) => excludeFilter(mechPart) == false)
                .ToList();
        }

        public List<MechPartModel> GetSortedMechPartList(Func<MechPartModel, bool> excludeFilter = null)
        {
            excludeFilter ??= (mechPart) => false;

            return mechPartInventory[EMechPartType.Barrel]
                .Concat(mechPartInventory[EMechPartType.Armor])
                .Concat(mechPartInventory[EMechPartType.Engine])
                .Where(mechPart => excludeFilter(mechPart) == false)
                .OrderBy(mechPart => (mechPart.Owner.Value != null) ? 0 : 1)
                .ThenByDescending(mechPart => mechPart.Rarity)
                .ThenBy(mechPart => mechPart.Type)
                .ThenBy(mechPart => mechPart.Id)
                .ToList();
        }

        public List<ItemStackModel> GetSortedItemStackList(EItemType itemType,
            Func<ItemStackModel, bool> excludeFilter = null)
        {
            excludeFilter ??= (itemStack) => false;

            if (itemType != EItemType.MaterialItem && itemType != EItemType.BattleItem)
                throw new ArgumentOutOfRangeException("Selected item type is not countable.");

            var targetInventory = (itemType == EItemType.MaterialItem) ? materialItemInventory : battleItemInventory;

            return targetInventory
                .Where(itemStack => excludeFilter(itemStack) == false)
                .OrderByDescending(itemStack => itemStack.Rarity)
                .ThenBy(itemStack => itemStack.Id)
                .ToList();
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

        public ItemStackModel AddCountableItemStack(EItemType itemType, int itemId, int amount)
        {
            CountableItemGameData itemGameData = GameDataLoader.GetCountableItemData(itemType, itemId);
            return AddCountableItemStack(itemGameData, amount);
        }
        
        public ItemStackModel AddCountableItemStack(CountableItemGameData itemGameData, int amount)
        {
            if (itemGameData == null)
                return null;

            ItemStackModel itemStack = new(itemGameData, amount);
            
            if (itemGameData.ItemType == EItemType.MaterialItem)
                materialItemInventory.Add(itemStack);
            else if (itemGameData.ItemType == EItemType.BattleItem)
                battleItemInventory.Add(itemStack);
            
            return itemStack;
        }

        private void SortMechPartList(EMechPartType type)
        {
            mechPartInventory[type] = mechPartInventory[type]
                .OrderBy(mechPart => (mechPart.Owner.Value != null) ? 0 : 1)
                .ThenByDescending(mechPart => mechPart.Rarity)
                .ThenBy(mechPart => mechPart.Id)
                .ToList();
        }
    }
}