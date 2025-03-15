#if UNITY_EDITOR
using DG.DOTweenEditor;
using DG.Tweening;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Mathlife.ProjectL.Gameplay.UI.Editor
{
    // ReSharper disable once InconsistentNaming
    public static class MyDOTweenEditorPreview
    {
        public static void Start(Tween tween, GameObject gameObject)
        {
            DOTweenEditorPreview.Stop();
            tween.onComplete = () => Stop(gameObject);
            DOTweenEditorPreview.PrepareTweenForPreview(tween, false);
            DOTweenEditorPreview.Start();
        }
        
        public static void Stop(GameObject gameObject)
        {
            DOTweenEditorPreview.Stop(true, true);
            EditorSceneManager.SaveScene(gameObject.scene);
        }
    }
}
#endif