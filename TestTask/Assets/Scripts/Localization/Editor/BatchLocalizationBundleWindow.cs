using System;
using System.Collections.Generic;
using System.IO;
using Features.Localization.Models.Enums;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;

namespace Experiments.Features.Localization.Editor
{
    public class BatchLocalizationBundleWindow : EditorWindow
    {
        private string _localizationBasePath = "Assets/Localizations";
        private readonly Dictionary<Locale, bool> _selectedLocales = new();
        private readonly List<ExperimentEntry> _entries = new();
        private Vector2 _scroll;

        [MenuItem("Tools/Batch Localization Bundle Creator")]
        public static void ShowWindow() =>
            GetWindow<BatchLocalizationBundleWindow>("Batch Bundle Creator");

        private void OnEnable()
        {
            foreach (Locale locale in Enum.GetValues(typeof(Locale)))
                if (!_selectedLocales.ContainsKey(locale))
                    _selectedLocales[locale] = locale == Locale.Ua;
        }

        private void OnGUI()
        {
            _scroll = EditorGUILayout.BeginScrollView(_scroll);

            DrawSettings();
            GUILayout.Space(6);
            DrawLocaleSelector();
            GUILayout.Space(6);
            DrawExperimentList();
            GUILayout.Space(10);
            DrawActionButtons();

            EditorGUILayout.EndScrollView();
        }

        // ─── Settings ────────────────────────────────────────────────────────────

        private void DrawSettings()
        {
            GUILayout.Label("Settings", EditorStyles.boldLabel);
            _localizationBasePath = EditorGUILayout.TextField("Base Localizations Path", _localizationBasePath);
        }

        // ─── Locale toggles ───────────────────────────────────────────────────────

        private void DrawLocaleSelector()
        {
            GUILayout.Label("Process Locales", EditorStyles.boldLabel);
            EditorGUILayout.BeginHorizontal();

            foreach (Locale locale in Enum.GetValues(typeof(Locale)))
            {
                bool current = _selectedLocales.TryGetValue(locale, out bool v) && v;
                bool next = GUILayout.Toggle(current, locale.ToString(), "Button", GUILayout.Width(55));
                if (next != current)
                    _selectedLocales[locale] = next;
            }

            EditorGUILayout.EndHorizontal();
        }

        // ─── Experiment list ──────────────────────────────────────────────────────

        private void DrawExperimentList()
        {
            GUILayout.Label("Experiments", EditorStyles.boldLabel);

            // Drop zone
            Rect dropRect = GUILayoutUtility.GetRect(0, 38, GUILayout.ExpandWidth(true));
            GUI.Box(dropRect, "↓  Drop experiment prefabs here  ↓", EditorStyles.helpBox);
            HandleDrop(dropRect);

            if (_entries.Count == 0)
                return;

            // Column header
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Prefab", EditorStyles.miniLabel, GUILayout.Width(180));
            GUILayout.Label("Experiment Name", EditorStyles.miniLabel, GUILayout.Width(160));
            foreach (Locale locale in Enum.GetValues(typeof(Locale)))
                GUILayout.Label(locale.ToString(), EditorStyles.miniLabel, GUILayout.Width(30));
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();

            // Rows
            for (int i = 0; i < _entries.Count; i++)
            {
                ExperimentEntry entry = _entries[i];
                EditorGUILayout.BeginHorizontal();

                // Prefab object field
                GameObject newPrefab = (GameObject)EditorGUILayout.ObjectField(
                    entry.Prefab, typeof(GameObject), false, GUILayout.Width(180));

                if (newPrefab != entry.Prefab)
                {
                    entry.Prefab = newPrefab;
                    // Auto-detect name only when no manual override has been set
                    if (newPrefab != null && string.IsNullOrEmpty(entry.ManualName))
                        entry.DetectedName = ResolveExperimentName(newPrefab);
                }

                // Editable name field — shows detected name, user can override
                string displayName = string.IsNullOrEmpty(entry.ManualName)
                    ? entry.DetectedName
                    : entry.ManualName;

                string newName = EditorGUILayout.TextField(displayName, GUILayout.Width(160));
                bool nameChangedByUser = newName != displayName;
                if (nameChangedByUser)
                    entry.ManualName = newName;

                // Status dots per locale
                string effectiveName = entry.EffectiveName;
                foreach (Locale locale in Enum.GetValues(typeof(Locale)))
                {
                    bool selected = _selectedLocales.TryGetValue(locale, out bool sv) && sv;
                    bool exists = !string.IsNullOrEmpty(effectiveName) && FolderExists(effectiveName, locale);

                    GUIStyle dot = new GUIStyle(EditorStyles.miniLabel) { alignment = TextAnchor.MiddleCenter };
                    dot.normal.textColor = !selected
                        ? new Color(0.45f, 0.45f, 0.45f)
                        : exists ? Color.green : Color.red;

                    GUILayout.Label(exists ? "✓" : "✗", dot, GUILayout.Width(30));
                }

                // Remove button
                if (GUILayout.Button("✕", GUILayout.Width(22)))
                {
                    _entries.RemoveAt(i);
                    i--;
                }

                EditorGUILayout.EndHorizontal();
            }
        }

