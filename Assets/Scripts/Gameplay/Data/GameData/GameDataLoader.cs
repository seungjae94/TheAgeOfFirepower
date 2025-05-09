using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Mathlife.ProjectL.Gameplay
{
    public class GameDataLoader : IDisposable
    {
        private StarterGameData starterGameData;
        private ExpGameData expGameData;
        private ShopGameData shopGameData;
        private readonly Dictionary<int, ShellGameData> shells = new();
        private readonly Dictionary<int, ArtyGameData> artyDict = new();
        private readonly Dictionary<int, MechPartGameData> mechParts = new();
        private readonly Dictionary<EItemType, Dictionary<int, CountableItemGameData>> countableItems = new();
        private readonly Dictionary<int, Dictionary<int, StageGameData>> stages = new();

        public int GetStageCount(int worldNo)
        {
            return stages.TryGetValue(worldNo, out var stage) ? stage.Count : 0;
        }
        
        public async UniTask Load()
        {
            var dataAssets = await Addressables.LoadAssetsAsync<ScriptableObject>("Data Asset");

            foreach (var dataAsset in dataAssets)
            {
                switch (dataAsset)
                {
                    case ExpGameData itExpGameData:
                        expGameData = itExpGameData;
                        break;
                    case ShopGameData itShopGameData:
                        shopGameData = itShopGameData;
                        break;
                    case StarterGameData itStarterGameData:
                        starterGameData = itStarterGameData;
                        break;
                    case ArtyGameData itArtyGameData:
                        artyDict.Add(itArtyGameData.id, itArtyGameData);
                        break;
                    case ShellGameData itShellGameData:
                        shells.Add(itShellGameData.id, itShellGameData);
                        break;
                    case MechPartGameData itMechPartGameData:
                        mechParts.Add(itMechPartGameData.id, itMechPartGameData);
                        break;
                    case CountableItemGameData itCountableItemGameData:
                    {
                        var itemType = itCountableItemGameData.ItemType;
                        if (countableItems.ContainsKey(itemType) == false)
                            countableItems.Add(itemType, new Dictionary<int, CountableItemGameData>());
                        countableItems[itemType].Add(itCountableItemGameData.id, itCountableItemGameData);
                        break;
                    }
                    case StageGameData itStageGameData:
                        int worldNo = itStageGameData.worldNo;
                        int stageNo = itStageGameData.stageNo;
                        if (stages.ContainsKey(worldNo) == false)
                            stages.Add(worldNo, new());
                        stages[worldNo].Add(stageNo, itStageGameData);
                        break;
                }
            }
        }

        public ArtyGameData GetArtyData(int id)
        {
            if (id < 0 || artyDict.ContainsKey(id) == false)
                return null;

            return artyDict[id];
        }
        
        public ShellGameData GetShellData(int id)
        {
            if (id < 0 || shells.ContainsKey(id) == false)
                return null;

            return shells[id];
        }

        public MechPartGameData GetMechPartData(int id)
        {
            if (id < 0 || mechParts.ContainsKey(id) == false)
                return null;

            return mechParts[id];
        }

        public CountableItemGameData GetCountableItemData(EItemType itemType, int id)
        {
            if (id < 0 || countableItems[itemType].ContainsKey(id) == false)
                return null;

            return countableItems[itemType][id];
        }

        public StarterGameData GetStarterData()
        {
            return starterGameData;
        }

        public ShopGameData GetShopData()
        {
            return shopGameData;
        }

        public ExpGameData GetExpData()
        {
            return expGameData;
        }

        public Dictionary<int, StageGameData> GetWorldMapData(int worldNo)
        {
            if (worldNo < 0 || stages.ContainsKey(worldNo) == false)
                return null;
            
            return stages[worldNo];
        }

        public void Dispose()
        {
            Addressables.Release(expGameData);

            foreach (var character in artyDict.Values)
            {
                Addressables.Release(character);
            }
        }
    }
}