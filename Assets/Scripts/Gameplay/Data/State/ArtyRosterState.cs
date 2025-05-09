using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Sirenix.Utilities;
using UniRx;
using UnityEngine;

namespace Mathlife.ProjectL.Gameplay
{
    public class ArtyRosterState : PersistableStateBase
    {
        // Field
        private readonly ReactiveCollection<ArtyModel> artyList = new();
        public ArtyModel this[int index] => artyList[index];

        public BatteryModel Battery { get; private set; }


        public override UniTask Load()
        {
            // Validate that InventoryState was created before creating CharacterState.
            if (GameState.Inst.inventoryState == null)
            {
                MyDebug.LogError($"[{nameof(ArtyRosterState)}] InventoryState is null.");
                throw new Exception($"[{nameof(ArtyRosterState)}] InventoryState is null.");
            }

            if (GameState.Inst.saveDataManager.CanLoad() && DebugSettings.Inst.UseSaveFileIfAvailable)
            {
                LoadFromSaveFile();
            }
            else
            {
                LoadFromStarterData();
            }

            return UniTask.CompletedTask;
        }

        protected override SaveFile SavedFile => SaveDataManager.ArtyRoster;

        protected override SaveFile TakeSnapShot()
        {
            return new ArtyRosterSaveFile()
            {
                artyRoster = artyList
                    .Where(model => model != null)
                    .Select(ModelToSaveData)
                    .ToList(),
                battery = new()
                {
                    memberIndexes = Battery
                        .Select(BatteryMemberToIndex)
                        .ToList()
                }
            };
        }

        private ArtySaveData ModelToSaveData(ArtyModel model)
        {
            ArtySaveData saveData = new()
            {
                artyId = model.Id,
                level = model.levelRx.Value,
                totalExp = model.totalExpRx.Value,
                barrelId = model.Barrel?.Id ?? -1,
                armorId = model.Armor?.Id ?? -1,
                engineId = model.Engine?.Id ?? -1
            };
            return saveData;
        }

        private int BatteryMemberToIndex(ArtyModel model)
        {
            return artyList.IndexOf(model);
        }

        // From Save File
        private void LoadFromSaveFile()
        {
            ArtyRosterSaveFile artyRosterSaveFile = SaveDataManager.ArtyRoster;

            foreach (var artySaveData in artyRosterSaveFile.artyRoster)
            {
                ArtyModel arty = new ArtyModel(
                    GameDataLoader.GetArtyData(artySaveData.artyId),
                    artySaveData.level,
                    artySaveData.totalExp
                );

                arty.Equip(EMechPartType.Barrel, artySaveData.barrelId);
                arty.Equip(EMechPartType.Armor, artySaveData.armorId);
                arty.Equip(EMechPartType.Engine, artySaveData.engineId);

                artyList.Add(arty);
            }

            // 세이브 파일에 저장된 화포가 없는 경우
            if (artyList.Count == 0)
            {
                LoadFromStarterData();
                return;
            }

            BatterySaveData teamSaveData = artyRosterSaveFile.battery;

            // 포대 설정이 잘못된 경우
            if (false == teamSaveData.IsHealthy())
            {
                ConstructBestTeam();
                return;
            }

            // Construct battery by data.
            List<ArtyModel> members = teamSaveData.memberIndexes
                .Select((memberIndex) => (memberIndex < 0) ? null : artyList[memberIndex])
                .ToList();

            Battery = new(members);
        }

        private void LoadFromStarterData()
        {
            StarterGameData starterData = GameDataLoader.GetStarterData();
            ExpGameData expData = GameDataLoader.GetExpData();

            var starterBattery = starterData.GetStarterBattery();
            var starterBench = starterData.GetStarterBench();

            List<ArtyPreset> artyPresets = new();
            artyPresets.AddRange(starterBattery);
            artyPresets.AddRange(starterBench);

            foreach (var artyPreset in artyPresets)
            {
                if (artyPreset.arty == null)
                    continue;

                int level = artyPreset.level;
                long totalExp = expData.totalExpAtLevelList[level] + artyPreset.currentLevelExp;
                ArtyModel arty = new(artyPreset.arty, level, totalExp);

                arty.Equip(EMechPartType.Barrel, (artyPreset.barrel != null) ? artyPreset.barrel.id : -1);
                arty.Equip(EMechPartType.Armor, (artyPreset.armor != null) ? artyPreset.armor.id : -1);
                arty.Equip(EMechPartType.Engine, (artyPreset.engine != null) ? artyPreset.engine.id : -1);

                artyList.Add(arty);
            }

            List<ArtyModel> batteryMembers = new();
            for (int i = 0; i < Constants.BATTERY_SIZE; ++i)
            {
                var artyPreset = starterBattery[i];

                if (artyPreset.arty == null)
                {
                    batteryMembers.Add(null);
                    continue;
                }

                ArtyModel arty = artyList[i];
                batteryMembers.Add(arty);
            }

            Battery = new(batteryMembers);
        }

        private void ConstructBestTeam()
        {
            List<ArtyModel> members = new();
            for (int i = 0; i < Constants.BATTERY_SIZE; ++i)
                members.Add(null);

            Battery = new(members);

            BuildBestTeam();
        }

        public void BuildBestTeam()
        {
            Sort();

            int memberCount = Mathf.Min(artyList.Count, Constants.BATTERY_SIZE);

            var members = artyList.Take(memberCount)
                .Concat(Enumerable.Repeat<ArtyModel>(null, Constants.BATTERY_SIZE - memberCount))
                .ToList();

            Battery.Rebuild(members);
        }

        public ArtyModel Add(ArtyGameData artyGameData, int level = 1, int totalExp = 0)
        {
            ArtyModel arty = new(
                artyGameData,
                level, totalExp);
            artyList.Add(arty);

            return arty;
        }

        public bool Remove(ArtyModel arty)
        {
            if (artyList.Contains(arty) == false)
                return false;

            artyList.Remove(arty);
            Battery.Remove(arty);
            return true;
        }

        public List<ArtyModel> GetSortedList(Func<ArtyModel, bool> excludeFilter = null)
        {
            Sort();

            excludeFilter ??= (arty) => false;

            return artyList
                .Where((arty) => excludeFilter(arty) == false)
                .ToList();
        }

        public IDisposable SubscribeAllArtyLevelChange(Action onChange)
        {
            return artyList.ObserveEveryValueChanged(list => EnumerableToHashCode(list.Select(at => at.levelRx.Value)))
                .Subscribe(_ => onChange?.Invoke());
        }
        
        private static int EnumerableToHashCode<T>(IEnumerable<T> list)
        {
            HashCode hash = new HashCode();
            foreach (var item in list)
            {
                hash.Add(item);
            }
            return hash.ToHashCode();
        }

        private void Sort()
        {
            artyList.Sort(Compare);
            return;

            int Compare(ArtyModel arty0, ArtyModel arty1)
            {
                int result = arty1.levelRx.Value.CompareTo(arty0.levelRx.Value); // Descending

                if (result == 0)
                    result = arty1.totalExpRx.Value.CompareTo(arty0.totalExpRx.Value); // Descending

                if (result == 0)
                    result = arty0.Id.CompareTo(arty1.Id); // Ascending

                return result;
            }
        }

        public int IndexOf(ArtyModel arty)
        {
            return artyList.IndexOf(arty);
        }
    }
}