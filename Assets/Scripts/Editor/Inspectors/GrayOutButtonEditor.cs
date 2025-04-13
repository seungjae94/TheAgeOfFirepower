using Mathlife.ProjectL.Gameplay.UI;
using UnityEditor;
using UnityEditor.UI;

namespace Mathlife.ProjectL.Editor
{
    [CustomEditor(typeof(GrayOutButton), true)]
    [CanEditMultipleObjects]
    public class GrayOutButtonEditor : ButtonEditor
    {
        private SerializedProperty targetGraphics;
        private SerializedProperty saturation;

        protected override void OnEnable()
        {
            base.OnEnable();
            targetGraphics = serializedObject.FindProperty("targetGraphics");
            saturation = serializedObject.FindProperty("saturation");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            serializedObject.Update();

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Gray Out", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(targetGraphics);
            EditorGUILayout.PropertyField(saturation);
            
            serializedObject.ApplyModifiedProperties();
        }
    }
}