        // ─── Action buttons ───────────────────────────────────────────────────────

        private void DrawActionButtons()
        {
            EditorGUILayout.BeginHorizontal();

            GUI.backgroundColor = new Color(0.5f, 1f, 0.5f);
            if (GUILayout.Button("Assign All Bundle Labels", GUILayout.Height(34)))
                AssignAll();
            GUI.backgroundColor = Color.white;

            if (GUILayout.Button("Clear List", GUILayout.Width(80), GUILayout.Height(34)))
                _entries.Clear();

            EditorGUILayout.EndHorizontal();
        }

        // ─── Drag & drop ──────────────────────────────────────────────────────────

        private void HandleDrop(Rect dropRect)
        {
            Event evt = Event.current;
            if (evt.type != EventType.DragUpdated && evt.type != EventType.DragPerform)
                return;
            if (!dropRect.Contains(evt.mousePosition))
                return;

            DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

            if (evt.type == EventType.DragPerform)
            {
                DragAndDrop.AcceptDrag();

                foreach (UnityEngine.Object obj in DragAndDrop.objectReferences)
                {
                    if (obj is not GameObject go) continue;
                    if (_entries.Exists(e => e.Prefab == go)) continue;

                    _entries.Add(new ExperimentEntry
                    {
                        Prefab = go,
                        DetectedName = ResolveExperimentName(go)
                    });
                }

                Repaint();
            }

            evt.Use();
        }

        // ─── Name resolution ──────────────────────────────────────────────────────

        /// <summary>
        /// Tries to find an existing localization folder for the prefab.
        /// Applies the same naming variants as ExperimentLocalizationControllerEditor:
        ///   1. Exact name
        ///   2. Strip locale suffix (_ua, _en …)
        ///   3. Add _loc
        ///   4. Strip suffix + add _loc
        /// Returns the first matching folder name, or falls back to the prefab name.
        /// </summary>
        private string ResolveExperimentName(GameObject prefab)
        {
            if (prefab == null) return "";

            string name = prefab.name;

            // 1. Exact name
            if (AssetDatabase.IsValidFolder($"{_localizationBasePath}/{name}"))
                return name;

            // 2–4. Variants per locale suffix
            foreach (Locale locale in Enum.GetValues(typeof(Locale)))
            {
                string suffix = "_" + locale.ToString().ToLower();
                if (!name.ToLower().EndsWith(suffix)) continue;

                string stripped = name.Substring(0, name.Length - suffix.Length);

                if (AssetDatabase.IsValidFolder($"{_localizationBasePath}/{stripped}"))
                    return stripped;

                if (AssetDatabase.IsValidFolder($"{_localizationBasePath}/{stripped}_loc"))
                    return stripped + "_loc";
            }

            // 3. Name + _loc
            if (!name.ToLower().EndsWith("_loc") &&
                AssetDatabase.IsValidFolder($"{_localizationBasePath}/{name}_loc"))
                return name + "_loc";

            return name; // fallback — user can edit in the name field
        }

        // ─── Bundle assignment ────────────────────────────────────────────────────

        private bool FolderExists(string expName, Locale locale) =>
            AssetDatabase.IsValidFolder($"{_localizationBasePath}/{expName}/{locale.ToString().ToLower()}");

