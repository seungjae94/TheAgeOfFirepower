using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;
using VContainer;

namespace Mathlife.ProjectL.Gameplay
{
    public class SaveDataManager
    {
        Dictionary<FieldInfo, string> saveFileFullPaths = new();

        public CharacterSaveFile character = new();
        public InventorySaveFile inventory = new();
        public SaveDataManager()
        {
            foreach (FieldInfo field in typeof(SaveDataManager).GetFields())
            {
                if (false == field.FieldType.IsSubclassOf(typeof(SaveFile)))
                    continue;

                saveFileFullPaths.Add(field, ConvertToSaveFileFullPath(field.FieldType.Name));
            }
        }

        public bool DoesSaveFileExist()
        {
            foreach (string path in saveFileFullPaths.Values)
            {
                if (File.Exists(path) == false)
                    return false;
            }

            return true;
        }

        public async UniTask Load()
        {
            // File -> POD
            foreach (var (field, path) in saveFileFullPaths)
            {
                string json = await File.ReadAllTextAsync(path);
                object saveFile = field.GetValue(this);
                JsonUtility.FromJsonOverwrite(json, saveFile);
            }
        }

        public async UniTask Save()
        {
            // POD -> File
            foreach (var (field, path) in saveFileFullPaths)
            {
                string json = JsonUtility.ToJson(field.GetValue(this));
                await File.WriteAllTextAsync(path, json);
            }
        }

        string ConvertToSaveFileFullPath(string saveFileName)
        {
#if UNITY_EDITOR
            return Application.dataPath + "/Editor/" + saveFileName + ".json";
#else
        return Application.persistentDataPath + saveFileName + ".json";
#endif
        }
    }
}