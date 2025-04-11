using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Mathlife.ProjectL.Gameplay.Play;
using UniRx;
using UnityEngine;

namespace Mathlife.ProjectL.Gameplay
{
    public class InventoryState : PersistableStateBase
    {
        // Method
        public override UniTask Load()
        {
            foreach (var type in Enum.GetValues(typeof(EMechPartType)))
            {
                mechPartInventory.Add((EMechPartType)type, new());
            }

            if (GameState.Inst.saveDataManager.CanLoad() && GameSettings.Inst.UseSaveFileIfAvailable)
            {
                LoadFromSaveFile();
            }
            else
            {
                LoadFromStarterData();
            }

            return UniTask.CompletedTask;
        }

        protected override SaveFile SavedFile => SaveDataManager.Inventory;

        protected override SaveFile TakeSnapShot()
        {
            return new InventorySaveFile()
            {
                gold = Gold,
                diamond = Diamond,
                mechParts = mechPartInventory
                    .SelectMany(kv => kv.Value)
                    .Select(model => model.Id)
                    .ToList(),
                materialItems = materialItemInventory
                    .Select(kv => kv.Value)
                    .Select(stack => new ItemStackSaveData() {amount = stack.Amount, id =  stack.Id})
                    .ToList(),
                battleItems = battleItemInventory
                    .Select(kv => kv.Value)
                    .Select(stack => new ItemStackSaveData() {amount = stack.Amount, id =  stack.Id})
                    .ToList()
            };
        }

        private void LoadFromSaveFile()
        {
            goldRx.Value = SaveDataManager.Inventory.gold;
            diamondRx.Value = SaveDataManager.Inventory.diamond;

            foreach (int equipmentId in SaveDataManager.Inventory.mechParts)
            {
                AddMechPart(equipmentId);
            }

            foreach (var itemStack in SaveDataManager.Inventory.materialItems)
            {
                AddCountableItemStack(EItemType.MaterialItem, itemStack.id, itemStack.amount);
            }

            foreach (var itemStack in SaveDataManager.Inventory.battleItems)
            {
                AddCountableItemStack(EItemType.BattleItem, itemStack.id, itemStack.amount);
            }
        }

        private void LoadFromStarterData()
        {
            StarterGameData starter = GameDataLoader.GetStarterData();

            goldRx.Value = starter.GetStarterGold();
            diamondRx.Value = starter.GetStarterDiamond();

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
        private readonly ReactiveProperty<long> goldRx = new(0L);
        private readonly ReactiveProperty<long> diamondRx = new(0L);

        public IObservable<long> GoldObservable => goldRx;
        public IObservable<long> DiamondObservable => diamondRx;

        public long Gold => goldRx.Value;
        public long Diamond => diamondRx.Value;

        public void GainGold(long gain)
        {
            if (gain <= 0L)
                return;

            goldRx.Value += gain;
        }

        private void LoseGold(long lose)
        {
            if (lose <= 0L)
                return;

            goldRx.Value -= lose;
        }

        public void GainDiamond(long gain)
        {
            if (gain <= 0L)
                return;

            diamondRx.Value += gain;
        }

        private void LoseDiamond(long lose)
        {
            if (lose <= 0L)
                return;

            diamondRx.Value -= lose;
        }

        public bool CanBuyByGold(int price, int amount)
        {
            return goldRx.Value >= price * amount;
        }

        public bool CanBuyByDiamond(int price, int amount)
        {
            return diamondRx.Value >= price * amount;
        }

        public bool BuyArty(ShopArtySaleInfo saleInfo)
        {
            if (CanBuyByDiamond(saleInfo.price, saleInfo.amount) == false)
            {
                return false;
            }

            ArtyRosterState roster = GameState.Inst.artyRosterState;
            roster.Add(saleInfo.arty, 1, 0);

            LoseDiamond(saleInfo.price * saleInfo.amount);
            return true;
        }

        public bool BuyItem(ShopItemSaleInfo saleInfo)
        {
            if (CanBuyByGold(saleInfo.price, saleInfo.amount) == false)
            {
                return false;
            }

            switch (saleInfo.item)
            {
                case MechPartGameData mechPartGameData:
                    for (int i = 0; i < saleInfo.amount; ++i)
                        AddMechPart(mechPartGameData.id);
                    break;
                case MaterialItemGameData materialItemGameData:
                    AddCountableItemStack(EItemType.MaterialItem, materialItemGameData.id, saleInfo.amount);
                    break;
                case BattleItemGameData battleItemGameData:
                    AddCountableItemStack(EItemType.BattleItem, battleItemGameData.id, saleInfo.amount);
                    break;
            }

            LoseGold(saleInfo.price * saleInfo.amount);
            return true;
        }

        // 아이템 관리
        private readonly ReactiveDictionary<EMechPartType, List<MechPartModel>> mechPartInventory = new();
        private readonly ReactiveDictionary<int, ItemStackModel> materialItemInventory = new();
        private readonly ReactiveDictionary<int, ItemStackModel> battleItemInventory = new();

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
                .Select(kv => kv.Value)
                .Where(itemStack => excludeFilter(itemStack) == false)
                .OrderByDescending(itemStack => itemStack.Rarity)
                .ThenBy(itemStack => itemStack.Id)
                .ToList();
        }

