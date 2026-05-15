using Features.Localization.Components;
using UnityEditor;
using UnityEngine;

namespace Experiments.Features.Localization.Editor
{
    [CustomEditor(typeof(TextLocalizationComponent))]
    public class TextLocalizationComponentEditor : UnityEditor.Editor
    {
        private SerializedProperty _textIdProp;
        private SerializedProperty _useCustomIdProp;

        private void OnEnable()
        {
            _textIdProp = serializedObject.FindProperty("_textId");
            _useCustomIdProp = serializedObject.FindProperty("_useCustomId");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(_useCustomIdProp, new GUIContent("Use custom Id"));

            if (_useCustomIdProp.boolValue)
            {
                EditorGUILayout.PropertyField(_textIdProp, new GUIContent("Text Id"));
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}