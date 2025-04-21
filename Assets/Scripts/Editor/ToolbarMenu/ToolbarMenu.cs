using System;
using System.IO;
using Cysharp.Threading.Tasks;
using Mathlife.ProjectL.Gameplay;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityToolbarExtender;
using Directory = System.IO.Directory;
using Object = UnityEngine.Object;

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

            TakeScreenShot();
            AdaptDisplay();
            // PlayFromStartScene();
        }

        private static void TakeScreenShot()
        {
            if (!GUILayout.Button(new GUIContent("SS", "Take Screen Shot"), ToolbarStyles.commandButtonStyle))
                return;

            string screenShotDirectoryPath = Path.Combine(Application.dataPath, "../ScreenShots");
            if (false == Directory.Exists(screenShotDirectoryPath))
            {
                Directory.CreateDirectory(screenShotDirectoryPath);
            }

            string year = DateTime.Now.Year.ToString().PadLeft(4, '0');
            string month = DateTime.Now.Month.ToString().PadLeft(2, '0');
            string day = DateTime.Now.Day.ToString().PadLeft(2, '0');
            string hour = DateTime.Now.Hour.ToString().PadLeft(2, '0');
            string minute = DateTime.Now.Minute.ToString().PadLeft(2, '0');
            string second = DateTime.Now.Second.ToString().PadLeft(2, '0');
            string milli = DateTime.Now.Millisecond.ToString().PadLeft(4, '0');
            ScreenCapture.CaptureScreenshot(
                $"ScreenShots/ScreenShot {year}-{month}-{day} {hour}-{minute}-{second}-{milli}.png");
        }

        private static void AdaptDisplay()
        {
            if (!GUILayout.Button(new GUIContent("DIS", "Adapt Display"), ToolbarStyles.commandButtonStyle))
                return;

            var adapter = Object.FindFirstObjectByType<DisplayManager>();
            adapter.Adapt().Forget();

            MyDebug.Log(
                $"[ToobarMenu] Display Adapted to Screen {Screen.currentResolution.width} x {Screen.currentResolution.height}.");
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