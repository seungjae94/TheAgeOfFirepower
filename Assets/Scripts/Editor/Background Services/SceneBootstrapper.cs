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
        const string k_editorKey_worldMapPrefabPath = "ProjectL/PlacedWorldMapPrefabPath";
        const bool k_saveLoadWorldMapPrefab = false;

        static bool s_restartingToSwitchScene = false;

        static string bootstrapSceneName => EditorBuildSettings.scenes[0].path;
        static string previousSceneName
        {
            get => EditorPrefs.GetString(k_editorKey_previousSceneName);
            set => EditorPrefs.SetString(k_editorKey_previousSceneName, value);
        }

        static string worldMapPrefabPath
        {
            get => EditorPrefs.GetString(k_editorKey_worldMapPrefabPath);
            set => EditorPrefs.SetString(k_editorKey_worldMapPrefabPath, value);
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

                if (k_saveLoadWorldMapPrefab && previousSceneName.Contains("WorldScene"))
                {
                    foreach (var GO in previousScene.GetRootGameObjects())
                    {
                        if (GO.CompareTag("Map") == false)
                            continue;

                        // Store map prefab into EditorPrefs.
                        GameObject worldMapPrefab = PrefabUtility.GetCorrespondingObjectFromSource(GO);
                        worldMapPrefabPath = AssetDatabase.GetAssetPath(worldMapPrefab);

                        // Destroy Map GO in the world scene.
                        Object.DestroyImmediate(GO);
                        EditorSceneManager.SaveScene(previousScene);
                    }
                }

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

                if (k_saveLoadWorldMapPrefab && EditorPrefs.HasKey(k_editorKey_worldMapPrefabPath))
                {
                    Scene previousScene = SceneManager.GetActiveScene();

                    // Load map prefab from EditorPrefs.
                    GameObject worldMapPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(worldMapPrefabPath);

                    // Instantiate prefab to the scene.
                    PrefabUtility.InstantiatePrefab(worldMapPrefab);
                    EditorSceneManager.SaveScene(previousScene);

                    // Delete used EditorPrefs key.
                    EditorPrefs.DeleteKey(k_editorKey_worldMapPrefabPath);
                }

            }
        }
    }
}