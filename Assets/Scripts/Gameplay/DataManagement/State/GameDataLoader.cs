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
        StarterSO m_starterSO;
        ExpSO m_expSO;
        ShopSO m_shopSO;
        PrefabSO m_prafabSO;
        Dictionary<int, CharacterGameData> m_characters = new();
        Dictionary<int, EquipmentGameData> m_equipments = new();

        public async UniTask Load()
        {
            var dataAssets = await Addressables.LoadAssetsAsync<ScriptableObject>("Data Asset");

            foreach (var dataAsset in dataAssets)
            {
                if (dataAsset is ExpSO expSO)
                    m_expSO = expSO;
                else if (dataAsset is ShopSO shopSO)
                    m_shopSO = shopSO;
                else if (dataAsset is PrefabSO prefabSO)
                    m_prafabSO = prefabSO;
                else if (dataAsset is StarterSO starterSO)
                    m_starterSO = starterSO;
                else if (dataAsset is CharacterGameData characterSO)
                    m_characters.Add(characterSO.id, characterSO);
                else if (dataAsset is EquipmentGameData equipmentSO)
                    m_equipments.Add(equipmentSO.id, equipmentSO);
            }
        }

        public T Instantiate<T>(EPrefabId prefabId) where T : Component
        {
            return Instantiate<T>(prefabId, null);
        }

        public T Instantiate<T>(EPrefabId prefabId, Transform parent) where T : Component
        {
            if (false == m_prafabSO.prefabs.ContainsKey(prefabId))
            {
                Debug.LogError($"Tried to instantiate undefined prefab {prefabId}");
                return null;
            }

            return GameObject.Instantiate(m_prafabSO.prefabs[prefabId], parent).GetComponent<T>();
        }

        public CharacterGameData GetCharacterData(int id)
        {
            if (id < 0)
                return null;

            return m_characters[id];
        }

        public EquipmentGameData GetEquipmentSO(int id)
        {
            if (id < 0)
                return null;

            return m_equipments[id];
        }

        public StarterSO GetStarterSO()
        {
            return m_starterSO;
        }

        public ShopSO GetShopSO()
        {
            return m_shopSO;
        }

        public PrefabSO GetPrefabSO()
        {
            return m_prafabSO;
        }

        public ExpSO GetExpSO()
        {
            return m_expSO;
        }

        public void Dispose()
        {
            Addressables.Release(m_expSO);
            
            foreach(var character in m_characters.Values)
            {
                Addressables.Release(character);
            }
        }
    }
}