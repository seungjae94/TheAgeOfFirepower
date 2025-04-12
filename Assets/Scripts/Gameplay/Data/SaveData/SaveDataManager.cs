using System;
using System.Collections.Concurrent;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;
using UniRx;
using UnityEngine;

namespace Mathlife.ProjectL.Gameplay
{
    public class SaveDataManager
    {
        private readonly ConcurrentDictionary<Type, SaveFile> saveFiles = new();
        private readonly Dictionary<Type, string> savePaths = new();

        public ArtyRosterSaveFile ArtyRoster => saveFiles[typeof(ArtyRosterSaveFile)] as ArtyRosterSaveFile;
        public InventorySaveFile Inventory => saveFiles[typeof(InventorySaveFile)] as InventorySaveFile;
        public GameProgressSaveFile GameProgress => saveFiles[typeof(GameProgressSaveFile)] as GameProgressSaveFile;

#if UNITY_EDITOR
        private static readonly Subject<string> DebugLogger = new();
#endif

        public bool CanLoad()
        {
            foreach (var savePath in savePaths.Values)
            {
                if (File.Exists(savePath) == false)
                    return false;
            }

            return true;
        }

        public async UniTask Load()
        {
#if UNITY_EDITOR
            DebugLogger.Subscribe(Debug.Log)
                .AddTo(GameState.Inst.gameObject);
#endif

            foreach (PropertyInfo prop in typeof(SaveDataManager).GetProperties())
            {
                if (false == prop.PropertyType.IsSubclassOf(typeof(SaveFile)))
                    continue;

                saveFiles.TryAdd(prop.PropertyType, null);

                string savePath = TypeNameToSavePath(prop.PropertyType.Name);
                savePaths.Add(prop.PropertyType, savePath);

                UniTask.RunOnThreadPool(() => SaveTask(prop.PropertyType)).Forget();
            }

            if (false == CanLoad())
                return;

            // File -> POD
            foreach (var (type, savePath) in savePaths)
            {
                string json = await File.ReadAllTextAsync(savePath);
                SaveFile saveFile = JsonUtility.FromJson(json, type) as SaveFile;

                // Update Save File
                UpdateSaveFile(type, saveFile);
            }
        }

        // Producer
        public async UniTaskVoid Save(SaveFile saveFile)
        {
            await UniTask.SwitchToThreadPool();

            Type type = saveFile.GetType();

            // Update Save File
            UpdateSaveFile(type, saveFile);
        }

        private void UpdateSaveFile(Type type, SaveFile saveFile)
        {
            bool updated = false;
            while (!updated)
            {
                if (!saveFiles.TryGetValue(type, out SaveFile prevSaveFile))
                {
                    updated = saveFiles.TryAdd(type, saveFile);
                    continue;
                }

                updated = saveFiles.TryUpdate(type, saveFile, prevSaveFile);
            }
        }

        // Consumer
        private async UniTaskVoid SaveTask(Type saveFileType)
        {
            string savePath = savePaths[saveFileType];

            saveFiles.TryGetValue(saveFileType, out SaveFile prevFile);

            //systemAlive
            while (!Application.exitCancellationToken.IsCancellationRequested)
            {
                if (false == saveFiles.TryGetValue(saveFileType, out SaveFile newFile))
                {
                    throw new KeyNotFoundException($"{saveFileType.Name} 인스턴스가 생성되지 않았습니다.");
                }

                if (prevFile == newFile)
                {
                    await UniTask.Yield();
                    continue;
                }

                string json = JsonUtility.ToJson(newFile);

                if (File.Exists(savePath) == false)
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(savePath) ?? "");
                }

                File.WriteAllText(savePath, json);

                prevFile = newFile;

#if UNITY_EDITOR
                DebugLogger.OnNext($"{saveFileType.Name} 세이브 완료!");
#endif
            }
        }

        private string TypeNameToSavePath(string saveFileName)
        {
#if UNITY_EDITOR
            return Application.dataPath + "/EditorAssets/" + saveFileName + ".json";
#else
        return Application.persistentDataPath + saveFileName + ".json";
#endif
        }
    }
}