        public MechPartModel FindBackupMechPart(EMechPartType type, int id)
        {
            return mechPartInventory[type].FirstOrDefault(eq => eq.Id == id && eq.Owner.Value == null);
        }

        private MechPartModel AddMechPart(int mechPartId)
        {
            if (mechPartId < 0)
                return null;

            MechPartModel mechPart = new(GameDataLoader.GetMechPartData(mechPartId));
            mechPartInventory[mechPart.Type].Add(mechPart);
            return mechPart;
        }

        private ItemStackModel AddCountableItemStack(EItemType itemType, int itemId, int amount)
        {
            CountableItemGameData itemGameData = GameDataLoader.GetCountableItemData(itemType, itemId);
            return AddCountableItemStack(itemGameData, amount);
        }

        private ItemStackModel AddCountableItemStack(CountableItemGameData itemGameData, int amount)
        {
            if (itemGameData == null)
                return null;

            ItemStackModel itemStack = null;

            switch (itemGameData.ItemType)
            {
                case EItemType.MaterialItem when materialItemInventory.TryGetValue(itemGameData.id, out itemStack):
                    itemStack.Add(amount);
                    break;
                case EItemType.MaterialItem:
                    itemStack = new(itemGameData, amount);
                    materialItemInventory.Add(itemGameData.id, itemStack);
                    break;
                case EItemType.BattleItem when battleItemInventory.TryGetValue(itemGameData.id, out itemStack):
                    itemStack.Add(amount);
                    break;
                case EItemType.BattleItem:
                    itemStack = new(itemGameData, amount);
                    battleItemInventory.Add(itemGameData.id, itemStack);
                    break;
            }

            return itemStack;
        }

        public bool LoseCountableItems(CountableItemGameData itemGameData, int amount)
        {
            if (itemGameData == null)
                return false;

            ItemStackModel itemStack = null;
            bool result = false;

            switch (itemGameData.ItemType)
            {
                case EItemType.MaterialItem when materialItemInventory.TryGetValue(itemGameData.id, out itemStack):
                    itemStack.Remove(amount);
                    if (itemStack.Amount == 0)
                    {
                        materialItemInventory.Remove(itemGameData.id);
                    }

                    result = true;
                    break;
                case EItemType.BattleItem when battleItemInventory.TryGetValue(itemGameData.id, out itemStack):
                    itemStack.Remove(amount);
                    if (itemStack.Amount == 0)
                    {
                        battleItemInventory.Remove(itemGameData.id);
                    }

                    result = true;
                    break;
                default:
                    break;
            }

            return result;
        }

        private void SortMechPartList(EMechPartType type)
        {
            mechPartInventory[type] = mechPartInventory[type]
                .OrderBy(mechPart => (mechPart.Owner.Value != null) ? 0 : 1)
                .ThenByDescending(mechPart => mechPart.Rarity)
                .ThenBy(mechPart => mechPart.Id)
                .ToList();
        }

        public ItemStackModel GetMaterialItemStack(int materialItemId)
        {
            bool result = materialItemInventory.TryGetValue(materialItemId, out ItemStackModel itemStack);
            return result ? itemStack : null;
        }

        public ItemStackModel GetBattleItemStack(int battleItemId)
        {
            bool result = battleItemInventory.TryGetValue(battleItemId, out ItemStackModel itemStack);
            return result ? itemStack : null;
        }

        public void UseBattleItem(int battleItemId, ArtyController artyController)
        {
            bool result = battleItemInventory.TryGetValue(battleItemId, out ItemStackModel itemStack);

            if (result == false)
            {
                Debug.LogError("배틀 아이템 사용 실패...");
                return;
            }

            BattleItemGameData itemData = itemStack.BattleItemGameData;
            if (itemData == null)
            {
                Debug.LogError("배틀 아이템 사용 실패...");
                return;
            }

            itemData.effect.Apply(artyController);
            itemStack.Remove(1);

            if (itemStack.Amount == 0)
            {
                battleItemInventory.Remove(battleItemId);
            }
        }

        public void GainReward(Reward reward)
        {
            GainGold(reward.gold);
            GainDiamond(reward.diamond);

            if (reward.itemGameData != null)
            {
                if (reward.itemGameData is MechPartGameData mechPartGameData)
                {
                    AddMechPart(mechPartGameData.id);
                }
                else if (reward.itemGameData is CountableItemGameData countableItemGameData)
                {
                    AddCountableItemStack(countableItemGameData, reward.itemAmount);
                }
            }
        }
    }
}