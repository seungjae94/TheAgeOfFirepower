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

        internal class ScriptableObjectAssetCreator<SOType> : IScriptableObjectAssetCreator where SOType : NamedSO
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

            [Button("생성", ButtonHeight = 40, Icon = SdfIconType.Box)]
            public void CreateButton()
            {
                string typeName = _so.GetType().Name;

                string directory = $"Assets/Game Assets/Scriptable Objects/{typeName}";

                string idString = (_so.intId).ToString().PadLeft(4, '0');
                string assetPath = $"{directory}/{typeName}_{idString}.asset";

                if (false == AssetDatabase.AssetPathExists(directory))
                {
                    AssetDatabase.CreateFolder("Assets/Game Assets/Scriptable Objects", typeName);
                }

                AssetDatabase.CreateAsset(_so, assetPath);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();

                // Addressable 처리 및 라벨 설정
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

                // 새 데이터 애셋 준비
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

            if (asset is not CharacterSO 
                && asset is not EquipmentSO 
                && asset is not SkillSO)
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
                "스타터 세팅",
                "Assets/Game Assets/Scriptable Objects/UniqueSO/StarterSO.asset",
                typeof(StarterSO)
            );

            tree.AddAssetAtPath(
                "경험치 테이블",
                "Assets/Game Assets/Scriptable Objects/UniqueSO/ExpSO.asset",
                typeof(ExpSO)
            );

            tree.AddAssetAtPath(
                "상점",
                "Assets/Game Assets/Scriptable Objects/UniqueSO/ShopSO.asset",
                typeof(ShopSO)
            );

            tree.AddAssetAtPath(
                "프리팹 등록",
                "Assets/Game Assets/Scriptable Objects/UniqueSO/PrefabSO.asset",
                typeof(PrefabSO)
            );

            // 메뉴 아이템 추가
            AddMenuItems<CharacterSO>(tree, "캐릭터");
            AddMenuItems<EquipmentSO>(tree, "장비");
            AddMenuItems<SkillSO>(tree, "스킬");

            // 메뉴 아이템 세팅
            tree.EnumerateTree()
                .Where(menuItem => menuItem.Value is NamedSO)
                .Select(menuItem => (menuItem: menuItem, namedSO: (NamedSO)menuItem.Value))
                .ForEach(tuple =>
                {
                    (OdinMenuItem menuItem, NamedSO namedSO) = tuple;
                    namedSO.ToMenuItem(ref menuItem);
                    AddDragHandles(menuItem);
                });

            return tree;
        }

        void AddMenuItems<SOType>(OdinMenuTree tree, string menuDirectoryName) where SOType : NamedSO
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