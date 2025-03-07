using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Mathlife.ProjectL.Editor
{
    [InitializeOnLoad]
    public static class SceneBootstrapper
    {
        private const string EditorPrefsKey_PreviousSceneName = "MobileCasualRPG/PreviousSceneName";
        private static bool _sRestartingToSwitchScene = false;

        private static string BootstrapSceneName => EditorBuildSettings.scenes[0].path;

        private static string PreviousSceneName
        {
            get => EditorPrefs.GetString(EditorPrefsKey_PreviousSceneName);
            set => EditorPrefs.SetString(EditorPrefsKey_PreviousSceneName, value);
        }

        static SceneBootstrapper()
        {
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        }

        private static void OnPlayModeStateChanged(PlayModeStateChange stateChange)
        {
            if (_sRestartingToSwitchScene)
            {
                if (stateChange == PlayModeStateChange.EnteredPlayMode)
                {
                    _sRestartingToSwitchScene = false;
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
                PreviousSceneName = previousScene.path;
                _sRestartingToSwitchScene = PreviousSceneName != BootstrapSceneName;

                if (_sRestartingToSwitchScene)
                {
                    EditorApplication.isPlaying = false;

                    EditorSceneManager.OpenScene(BootstrapSceneName);

                    EditorApplication.isPlaying = true;
                }

            }
            else if (stateChange == PlayModeStateChange.EnteredEditMode)
            {
                EditorSceneManager.OpenScene(PreviousSceneName);
            }
        }
    }
}