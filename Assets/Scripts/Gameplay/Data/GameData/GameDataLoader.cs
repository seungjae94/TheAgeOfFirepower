using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Data;
using System.Net;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.ResourceLocations;
using UnityEngine.UIElements;

namespace Mathlife.ProjectL.Gameplay
{
    public class GameDataLoader : IDisposable
    {
        private StarterGameData mStarterGameData;
        private ExpGameData expGameData;
        private ShopGameData shopGameData;
        private PrefabGameData prafabGameData;
        private Dictionary<int, ShellGameData> shells = new();
        private Dictionary<int, VehicleGameData> vehicles = new();
        private Dictionary<int, MechPartGameData> mechParts = new();

        public async UniTask Load()
        {
            var dataAssets = await Addressables.LoadAssetsAsync<ScriptableObject>("Data Asset");

            foreach (var dataAsset in dataAssets)
            {
                if (dataAsset is ExpGameData expSO)
                    expGameData = expSO;
                else if (dataAsset is ShopGameData shopSO)
                    shopGameData = shopSO;
                else if (dataAsset is PrefabGameData prefabSO)
                    prafabGameData = prefabSO;
                else if (dataAsset is StarterGameData starterSO)
                    mStarterGameData = starterSO;
                else if (dataAsset is VehicleGameData characterSO)
                    vehicles.Add(characterSO.id, characterSO);
                else if (dataAsset is MechPartGameData equipmentSO)
                    mechParts.Add(equipmentSO.id, equipmentSO);
            }
        }

        public T Instantiate<T>(EPrefabId prefabId) where T : Component
        {
            return Instantiate<T>(prefabId, null);
        }

        public T Instantiate<T>(EPrefabId prefabId, Transform parent) where T : Component
        {
            if (false == prafabGameData.prefabs.ContainsKey(prefabId))
            {
                Debug.LogError($"Tried to instantiate undefined prefab {prefabId}");
                return null;
            }

            return GameObject.Instantiate(prafabGameData.prefabs[prefabId], parent).GetComponent<T>();
        }

        public VehicleGameData GetCharacterData(int id)
        {
            if (id < 0)
                return null;

            return vehicles[id];
        }

        public MechPartGameData GetEquipmentSO(int id)
        {
            if (id < 0)
                return null;

            return mechParts[id];
        }

        public StarterGameData GetStarterSO()
        {
            return mStarterGameData;
        }

        public ShopGameData GetShopSO()
        {
            return shopGameData;
        }

        public PrefabGameData GetPrefabSO()
        {
            return prafabGameData;
        }

        public ExpGameData GetExpSO()
        {
            return expGameData;
        }

        public void Dispose()
        {
            Addressables.Release(expGameData);
            
            foreach(var character in vehicles.Values)
            {
                Addressables.Release(character);
            }
        }
    }
}