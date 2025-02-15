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
using System.Net;
using System.Collections.Generic;
using UnityEngine.TextCore.Text;
using DG.Tweening.Plugins.Core.PathCore;

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
            [SerializeReference]
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
                string assetPath = $"{directory}/{typeName}_{_so.intId}.asset";

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
                && asset is not SkillDataAsset)
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
                "Assets/Game Assets/Scriptable Objects/GlobalSO/Starter Data.asset",
                typeof(StarterDataAsset)
            );

            tree.AddAssetAtPath(
                "상수",
                "Assets/Game Assets/Scriptable Objects/GlobalSO/Constant Data.asset",
                typeof(GlobalDataAsset)
            );

            tree.AddAssetAtPath(
                "경험치 테이블",
                "Assets/Game Assets/Scriptable Objects/GlobalSO/Exp Data.asset",
                typeof(ExpDataAsset)
            );

            // 캐릭터
            IScriptableObjectAssetCreator creator = new ScriptableObjectAssetCreator<CharacterSO>();
            _soAssetCreators.Add(creator);
            tree.Add("캐릭터", creator);

            tree.AddAllAssetsAtPath(
                "캐릭터",
                $"Assets/Game Assets/Scriptable Objects/{nameof(CharacterSO)}",
                typeof(CharacterSO)
            )
            .ForEach(AddDragHandles);

            // 장비
            creator = new ScriptableObjectAssetCreator<EquipmentSO>();
            _soAssetCreators.Add(creator);
            tree.Add("장비", creator);

            tree.AddAllAssetsAtPath(
                "장비",
                $"Assets/Game Assets/Scriptable Objects/{nameof(EquipmentSO)}",
                typeof(EquipmentSO)
            )
            .ForEach(AddDragHandles);

            tree.EnumerateTree()
                .ForEach(menuItem =>
                {
                    if (menuItem.Value is CharacterSO character)
                    {
                        menuItem.Name = character.displayName;
                        menuItem.Icon = character.portrait?.texture;
                    }
                    else if (menuItem.Value is EquipmentSO equipment)
                    {
                        menuItem.Name = equipment.displayName;

                        if (equipment.icon != null)
                        {
                            Rect texRect = equipment.icon.textureRect;
                            menuItem.Icon = equipment.icon.texture.CropTexture(texRect);
                        }
                    }
                    else
                    {
                        return;
                    }

                    AddDragHandles(menuItem);
                });

            return tree;
        }

        void AddDragHandles(OdinMenuItem menuItem)
        {
            menuItem.OnDrawItem += x => DragAndDropUtilities.DragZone(menuItem.Rect, menuItem.Value, false, false);
        }
    }
}