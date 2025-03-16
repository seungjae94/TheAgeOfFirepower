using Mathlife.ProjectL.Gameplay;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using System.Collections.Generic;
using System;

namespace Mathlife.ProjectL.Editor
{
    internal class GameDesigner : OdinMenuEditorWindow
    {
        private const string k_gameDataAssetFolder = "Assets/AddressableAssets/GameData";

        private interface IScriptableObjectAssetCreator
        {
            public void DestroyScriptableObjectInstance();
        }

        internal class ScriptableObjectAssetCreator<TGameData> : IScriptableObjectAssetCreator
            where TGameData : GameData
        {
            [ShowInInspector]
            [InlineEditor(ObjectFieldMode = InlineEditorObjectFieldModes.Hidden)]
            private TGameData gameData;

            public ScriptableObjectAssetCreator()
            {
                CreateNewInstance();
            }

            public void DestroyScriptableObjectInstance()
            {
                DestroyImmediate(gameData);
            }

            [Button("Create Game Data", ButtonHeight = 40, Icon = SdfIconType.Box)]
            public void CreateButton()
            {
                string typeName = gameData.GetType().Name;

                string directory = $"{k_gameDataAssetFolder}/{typeName}";

                string idString = (gameData.id).ToString().PadLeft(4, '0');
                string assetPath = $"{directory}/{typeName}_{idString}.asset";

                if (false == AssetDatabase.AssetPathExists(directory))
                {
                    AssetDatabase.CreateFolder(k_gameDataAssetFolder, typeName);
                }

                AssetDatabase.CreateAsset(gameData, assetPath);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();

                // Addressable 애셋 세팅
                AddressableAssetSettings settings = AddressableAssetSettingsDefaultObject.Settings;
                if (settings == null)
                {
                    Debug.LogError("Addressable Asset Settings not found. Please set up Addressables.");
                    return;
                }

                AddressableAssetEntry entry =
                    settings.CreateOrMoveEntry(AssetDatabase.AssetPathToGUID(assetPath), settings.DefaultGroup);
                entry.address = assetPath;
                entry.SetLabel("Data Asset", true);

                settings.SetDirty(AddressableAssetSettings.ModificationEvent.EntryModified, entry, true);

                // 새 인스턴스 생성
                CreateNewInstance();
            }

            void CreateNewInstance()
            {
                gameData = ScriptableObject.CreateInstance<TGameData>();
                gameData.displayName = $"New {gameData.GetType().Name}";

                string typeName = gameData.GetType().Name;
                string directory = $"{k_gameDataAssetFolder}/{typeName}";
                gameData.id = AssetDatabase.FindAssets("", new[] { directory }).Length;
            }
        }

        readonly List<IScriptableObjectAssetCreator> gameDataAssetCreators = new();

        [MenuItem("Tools/Game Designer")]
        static void OpenWindow()
        {
            var window = GetWindow<GameDesigner>();
            window.position = GUIHelper.GetEditorWindowRect().AlignCenter(1280, 720);

            EditorApplication.delayCall += () =>
            {
                window.Focus();
                window.Repaint();
            };
        }
        
        protected override void Initialize()
        {
            base.Initialize();
    
            if (MenuTree != null && MenuTree.MenuItems.Count > 0)
            {
                // 첫 번째 항목 강제 선택
                MenuTree.Selection.Clear();
                MenuTree.Selection.Add(MenuTree.MenuItems[0]);
        
                // 포커스 갱신 트리거
                GUIHelper.RequestRepaint();
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            foreach (var creator in gameDataAssetCreators)
            {
                creator.DestroyScriptableObjectInstance();
            }

            gameDataAssetCreators.Clear();
        }

        protected override void OnBeginDrawEditors()
        {
            base.OnBeginDrawEditors();

            SirenixEditorGUI.BeginHorizontalToolbar();
            DrawToolBar();
            SirenixEditorGUI.EndHorizontalToolbar();
        }

        void DrawToolBar()
        {
            GUILayout.FlexibleSpace();

            OdinMenuTreeSelection selection = MenuTree.Selection;
            OdinMenuItem selectedMenu = selection.FirstOrDefault();

            if (selectedMenu == null)
                return;

            ScriptableObject asset = selection.SelectedValue as ScriptableObject;

            if (asset == null)
                return;

            if (asset is not ArtyGameData
                && asset is not MechPartGameData
                && asset is not SkillGameData)
                return;

            GUILayout.Label(selection.FirstOrDefault()?.Name);

            if (SirenixEditorGUI.ToolbarButton("Delete"))
            {
                string path = AssetDatabase.GetAssetPath(asset);
                AssetDatabase.DeleteAsset(path);
                AssetDatabase.SaveAssets();
            }
        }

        protected override OdinMenuTree BuildMenuTree()
        {
            var tree = new OdinMenuTree();
            tree.DefaultMenuStyle.SetHeight(50);
            tree.DefaultMenuStyle.SetIconSize(50);
            
            tree.AddAssetAtPath(
                "경험치",
                $"{k_gameDataAssetFolder}/UniqueGameData/ExpGameData.asset",
                typeof(ExpGameData)
            );
            
            tree.AddAssetAtPath(
                "스타터",
                $"{k_gameDataAssetFolder}/UniqueGameData/StarterGameData.asset",
                typeof(StarterGameData)
            );

            tree.AddAssetAtPath(
                "상점",
                $"{k_gameDataAssetFolder}/UniqueGameData/ShopGameData.asset",
                typeof(ShopGameData)
            );

            tree.AddAssetAtPath(
                "프리팹",
                $"{k_gameDataAssetFolder}/UniqueGameData/PrefabGameData.asset",
                typeof(PrefabGameData)
            );

            // 메뉴 아이템 생성
            AddMenuItems<ArtyGameData>(tree, "차량");
            AddMenuItems<ShellGameData>(tree, "포탄");
            AddMenuItems<MechPartGameData>(tree, "기계 부품");
            //AddMenuItems<SkillGameData>(tree, "스킬");

            // 메뉴 아이템 아이콘 설정 및 드래그 핸들 추가
            tree.EnumerateTree()
                .Where(menuItem => menuItem.Value is GameData)
                .ForEach(menuItem =>
                {
                    GameData gameData = menuItem.Value as GameData;
                    gameData?.SetMenuItem(ref menuItem);
                    AddDragHandles(menuItem);
                });
            
            return tree;
        }

        void AddMenuItems<TGameData>(OdinMenuTree tree, string menuDirectoryName) where TGameData : GameData
        {
            string assetDirectory = $"{k_gameDataAssetFolder}/{typeof(TGameData).Name}";

            if (AssetDatabase.IsValidFolder(assetDirectory) == false)
            {
                AssetDatabase.CreateFolder(k_gameDataAssetFolder, typeof(TGameData).Name);
            }

            IScriptableObjectAssetCreator creator = new ScriptableObjectAssetCreator<TGameData>();
            gameDataAssetCreators.Add(creator);
            tree.Add(menuDirectoryName, creator);

            foreach (string guid in AssetDatabase.FindAssets("", new[] { assetDirectory }))
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                tree.AddAssetAtPath($"{menuDirectoryName}/{guid}", assetPath, typeof(TGameData));
            }
        }

        void AddDragHandles(OdinMenuItem menuItem)
        {
            menuItem.OnDrawItem += x => DragAndDropUtilities.DragZone(menuItem.Rect, menuItem.Value, false, false);
        }
    }
}