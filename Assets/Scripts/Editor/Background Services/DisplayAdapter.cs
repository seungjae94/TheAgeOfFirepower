using Cysharp.Threading.Tasks;
using Mathlife.ProjectL.Gameplay;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Mathlife.ProjectL.Editor
{
    [InitializeOnLoad]
    public static class DisplayAdapter
    {
        const int k_adaptingInterval = 500;
        const string k_editorKey_displayAdapting = "ProjectL/DisplayAdapting";

        static bool displayAdapting
        {
            get => EditorPrefs.GetBool(k_editorKey_displayAdapting);
            set => EditorPrefs.SetBool(k_editorKey_displayAdapting, value);
        }

        static DisplayAdapter()
        {
            displayAdapting = false;
            EditorApplication.update += Update;
        }

        static async void Update()
        {
            if (EditorApplication.isPlaying || displayAdapting)
                return;

            if (CanvasUpdateRegistry.IsRebuildingGraphics())
            {
                return;
            }

            displayAdapting = true;

            Scene activeScene = SceneManager.GetActiveScene();

            await UniTask.SwitchToMainThread();

            foreach (var area in GameObject.FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None).OfType<IUIArea>())
            {
                area.ApplyArea();
            }

            await UniTask.Delay(k_adaptingInterval);

            displayAdapting = false;
        }
    }
}