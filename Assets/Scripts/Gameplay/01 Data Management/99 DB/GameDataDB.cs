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
    public class GameDataDB : IDisposable
    {
        ExpDataAsset m_expData;
        GlobalDataAsset m_globalData;
        StarterDataAsset m_starterData;
        Dictionary<ECharacterId, CharacterSO> m_characters = new();
        Dictionary<EEquipmentId, EquipmentSO> m_equipments = new();

        public async UniTask Load()
        {
            var dataAssets = await Addressables.LoadAssetsAsync<ScriptableObject>("Data Asset");

            foreach (var dataAsset in dataAssets)
            {
                if (dataAsset is ExpDataAsset expData)
                {
                    m_expData = expData;
                }
                else if (dataAsset is GlobalDataAsset globalData)
                {
                    m_globalData = globalData;
                }
                else if (dataAsset is StarterDataAsset starterData)
                {
                    m_starterData = starterData;
                }
                else if (dataAsset is CharacterSO character)
                {
                    m_characters.Add(character.id, character);
                }
                else if (dataAsset is EquipmentSO equipment)
                {
                    m_equipments.Add(equipment.id, equipment);
                }
            }
        }

        public T Instantiate<T>(EPrefabId prefabId) where T : Component
        {
            return Instantiate<T>(prefabId, null);
        }

        public T Instantiate<T>(EPrefabId prefabId, Transform parent) where T : Component
        {
            if (false == m_globalData.prefabs.ContainsKey(prefabId))
            {
                Debug.LogError($"Tried to instantiate undefined prefab {prefabId}");
                return null;
            }

            return GameObject.Instantiate(m_globalData.prefabs[prefabId], parent).GetComponent<T>();
        }

        public CharacterSO GetCharacterData(ECharacterId id)
        {
            if (id == ECharacterId.None)
                return null;

            return m_characters[id];
        }

        public EquipmentSO GetEquipmentSO(EEquipmentId id)
        {
            if (id == EEquipmentId.None)
                return null;

            return m_equipments[id];
        }

        public StarterDataAsset GetStarterData()
        {
            return m_starterData;
        }

        public GlobalDataAsset GetGlobalData()
        {
            return m_globalData;
        }

        public ExpDataAsset GetExpData()
        {
            return m_expData;
        }

        public void Dispose()
        {
            Addressables.Release(m_expData);
            
            foreach(var character in m_characters.Values)
            {
                Addressables.Release(character);
            }
        }
    }
}