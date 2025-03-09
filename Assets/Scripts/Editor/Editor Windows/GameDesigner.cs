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

        internal class ScriptableObjectAssetCreator<SOType> : IScriptableObjectAssetCreator where SOType : NamedGameData
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

            [Button("����", ButtonHeight = 40, Icon = SdfIconType.Box)]
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

                // Addressable ó�� �� �� ����
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

                // �� ������ �ּ� �غ�
                CreateNewInstance();
            }

            void CreateNewInstance()
            {
                _so = ScriptableObject.CreateInstance<SOType>();
                _so.displayName = $"New {_so.GetType().Name}";
            }
        }

        List<IScriptableObjectAssetCreator> _soAssetCreators = new();

        [MenuItem("Project L/Game Designer")]
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
                "��Ÿ�� ����",
                "Assets/Game Assets/Scriptable Objects/UniqueSO/StarterSO.asset",
                typeof(StarterSO)
            );

            tree.AddAssetAtPath(
                "����ġ ���̺�",
                "Assets/Game Assets/Scriptable Objects/UniqueSO/ExpSO.asset",
                typeof(ExpSO)
            );

            tree.AddAssetAtPath(
                "����",
                "Assets/Game Assets/Scriptable Objects/UniqueSO/ShopSO.asset",
                typeof(ShopSO)
            );

            tree.AddAssetAtPath(
                "������ ���",
                "Assets/Game Assets/Scriptable Objects/UniqueSO/PrefabSO.asset",
                typeof(PrefabSO)
            );

            // �޴� ������ �߰�
            AddMenuItems<CharacterGameData>(tree, "ĳ����");
            AddMenuItems<EquipmentGameData>(tree, "���");
            AddMenuItems<SkillGameData>(tree, "��ų");

            // �޴� ������ ����
            tree.EnumerateTree()
                .Where(menuItem => menuItem.Value is NamedGameData)
                .Select(menuItem => (menuItem: menuItem, namedSO: (NamedGameData)menuItem.Value))
                .ForEach(tuple =>
                {
                    (OdinMenuItem menuItem, NamedGameData namedSO) = tuple;
                    namedSO.ToMenuItem(ref menuItem);
                    AddDragHandles(menuItem);
                });

            return tree;
        }

        void AddMenuItems<SOType>(OdinMenuTree tree, string menuDirectoryName) where SOType : NamedGameData
        {
            Type soType = typeof(SOType);

            IScriptableObjectAssetCreator creator = new ScriptableObjectAssetCreator<SOType>();
            _soAssetCreators.Add(creator);
            tree.Add(menuDirectoryName, creator);

            foreach (string guid in AssetDatabase.FindAssets("", new[] { $"Assets/Game Assets/Scriptable Objects/{soType.Name}" }))
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