 using UnityEditor;

[CustomEditor(typeof(AnimatorEventObject))]
public class MyScriptEditor : Editor
{
    SerializedProperty isBoolProp;
    SerializedProperty isBoolValueProp;

    void OnEnable()
    {
        isBoolProp = serializedObject.FindProperty("isBool");
        isBoolValueProp = serializedObject.FindProperty("isBoolValue");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        SerializedProperty iterator = serializedObject.GetIterator();
        bool hasNext = iterator.NextVisible(true);

        while (hasNext)
        {
            if (iterator.propertyPath != "isBoolValue" && iterator.propertyPath != "isBool")
            {
                EditorGUILayout.PropertyField(iterator, true);
            }

            hasNext = iterator.NextVisible(false);
        }

        //Show property 'isBool'
        EditorGUILayout.PropertyField(isBoolProp);
        if (isBoolProp.boolValue)
        {
            EditorGUILayout.PropertyField(isBoolValueProp);
        }

        serializedObject.ApplyModifiedProperties();
    }
}