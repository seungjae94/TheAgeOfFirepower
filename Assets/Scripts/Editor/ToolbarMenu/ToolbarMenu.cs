using Cysharp.Threading.Tasks;
using Mathlife.ProjectL.Gameplay;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityToolbarExtender;

namespace Mathlife.ProjectL.Editor
{
    [InitializeOnLoad]
    internal static class ToolbarMenu
    {
        private const string k_editorPrefsKey_editModeScenePath = "MobileCasualRPG/EditModeScenePath";
        private static readonly string s_startScenePath = EditorBuildSettings.scenes[0].path;

        static ToolbarMenu()
        {
            ToolbarExtender.LeftToolbarGUI.Add(OnToolbarGUI);

            EditorApplication.playModeStateChanged += OnEnteredEditMode;
        }

        private static void OnToolbarGUI()
        {
            GUILayout.FlexibleSpace();

            AdaptDisplay();
            // PlayFromStartScene();
        }

        private static void AdaptDisplay()
        {
            if (!GUILayout.Button(new GUIContent("DIS", "Adapt Display"), ToolbarStyles.commandButtonStyle))
                return;
            
            var adapter = Object.FindFirstObjectByType<DisplayManager>();
            adapter.Adapt().Forget();

            MyDebug.Log($"[ToobarMenu] Display Adapted to Screen {Screen.currentResolution.width} x {Screen.currentResolution.height}.");
        }

        private static void PlayFromStartScene()
        {
            if (!GUILayout.Button(new GUIContent("S", "Play From Start"), ToolbarStyles.commandButtonStyle))
                return;

            if (EditorPrefs.HasKey(k_editorPrefsKey_editModeScenePath))
                EditorPrefs.DeleteKey(k_editorPrefsKey_editModeScenePath);

            // 1. Edit Mode 씬 백업
            // 씬 저장 여부 묻기
            if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo() == false)
                return;

            Scene activeScene = SceneManager.GetActiveScene();
            EditorPrefs.SetString(k_editorPrefsKey_editModeScenePath, activeScene.path);

            // 2. 스타트 씬 열기
            EditorSceneManager.OpenScene(s_startScenePath);

            // 3. 플레이 모드 진입
            EditorApplication.EnterPlaymode();
        }

        static void OnEnteredEditMode(PlayModeStateChange playModeStateChange)
        {
            if (playModeStateChange != PlayModeStateChange.EnteredEditMode)
                return;

            if (EditorPrefs.HasKey(k_editorPrefsKey_editModeScenePath) == false)
                return;

            EditorSceneManager.OpenScene(EditorPrefs.GetString(k_editorPrefsKey_editModeScenePath));
            EditorPrefs.DeleteKey(k_editorPrefsKey_editModeScenePath);
        }
    }
}