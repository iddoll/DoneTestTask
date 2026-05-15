using UnityEditor;
using UnityEngine;
using TMPro;

public class DisableTextMeshProComponents : EditorWindow
{
    private GameObject prefab;

    [MenuItem("Tools/Manage TextMeshPro Components")]
    public static void ShowWindow()
    {
        GetWindow<DisableTextMeshProComponents>("Manage TextMeshPro");
    }
    private void OnGUI()
    {
        GUILayout.Label("Manage TextMeshPro Components", EditorStyles.boldLabel);
        prefab = (GameObject)EditorGUILayout.ObjectField("Prefab (Optional)", prefab, typeof(GameObject), true);

        GUILayout.Space(10);
        
        if (GUILayout.Button("Disable TextMeshPro"))
        {
            if (prefab != null)
            {
                SetTextMeshProStateInPrefab(false);
            }
            else
            {
                SetTextMeshProStateInScene(false);
            }
        }

        if (GUILayout.Button("Enable TextMeshPro"))
        {
            if (prefab != null)
            {
                SetTextMeshProStateInPrefab(true);
            }
            else
            {
                SetTextMeshProStateInScene(true);
            }
        }
    }

    private void SetTextMeshProStateInPrefab(bool state)
    {
        if (prefab == null)
        {
            Debug.LogError("Префаб не указан!");
            return;
        }
        
        TextMeshPro[] textMeshPros = prefab.GetComponentsInChildren<TextMeshPro>(true);

        int count = 0;
        foreach (TextMeshPro textMeshPro in textMeshPros)
        {
            if (textMeshPro != null)
            {
                textMeshPro.enabled = state;
                count++;
            }
        }

        string action = state ? "включено" : "отключено";
        Debug.Log($"{action} {count} компонентов TextMeshPro внутри префаба {prefab.name}.");
    }

    private void SetTextMeshProStateInScene(bool state)
    {
        TextMeshPro[] textMeshPros = FindObjectsOfType<TextMeshPro>(true);

        int count = 0;
        foreach (TextMeshPro textMeshPro in textMeshPros)
        {
            if (textMeshPro != null)
            {
                textMeshPro.enabled = state;
                count++;
            }
        }

        string action = state ? "включено" : "отключено";
        Debug.Log($"{action} {count} компонентов TextMeshPro на всей сцене.");
    }
}