        private void AssignAll()
        {
            int success = 0, skipped = 0;

            foreach (ExperimentEntry entry in _entries)
            {
                string expName = entry.EffectiveName;
                if (string.IsNullOrEmpty(expName))
                {
                    Debug.LogWarning("[BatchBundle] Skipping entry with empty experiment name.");
                    skipped++;
                    continue;
                }

                foreach (Locale locale in Enum.GetValues(typeof(Locale)))
                {
                    if (!_selectedLocales.TryGetValue(locale, out bool selected) || !selected)
                        continue;

                    string folderPath = $"{_localizationBasePath}/{expName}/{locale.ToString().ToLower()}";

                    if (!AssetDatabase.IsValidFolder(folderPath))
                    {
                        Debug.LogWarning($"[BatchBundle] Folder not found, skipping: {folderPath}");
                        skipped++;
                        continue;
                    }

                    string bundleName = $"localization_{locale.ToString().ToLower()}_{expName.ToLower()}";
                    AssetImporter importer = AssetImporter.GetAtPath(folderPath);

                    if (importer != null)
                    {
                        importer.assetBundleName = bundleName;
                        Debug.Log($"[BatchBundle] '{bundleName}' ← {folderPath}");
                        success++;

                        // Also label textures & materials referenced in the JSON so they
                        // get bundled even though they live outside the localization folder.
                        AssignBundleToLinkedAssets(folderPath, bundleName);
                    }
                    else
                    {
                        Debug.LogError($"[BatchBundle] AssetImporter not found for: {folderPath}");
                        skipped++;
                    }
                }
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            EditorUtility.DisplayDialog(
                "Batch Bundle Creator",
                $"Done!\nBundle labels assigned: {success}\nSkipped (folder missing): {skipped}",
                "OK");
        }

        // ─── Linked-asset helpers ─────────────────────────────────────────────────

        /// <summary>
        /// Reads the localization JSON from <paramref name="folderPath"/>,
        /// extracts Texture and Material keys, finds those assets anywhere in the project,
        /// and assigns <paramref name="bundleName"/> to their importers.
        /// </summary>
        private static void AssignBundleToLinkedAssets(string folderPath, string bundleName)
        {
            string[] jsonGuids = AssetDatabase.FindAssets("t:TextAsset", new[] { folderPath });
            if (jsonGuids.Length == 0)
                return;

            string jsonPath = AssetDatabase.GUIDToAssetPath(jsonGuids[0]);
            string json = File.ReadAllText(jsonPath);

            try
            {
                var data = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);
                if (data == null) return;

                if (data.TryGetValue("Textures", out object texObj))
                {
                    var list = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(texObj.ToString());
                    foreach (var e in list)
                        if (e.TryGetValue("Key", out string k))
                            AssignBundleToAssetByKey(k, "Texture2D", bundleName);
                }

                if (data.TryGetValue("Materials", out object matObj))
                {
                    var list = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(matObj.ToString());
                    foreach (var e in list)
                        if (e.TryGetValue("Key", out string k))
                            AssignBundleToAssetByKey(k, "Material", bundleName);
                }
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"[BatchBundle] Failed to parse linked assets in {jsonPath}: {ex.Message}");
            }
        }

        /// <summary>
        /// Finds the first project asset whose filename (without extension) matches
        /// <paramref name="key"/> (case-insensitive) and assigns the bundle label.
        /// </summary>
        private static void AssignBundleToAssetByKey(string key, string typeName, string bundleName)
        {
            if (string.IsNullOrEmpty(key)) return;

            string[] guids = AssetDatabase.FindAssets($"{key} t:{typeName}");
            string path = null;

            foreach (string guid in guids)
            {
                string p = AssetDatabase.GUIDToAssetPath(guid);
                if (string.Equals(Path.GetFileNameWithoutExtension(p), key,
                        StringComparison.OrdinalIgnoreCase))
                {
                    path = p;
                    break;
                }
            }

            if (path == null && guids.Length > 0)
                path = AssetDatabase.GUIDToAssetPath(guids[0]);

            if (string.IsNullOrEmpty(path))
            {
                Debug.LogWarning($"[BatchBundle] Asset not found: '{key}' (t:{typeName})");
                return;
            }

            AssetImporter imp = AssetImporter.GetAtPath(path);
            if (imp != null)
            {
                imp.assetBundleName = bundleName;
                Debug.Log($"[BatchBundle] '{bundleName}' → {path}");
            }
        }

        // ─── Data class ───────────────────────────────────────────────────────────

        private class ExperimentEntry
        {
            public GameObject Prefab;
            public string DetectedName = ""; // auto-resolved from prefab name
            public string ManualName = "";   // typed by user; takes priority if non-empty

            public string EffectiveName =>
                !string.IsNullOrEmpty(ManualName) ? ManualName : DetectedName;
        }
    }
}
