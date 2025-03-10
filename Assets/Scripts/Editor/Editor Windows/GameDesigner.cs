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
        public interface IScriptableObjectAssetCreator 
        {
            public void DestroySOInstance();
        }

        internal class ScriptableObjectAssetCreator<SOType> : IScriptableObjectAssetCreator where SOType : GameData
        {
            [ShowInInspector]
            [InlineEditor(ObjectFieldMode = InlineEditorObjectFieldModes.Hidden)]
            SOType _so;

            public ScriptableObjectAssetCreator()
            {
                CreateNewInstance();
            }

            public void DestroySOInstance()
            {
                DestroyImmediate(_so);
            }

            [Button("Create Game Data", ButtonHeight = 40, Icon = SdfIconType.Box)]
            public void CreateButton()
            {
                string typeName = _so.GetType().Name;

                string directory = $"Assets/Game Assets/Scriptable Objects/{typeName}";

                string idString = (_so.id).ToString().PadLeft(4, '0');
                string assetPath = $"{directory}/{typeName}_{idString}.asset";

                if (false == AssetDatabase.AssetPathExists(directory))
                {
                    AssetDatabase.CreateFolder("Assets/Game Assets/Scriptable Objects", typeName);
                }

                AssetDatabase.CreateAsset(_so, assetPath);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();

                // Addressable 애셋 세팅
                AddressableAssetSettings settings = AddressableAssetSettingsDefaultObject.Settings;
                if (settings == null)
                {
                    Debug.LogError("Addressable Asset Settings not found. Please set up Addressables.");
                    return;
                }

                AddressableAssetEntry entry = settings.CreateOrMoveEntry(AssetDatabase.AssetPathToGUID(assetPath), settings.DefaultGroup);
                entry.address = assetPath;
                entry.SetLabel("Data Asset", true);

                settings.SetDirty(AddressableAssetSettings.ModificationEvent.EntryModified, entry, true);

                // 새 인스턴스 생성
                CreateNewInstance();
            }

            void CreateNewInstance()
            {
                _so = ScriptableObject.CreateInstance<SOType>();
                _so.displayName = $"New {_so.GetType().Name}";
            }
        }

        readonly List<IScriptableObjectAssetCreator> _soAssetCreators = new();

        [MenuItem("Tools/Game Designer")]
        static void OpenWindow()
        {
            var window = GetWindow<GameDesigner>();
            window.position = GUIHelper.GetEditorWindowRect().AlignCenter(1280, 720);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            foreach (var creator in _soAssetCreators)
            {
                creator.DestroySOInstance();
            }

            _soAssetCreators.Clear();
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

            if (asset is not CharacterGameData 
                && asset is not EquipmentGameData 
                && asset is not SkillGameData)
                return;

            GUILayout.Label(selection.FirstOrDefault().Name);

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
            //tree.Config.DrawSearchToolbar = true;

            tree.AddAssetAtPath(
                "스타터",
                "Assets/AddressableAssets/GameData/UniqueGameData/StarterGameData.asset",
                typeof(StarterGameData)
            );

            tree.AddAssetAtPath(
                "경험치",
                "Assets/AddressableAssets/GameData/UniqueGameData/ExpGameData.asset",
                typeof(ExpGameData)
            );

            tree.AddAssetAtPath(
                "상점",
                "Assets/AddressableAssets/GameData/UniqueGameData/ShopGameData.asset",
                typeof(ShopGameData)
            );

            tree.AddAssetAtPath(
                "프리팹",
                "Assets/AddressableAssets/GameData/UniqueGameData/PrefabGameData.asset",
                typeof(PrefabGameData)
            );

            // 메뉴 아이템 생성
            AddMenuItems<CharacterGameData>(tree, "캐릭터");
            AddMenuItems<EquipmentGameData>(tree, "장비");
            AddMenuItems<SkillGameData>(tree, "스킬");

            // 
            tree.EnumerateTree()
                .Where(menuItem => menuItem.Value is GameData)
                .Select(menuItem => (menuItem: menuItem, namedSO: (GameData)menuItem.Value))
                .ForEach(tuple =>
                {
                    (OdinMenuItem menuItem, GameData namedSO) = tuple;
                    namedSO.ToMenuItem(ref menuItem);
                    AddDragHandles(menuItem);
                });

            return tree;
        }

        void AddMenuItems<SOType>(OdinMenuTree tree, string menuDirectoryName) where SOType : GameData
        {
            Type soType = typeof(SOType);

            IScriptableObjectAssetCreator creator = new ScriptableObjectAssetCreator<SOType>();
            _soAssetCreators.Add(creator);
            tree.Add(menuDirectoryName, creator);

            foreach (string guid in AssetDatabase.FindAssets("", new[] { $"Assets/AddressableAssets/GameData/{soType.Name}" }))
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                tree.AddAssetAtPath($"{menuDirectoryName}/{guid}", assetPath, typeof(SOType));
            }
        }

        void AddDragHandles(OdinMenuItem menuItem)
        {
            menuItem.OnDrawItem += x => DragAndDropUtilities.DragZone(menuItem.Rect, menuItem.Value, false, false);
        }
    }
}