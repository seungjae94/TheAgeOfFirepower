using DG.DemiEditor;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityToolbarExtender;

namespace Mathlife.ProjectL.Editor
{
    [InitializeOnLoad]
    static class PlayFromStartToolbarMenu
    {
        private const string k_editorPrefsKey_editModeScenePath = "MobileCasualRPG/EditModeScenePath";
        private static readonly string s_startScenePath = EditorBuildSettings.scenes[0].path;
        
        static PlayFromStartToolbarMenu()
        {
            ToolbarExtender.LeftToolbarGUI.Add(OnToolbarGUI);
            
            EditorApplication.playModeStateChanged += OnEnteredEditMode;
        }

        static void OnToolbarGUI()
        {
            GUILayout.FlexibleSpace();

            if(GUILayout.Button(new GUIContent("S", "Play From Start"), ToolbarStyles.commandButtonStyle))
            {
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