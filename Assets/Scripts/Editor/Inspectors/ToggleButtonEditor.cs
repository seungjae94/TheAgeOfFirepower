using Mathlife.ProjectL.Gameplay.UI;
using UnityEditor;
using UnityEditor.UI;

namespace Mathlife.ProjectL.Editor
{
    [CustomEditor(typeof(ToggleButton), true)]
    [CanEditMultipleObjects]
    public class ToggleButtonEditor : ButtonEditor
    {
        private SerializedProperty targetImage;
        private SerializedProperty targetText;
        private SerializedProperty onPreset;
        private SerializedProperty offPreset;
        
        protected override void OnEnable()
        {
            base.OnEnable();
            targetImage = serializedObject.FindProperty("targetImage");
            targetText = serializedObject.FindProperty("targetText");
            onPreset = serializedObject.FindProperty("onPreset");
            offPreset = serializedObject.FindProperty("offPreset");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            serializedObject.Update();

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Toggle Button", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(targetImage);
            EditorGUILayout.PropertyField(targetText);
            EditorGUILayout.PropertyField(onPreset);
            EditorGUILayout.PropertyField(offPreset);
            
            serializedObject.ApplyModifiedProperties();
        }
    }
}