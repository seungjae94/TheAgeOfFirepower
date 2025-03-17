using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;

namespace Mathlife.ProjectL.Gameplay
{
    public class ArtyRosterState : IPersistable
    {
        SaveDataManager saveDataManager;
        GameDataLoader gameDataLoader;
        
        private readonly ReactiveCollection<ArtyModel> artyList = new();
        public ArtyModel this[int index] => artyList[index];
        
        public BatteryModel Battery { get; private set; }
        
        public UniTask Load()
        {
            saveDataManager = GameState.Inst.SaveDataManager;
            gameDataLoader = GameState.Inst.GameDataLoader;

            // Validate that InventoryState was created before creating CharacterState.
            if (GameState.Inst.InventoryState == null)
            {
                Debug.LogError("[CharacterRosterState] InventoryState is null.");
                throw new Exception("[CharacterRosterState] InventoryState is null.");
            }

            if (GameState.Inst.SaveDataManager.DoesSaveFileExist())
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

        // From Save File
        private void LoadFromSaveFile()
        {
            ArtyRosterSaveFile artyRosterSaveFile = saveDataManager.artyRoster;

            foreach (var characterSaveData in artyRosterSaveFile.artyRoster)
            {
                ArtyModel model = new ArtyModel(
                    gameDataLoader.GetCharacterData(characterSaveData.artyId),
                    gameDataLoader.GetExpData(),
                    characterSaveData.level,
                    characterSaveData.totalExp
                );

                Equip(model, characterSaveData.weaponId);
                Equip(model, characterSaveData.armorId);
                Equip(model, characterSaveData.artifactId);

                artyList.Add(model);
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

            // Construct team by data.
            List<ArtyModel> members = teamSaveData.memberIndexes
                .Select((memberIndex) => (memberIndex < 0) ? null : artyList[memberIndex])
                .ToList();

            Battery = new(members);
        }

        private void LoadFromStarterData()
        {
            StarterGameData starterData = gameDataLoader.GetStarterData();
            ExpGameData expData = gameDataLoader.GetExpData();

            var starterBattery = starterData.GetStarterBattery();
            var starterRosterMinusBattery = starterData.GetStarterRosterMinusBattery();

            List<ArtyPreset> artyPresets = new();
            artyPresets.AddRange(starterBattery);
            artyPresets.AddRange(starterRosterMinusBattery);

            foreach (var artyPreset in artyPresets)
            {
                if (artyPreset.arty == null)
                    continue;

                int level = artyPreset.level;
                long totalExp = expData.characterTotalExpAtLevelList[level] + artyPreset.currentLevelExp;
                ArtyModel arty = new(artyPreset.arty, expData, level, totalExp);

                Equip(arty, artyPreset.barrel?.id ?? -1);
                Equip(arty, artyPreset.armor?.id ?? -1);
                Equip(arty, artyPreset.engine?.id ?? -1);

                artyList.Add(arty);
            }

            List<ArtyModel> batteryMembers = new();
            for (int i = 0; i < Constants.BatterySize; ++i)
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

        private void Equip(ArtyModel arty, int equipmentId)
        {
            EEquipmentType equipmentType = (EEquipmentType)(equipmentId / 1000);
            arty.Equip(equipmentType, GameState.Inst.InventoryState.FindEquipment(equipmentType, equipmentId));
        }

        private void ConstructBestTeam()
        {
            List<ArtyModel> members = new();
            for (int i = 0; i < Constants.BatterySize; ++i)
                members.Add(null);

            Battery = new(members);

            BuildBestTeam();
        }

        public void BuildBestTeam()
        {
            int memberCount = Mathf.Min(artyList.Count, Constants.BatterySize);

            List<ArtyModel> members = artyList
                .OrderByDescending(arty => arty.levelRx.Value)
                .Take(memberCount)
                .ToList();

            for (int i = memberCount; i < Constants.BatterySize; ++i)
            {
                members.Add(null);
            }

            Battery.Rebuild(members);
        }

        public ArtyModel Add(int id, int level = 1, int totalExp = 0)
        {
            ArtyModel arty = new(
                gameDataLoader.GetCharacterData(id),
                gameDataLoader.GetExpData(),
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
            excludeFilter ??= (arty) => false;

            return artyList
                .Where((arty) => excludeFilter(arty) == false)
                .OrderByDescending(arty => arty.levelRx.Value)
                .ThenBy(arty => arty.totalExpRx.Value)
                .ThenBy(arty => arty.id)
                .ToList();
        }
    }
}
