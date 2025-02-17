using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Mathlife.ProjectL.Editor
{
    [InitializeOnLoad]
    public static class SceneBootstrapper
    {
        const string k_editorKey_previousSceneName = "ProjectL/PreviousSceneName";
        static bool s_restartingToSwitchScene = false;

        static string bootstrapSceneName => EditorBuildSettings.scenes[0].path;
        static string previousSceneName
        {
            get => EditorPrefs.GetString(k_editorKey_previousSceneName);
            set => EditorPrefs.SetString(k_editorKey_previousSceneName, value);
        }

        static SceneBootstrapper()
        {
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        }

        private static void OnPlayModeStateChanged(PlayModeStateChange stateChange)
        {
            if (s_restartingToSwitchScene)
            {
                if (stateChange == PlayModeStateChange.EnteredPlayMode)
                {
                    s_restartingToSwitchScene = false;
                }
                return;
            }

            if (stateChange == PlayModeStateChange.ExitingEditMode)
            {
                if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo() == false)
                {
                    EditorApplication.isPlaying = false;
                    return;
                }

                Scene previousScene = SceneManager.GetActiveScene();
                previousSceneName = previousScene.path;
                s_restartingToSwitchScene = previousSceneName != bootstrapSceneName;

                if (s_restartingToSwitchScene)
                {
                    EditorApplication.isPlaying = false;

                    EditorSceneManager.OpenScene(bootstrapSceneName);

                    EditorApplication.isPlaying = true;
                }

            }
            else if (stateChange == PlayModeStateChange.EnteredEditMode)
            {
                EditorSceneManager.OpenScene(previousSceneName);
            }
        }
    }
}