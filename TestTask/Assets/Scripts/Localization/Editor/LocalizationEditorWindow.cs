using System.Collections.Generic;
using System.IO;
using System.Linq;
using Features.Localization.Components;
using Features.Localization.Models;
using Features.Localization.Models.Enums;
using Newtonsoft.Json;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Experiments.Features.Localization.Editor
{
    public class LocalizationEditorWindow : EditorWindow
    {
        private readonly string LocalizationDataJson = "LocalizationData{0}.json";
        private readonly List<TextEntry> _textEntries = new();
        private readonly List<TextureEntry> _textureEntries = new();
        private readonly List<VoiceoverEntry> _voiceoverEntries = new();
        private readonly List<MaterialEntry> _materialEntries = new();

        private string _experimentLocalizationsPath = "Assets/Localizations";
        private Locale _selectedLocale = Locale.Ua;
        private string _experimentName = "";
        private Vector2 _scrollPosition;
        private string _localizationDataPath;

        private string CombinedLocalizationsPath => $"{_experimentLocalizationsPath}/{_experimentName}/{_selectedLocale.ToString().ToLower()}";

        [MenuItem("Tools/Localization Editor")]
        public static void ShowWindow()
        {
            GetWindow<LocalizationEditorWindow>("Localization Editor");
        }

        private void OnGUI()
        {
            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);
            GUILayout.Label("Settings", EditorStyles.boldLabel);
            GUILayout.BeginHorizontal();
            _experimentLocalizationsPath = EditorGUILayout.TextField("Localization Path", _experimentLocalizationsPath);

            if (GUILayout.Button("Load json"))
            {
                LoadFromJson();
            }

            GUILayout.EndVertical();
            
            _experimentName = EditorGUILayout.TextField("Experiment Name", _experimentName);
            Locale newLocale = (Locale)EditorGUILayout.EnumPopup("Locale", _selectedLocale);

            if (newLocale != _selectedLocale)
            {
                _selectedLocale = newLocale;
                TryLoadLocalizedJson();
            }

            if (GUILayout.Button("Add Localization Components to all TMP_Texts"))
            {
                var prefabStage = PrefabStageUtility.GetCurrentPrefabStage();

                if (prefabStage == null) // Scene object
                {
                    if (EditorUtility.DisplayDialog(
                            "Add Localization Components",
                            "Are you sure you want to add TextLocalizationComponent to all TMP_Texts in the current scene?",
                            "Yes", "No"))
                    {
                        AddLocalizationToAllTMPTextsInScene();
                    }
                }
                else // Prefab opened in isolation
                {
                    AddLocalizationToAllTMPTextsInPrefab();
                }
            }

            if (GUILayout.Button("Spawn Localization Controller"))
            {
                var prefabStage = PrefabStageUtility.GetCurrentPrefabStage();

                if (prefabStage == null) // Scene
                {
                    if (EditorUtility.DisplayDialog(
                            "Spawn ExperimentLocalizationController",
                            "Are you sure you want to spawn the ExperimentLocalizationController in the current scene?",
                            "Yes", "No"))
                    {
                        SpawnExperimentLocalizationControllerInScene();
                    }
                }
                else // Prefab mode
                {
                    SpawnExperimentLocalizationControllerInPrefab();
                }
            }

            if (GUILayout.Button("Auto-Assign AudioSources to Controller"))
            {
                PrefabStage prefabStage = PrefabStageUtility.GetCurrentPrefabStage();

                if (prefabStage == null) // Scene
                {
                    if (EditorUtility.DisplayDialog(
                            "Auto-Assign AudioSources",
                            "Are you sure you want to find and assign all AudioSources to the ExperimentLocalizationController in the current scene?",
                            "Yes", "No"))
                    {
                        AssignAudioSourcesToControllerInScene();
                    }
                }
                else // Prefab mode
                {
                    AssignAudioSourcesToControllerInPrefab();
                }
            }

            if (GUILayout.Button("Update IDs on All TextLocalizationComponents"))
            {
                PrefabStage prefabStage = PrefabStageUtility.GetCurrentPrefabStage();

                if (prefabStage == null) // Scene
                {
                    if (EditorUtility.DisplayDialog(
                            "Update All Localization IDs",
                            "Are you sure you want to call UpdateId() on all TextLocalizationComponents in the scene?",
                            "Yes", "No"))
                    {
                        UpdateAllTextLocalizationComponentIdsInScene();
                    }
                }
                else // Prefab mode
                {
                    UpdateAllTextLocalizationComponentIdsInPrefab();
                }
            }

            GUILayout.Space(10);

            GUILayout.Label("Text Entries", EditorStyles.boldLabel);
            if (GUILayout.Button("Populate Text Entries from Components"))
            {
                PrefabStage prefabStage = PrefabStageUtility.GetCurrentPrefabStage();

                if (prefabStage == null) // Scene
                {
                    if (EditorUtility.DisplayDialog(
                            "Populate Text Entries",
                            "Are you sure you want to populate text entries from all TextLocalizationComponents in the scene?",
                            "Yes", "No"))
                    {
                        PopulateTextEntriesFromScene();
                    }
                }
                else // Prefab mode
                {
                    PopulateTextEntriesFromPrefab();
                }
            }
            DrawTextEntries();
            GUILayout.Space(10);

            GUILayout.Label("Texture Entries", EditorStyles.boldLabel);
            if (GUILayout.Button("Populate Texture Entries from Components"))
            {
                PrefabStage prefabStage = PrefabStageUtility.GetCurrentPrefabStage();

                if (prefabStage == null) // Scene
                {
                    if (EditorUtility.DisplayDialog(
                            "Populate Texture Entries",
                            "Are you sure you want to populate texture entries from all TextureLocalizationComponents in the scene?",
                            "Yes", "No"))
                    {
                        PopulateTextureEntriesFromScene();
                    }
                }
                else // Prefab mode
                {
                    PopulateTextureEntriesFromPrefab();
                }
            }
            DrawTextureEntries();
            GUILayout.Space(10);

            GUILayout.Label("Material Entries", EditorStyles.boldLabel);
            if (GUILayout.Button("Populate Material Entries from MaterialLocalizationComponents"))
            {
                PrefabStage prefabStage = PrefabStageUtility.GetCurrentPrefabStage();

                if (prefabStage == null) // Scene
                {
                    if (EditorUtility.DisplayDialog(
                            "Populate Material Entries",
                            "Are you sure you want to populate material entries from all MaterialLocalizationComponents in the scene?",
                            "Yes", "No"))
                    {
                        PopulateMaterialEntriesFromScene();
                    }
                }
                else // Prefab mode
                {
                    PopulateMaterialEntriesFromPrefab();
                }
            }
            DrawMaterialEntries();
            GUILayout.Space(10);

            GUILayout.Label("Voiceover Entries", EditorStyles.boldLabel);
            if (GUILayout.Button("Populate Voiceover Entries from AudioSources"))
            {
                PrefabStage prefabStage = PrefabStageUtility.GetCurrentPrefabStage();

                if (prefabStage == null) // Scene
                {
                    if (EditorUtility.DisplayDialog(
                            "Populate Voiceover Entries",
                            "Are you sure you want to populate voiceover entries from all AudioSources in the scene?",
                            "Yes", "No"))
                    {
                        PopulateVoiceoverEntriesFromScene();
                    }
                }
                else // Prefab mode
                {
                    PopulateVoiceoverEntriesFromPrefab();
                }
            }
            DrawVoiceoverEntries();
            GUILayout.Space(10);

            if (GUILayout.Button("Save to JSON"))
            {
                SaveToJson();
            }

            GUIStyle style = new GUIStyle(GUI.skin.button);
            style.normal.textColor = Color.green;
            if (GUILayout.Button("Auto generate localization", style, GUILayout.Height(50)))
            {
                var prefabStage = PrefabStageUtility.GetCurrentPrefabStage();

                if (prefabStage == null) // Scene object
                {
                    if (EditorUtility.DisplayDialog(
                            "Add Localization Components",
                            "Are you sure you want to add TextLocalizationComponent to all TMP_Texts in the current scene?",
                            "Yes", "No"))
                    {
                        switch (_selectedLocale)
                        {
                            case Locale.Ua:
                                AddLocalizationToAllTMPTextsInScene();
                                SpawnExperimentLocalizationControllerInScene();
                                AssignAudioSourcesToControllerInScene();
                                UpdateAllTextLocalizationComponentIdsInScene();
                                PopulateTextEntriesFromScene();
                                PopulateVoiceoverEntriesFromScene();

                                break;
                            default:
                                AddLocalizationToAllTMPTextsInScene();
                                SpawnExperimentLocalizationControllerInScene();
                                AssignAudioSourcesToControllerInScene();
                                UpdateAllTextLocalizationComponentIdsInScene();

                                break;
                        }
                    }
                }
                else // Prefab opened in isolation
                {
                    switch (_selectedLocale)
                    {
                        case Locale.Ua:
                            AddLocalizationToAllTMPTextsInPrefab();
                            SpawnExperimentLocalizationControllerInPrefab();
                            AssignAudioSourcesToControllerInPrefab();
                            UpdateAllTextLocalizationComponentIdsInPrefab();
                            PopulateTextEntriesFromPrefab();
                            PopulateVoiceoverEntriesFromPrefab();

                            break;
                        default:
                            AddLocalizationToAllTMPTextsInPrefab();
                            SpawnExperimentLocalizationControllerInPrefab();
                            AssignAudioSourcesToControllerInPrefab();
                            UpdateAllTextLocalizationComponentIdsInPrefab();

                            break;
                    }
                }
                
                SaveToJson();
            }
            

            if (!string.IsNullOrEmpty(_localizationDataPath))
            {
                if (GUILayout.Button("Create Asset Label"))
                {
                    AssignObjectsToLocalizationAssetBundle();
                }
            }

            EditorGUILayout.EndScrollView();
        }

        private void TryLoadLocalizedJson()
        {
            if (LoadFromJson())
            {
                return;
            }

            foreach (TextEntry textEntry in _textEntries)
            {
                textEntry.LocalizedText = "";
            }
        }

        private void DrawTextEntries()
        {
            for (int i = 0; i < _textEntries.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();

                _textEntries[i] = new TextEntry(
                    EditorGUILayout.TextField(_textEntries[i].Key),
                    EditorGUILayout.TextField(_textEntries[i].LocalizedText));

                if (GUILayout.Button("X", GUILayout.Width(width:20)))
                {
                    _textEntries.RemoveAt(i);
                }

                EditorGUILayout.EndHorizontal();
            }

            if (GUILayout.Button("Add Text Entry"))
            {
                _textEntries.Add(new TextEntry("NewKey", ""));
            }
        }

        private void DrawTextureEntries()
        {
            for (int i = 0; i < _textureEntries.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();
                string pathOverride = "";
                Texture texture = null;

                bool isPathOverrideToggle = _textureEntries[i].IsPathOverride;

                bool newIsPathOverrideToggle = GUILayout.Toggle(isPathOverrideToggle,
                    new GUIContent("", "Use custom texture name"), GUILayout.Width(20));

                if (isPathOverrideToggle)
                {
                    pathOverride =
                        EditorGUILayout.TextField(new GUIContent("Custom texture name"), _textureEntries[i].Key);
                }
                else
                {
                    texture = (Texture)EditorGUILayout.ObjectField(_textureEntries[i].Texture, typeof(Texture),
                        allowSceneObjects:false);
                }

                string newKey = isPathOverrideToggle ? pathOverride :
                    _textureEntries[i].Texture ? _textureEntries[i].Texture.name : "";

                string materialProperty = EditorGUILayout.TextField(new GUIContent("Material Property Name"),
                    _textureEntries[i].MaterialPropertyName);


                if (GUILayout.Button("X", GUILayout.Width(width:20)))
                {
                    _textureEntries.RemoveAt(i);
                }
                else
                {
                    _textureEntries[i] = new TextureEntry(newKey, materialProperty, newIsPathOverrideToggle, texture);
                }

                EditorGUILayout.EndHorizontal();
            }

            if (GUILayout.Button("Add Texture Entry"))
            {
                _textureEntries.Add(new TextureEntry("NewTextureKey", "", false, null));
            }
        }

        private void DrawVoiceoverEntries()
        {
            for (int i = 0; i < _voiceoverEntries.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();

                string pathOverride = "";
                AudioClip clip = null;

                bool isPathOverrideToggle = _voiceoverEntries[i].IsPathOverride;

                bool newIsPathOverrideToggle = GUILayout.Toggle(isPathOverrideToggle,
                    new GUIContent("", "Use custom texture name"), GUILayout.Width(20));

                if (isPathOverrideToggle)
                {
                    pathOverride =
                        EditorGUILayout.TextField(new GUIContent("Custom audio name"), _voiceoverEntries[i].Key);
                }
                else
                {
                    clip = (AudioClip)EditorGUILayout.ObjectField(_voiceoverEntries[i].AudioClip, typeof(AudioClip),
                        allowSceneObjects:false);
                }

                string newKey = isPathOverrideToggle ? pathOverride :
                    _voiceoverEntries[i].AudioClip ? _voiceoverEntries[i].AudioClip.name : "";

                if (GUILayout.Button("X", GUILayout.Width(width:20)))
                {
                    _voiceoverEntries.RemoveAt(i);
                }
                else
                {
                    _voiceoverEntries[i] = new VoiceoverEntry(newKey, newIsPathOverrideToggle, clip);
                }

                EditorGUILayout.EndHorizontal();
            }

            if (GUILayout.Button("Add Voiceover Entry"))
            {
                _voiceoverEntries.Add(new VoiceoverEntry("NewVoiceoverKey", false, null));
            }
        }

        private bool LoadFromJson()
        {
            if (string.IsNullOrWhiteSpace(_experimentName))
            {
                EditorUtility.DisplayDialog("Error", "Please fill in experiment name first!", "OK");
                return false;
            }

            string localizationPath = Path.Combine(CombinedLocalizationsPath,
                string.Format(LocalizationDataJson, _selectedLocale.ToString()));

            if (!File.Exists(localizationPath))
            {
                Debug.LogError("Localization file not found: " + localizationPath);

                return false;
            }

            _localizationDataPath = localizationPath;

            string json = File.ReadAllText(localizationPath);

            Dictionary<string, object> localizationData =
                JsonConvert.DeserializeObject<Dictionary<string, object>>(json);

            _experimentName = localizationData.TryGetValue("ExperimentName", out object experimentName)
                ? experimentName.ToString()
                : "";

            _textEntries.Clear();
            _textureEntries.Clear();
            _voiceoverEntries.Clear();
            _materialEntries.Clear();

            if (localizationData.TryGetValue("Texts", out object textsArray))
            {
                List<Dictionary<string, string>> texts =
                    JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(textsArray.ToString());

                foreach (Dictionary<string, string> entry in texts)
                {
                    _textEntries.Add(new TextEntry(entry["Key"], entry["LocalizedText"]));
                }
            }

            if (localizationData.TryGetValue("Textures", out object value1))
            {
                List<Dictionary<string, string>> textures =
                    JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(value1.ToString());

                foreach (Dictionary<string, string> entry in textures)
                {
                    string key = entry["Key"];

                    string property = entry.TryGetValue("MaterialPropertyName", out string materialPropertyName)
                        ? materialPropertyName
                        : "";

                    _textureEntries.Add(new TextureEntry(key, property, true, null));
                }
            }

            if (localizationData.TryGetValue("VoiceOvers", out object value))
            {
                List<Dictionary<string, string>> voiceovers =
                    JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(value.ToString());

                foreach (var entry in voiceovers)
                {
                    string key = entry["Key"];
                    _voiceoverEntries.Add(new VoiceoverEntry(key, true, null));
                }
            }

            if (localizationData.TryGetValue("Materials", out object materialsObj))
            {
                List<Dictionary<string, object>> materialsList =
                    JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(materialsObj.ToString());

                foreach (Dictionary<string, object> entry in materialsList)
                {
                    string key = entry["Key"].ToString();
                    int materialIndex = entry.TryGetValue("MaterialIndex", out object indexObj)
                        ? int.Parse(indexObj.ToString())
                        : 0;
                    _materialEntries.Add(new MaterialEntry(key, materialIndex, true, null));
                }
            }

            Debug.Log("Localization data loaded successfully");

            return true;
        }

        private void SaveToJson()
        {
            if (string.IsNullOrWhiteSpace(_experimentName))
            {
                EditorUtility.DisplayDialog("Error", "Please fill in experiment name first!", "OK");
                return;
            }
            
            LocalizationModel localizationData = new()
            {
                ExperimentName = _experimentName,
                Texts = SerializeTextEntries(),
                Textures = SerializeTextureEntries(),
                VoiceOvers = SerializeVoiceoverEntries(),
                Materials = SerializeMaterialEntries()
            };

            string json = JsonConvert.SerializeObject(localizationData, Formatting.Indented);

            _localizationDataPath = Path.Combine(CombinedLocalizationsPath,
                string.Format(LocalizationDataJson, _selectedLocale.ToString()));

            if (!Directory.Exists(CombinedLocalizationsPath))
            {
                Directory.CreateDirectory(CombinedLocalizationsPath);
            }

            File.WriteAllText(_localizationDataPath, json);

            int copied = CopyAssetsToLocalizationFolder();

            if (File.Exists(_localizationDataPath))
            {
                string msg = copied > 0
                    ? $"Localization data saved!\n{copied} asset(s) copied to localization folder."
                    : "Localization data saved!";
                EditorUtility.DisplayDialog("Localization", msg, "OK");
            }
            else
            {
                EditorUtility.DisplayDialog("Localization", "Localization data not saved!", "close");
            }

            AssetDatabase.Refresh();
        }

        private int CopyAssetsToLocalizationFolder()
        {
            int count = 0;

            // Only audio is auto-copied; textures and materials are placed manually.
            foreach (VoiceoverEntry entry in _voiceoverEntries)
            {
                if (!entry.IsPathOverride && entry.AudioClip != null)
                    count += CopyAssetToFolder(entry.AudioClip);
            }

            return count;
        }

        private int CopyAssetToFolder(UnityEngine.Object asset)
        {
            string sourcePath = AssetDatabase.GetAssetPath(asset);
            if (string.IsNullOrEmpty(sourcePath))
                return 0;

            string fileName = Path.GetFileName(sourcePath);
            string destPath = Path.Combine(CombinedLocalizationsPath, fileName);

            if (Path.GetFullPath(sourcePath) == Path.GetFullPath(destPath))
                return 0;

            File.Copy(sourcePath, destPath, overwrite: true);
            Debug.Log($"[Localization] Copied: {sourcePath} → {destPath}");
            return 1;
        }

        private void AddLocalizationToAllTMPTextsInScene()
        {
            TMP_Text[] allTMPs = GameObject.FindObjectsOfType<TMP_Text>(true);

            int count = 0;

            foreach (TMP_Text tmp in allTMPs)
            {
                if (tmp.GetComponent<TextLocalizationComponent>() != null)
                {
                    continue;
                }

                Undo.AddComponent<TextLocalizationComponent>(tmp.gameObject);
                count++;
            }

            EditorUtility.DisplayDialog("Localization", $"{count} TextLocalizationComponent(s) added in scene.", "OK");
        }

        private void AddLocalizationToAllTMPTextsInPrefab()
        {
            var prefabStage = PrefabStageUtility.GetCurrentPrefabStage();

            if (prefabStage == null)
            {
                EditorUtility.DisplayDialog("Error", "No prefab is currently open in prefab mode.", "OK");

                return;
            }

            TMPro.TMP_Text[] allTMPs = prefabStage.prefabContentsRoot.GetComponentsInChildren<TMPro.TMP_Text>(true);

            int count = 0;

            foreach (var tmp in allTMPs)
            {
                if (tmp.GetComponent<TextLocalizationComponent>() == null)
                {
                    Undo.AddComponent<TextLocalizationComponent>(tmp.gameObject);
                    count++;
                }
            }

            EditorUtility.DisplayDialog("Localization", $"{count} TextLocalizationComponent(s) added to prefab.", "OK");
        }

        private void SpawnExperimentLocalizationControllerInScene()
        {
            ExperimentLocalizationController existing = FindObjectOfType<ExperimentLocalizationController>();

            if (existing != null)
            {
                Selection.activeGameObject = existing.gameObject;
                EditorGUIUtility.PingObject(existing.gameObject);

                EditorUtility.DisplayDialog("Already Exists",
                    "ExperimentLocalizationController already exists in the scene. It has been selected for you.",
                    "OK");

                return;
            }

            GameObject controllerGo = new GameObject("ExperimentLocalizationController");
            Undo.RegisterCreatedObjectUndo(controllerGo, "Create ExperimentLocalizationController");
            controllerGo.AddComponent<ExperimentLocalizationController>();

            Selection.activeGameObject = controllerGo;
            EditorGUIUtility.PingObject(controllerGo);
            EditorUtility.DisplayDialog("Spawned", "ExperimentLocalizationController added to scene.", "OK");
        }

        private void SpawnExperimentLocalizationControllerInPrefab()
        {
            PrefabStage prefabStage = PrefabStageUtility.GetCurrentPrefabStage();

            if (prefabStage == null)
            {
                EditorUtility.DisplayDialog("Error", "No prefab is currently open in prefab mode.", "OK");

                return;
            }

            ExperimentLocalizationController existing = prefabStage.prefabContentsRoot
                .GetComponentInChildren<ExperimentLocalizationController>(true);

            if (existing != null)
            {
                Selection.activeGameObject = existing.gameObject;
                EditorGUIUtility.PingObject(existing.gameObject);

                EditorUtility.DisplayDialog("Already Exists",
                    "ExperimentLocalizationController already exists in the prefab. It has been selected for you.",
                    "OK");

                return;
            }

            GameObject prefabContentsRoot = prefabStage.prefabContentsRoot;
            Undo.RegisterCreatedObjectUndo(prefabStage.prefabContentsRoot, "Create ExperimentLocalizationController");
            prefabContentsRoot.AddComponent<ExperimentLocalizationController>();
            prefabContentsRoot.transform.SetParent(prefabStage.prefabContentsRoot.transform);

            Selection.activeGameObject = prefabContentsRoot;
            EditorGUIUtility.PingObject(prefabContentsRoot);
            EditorUtility.DisplayDialog("Spawned", "ExperimentLocalizationController added to prefab.", "OK");
        }

        private void AssignObjectsToLocalizationAssetBundle()
        {
            if (string.IsNullOrWhiteSpace(_experimentName))
            {
                EditorUtility.DisplayDialog("Error", "Please fill in experiment name first!", "OK");
                return;
            }

            string bundleName = "localization_" + _selectedLocale.ToString().ToLower() + "_" + _experimentName.ToLower();
            Debug.Log("Creating Asset Bundle: " + bundleName);

            if (!AssetDatabase.IsValidFolder(CombinedLocalizationsPath))
            {
                Debug.LogError("Cant find localization folder by path: " + CombinedLocalizationsPath);
                return;
            }

            int labeledInFolder = AssignBundleToAllAssetsInFolder(CombinedLocalizationsPath, bundleName);

            // Assign bundle label to textures/materials listed in the editor.
            // Prefer copies inside the locale folder to avoid duplication with the experiment bundle.
            foreach (TextureEntry entry in _textureEntries)
                AssignBundleToEntry(entry.Key,
                    entry.IsPathOverride ? null : entry.Texture,
                    "Texture2D", bundleName, CombinedLocalizationsPath);

            foreach (MaterialEntry entry in _materialEntries)
                AssignBundleToEntry(entry.Key,
                    entry.IsPathOverride ? null : entry.Material,
                    "Material", bundleName, CombinedLocalizationsPath);

            foreach (VoiceoverEntry entry in _voiceoverEntries)
            {
                if (entry.AudioClip != null)
                    AssignBundleToEntry(entry.Key, entry.AudioClip, "AudioClip", bundleName, CombinedLocalizationsPath);
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log($"Assigned bundle '{bundleName}' to {labeledInFolder} asset(s) in '{CombinedLocalizationsPath}'.");
            EditorUtility.DisplayDialog("Asset Labels",
                $"Bundle: {bundleName}\n\n" +
                "Rebuild AssetBundles and copy them to StreamingAssets before Play Mode testing.\n\n" +
                "Note: switching Locale in this window only edits JSON — it does not change audio in Play Mode.",
                "OK");
        }

        private static int AssignBundleToAllAssetsInFolder(string folderPath, string bundleName)
        {
            int count = 0;
            string[] guids = AssetDatabase.FindAssets(string.Empty, new[] { folderPath });

            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                if (string.IsNullOrEmpty(path) || AssetDatabase.IsValidFolder(path))
                    continue;

                AssetImporter imp = AssetImporter.GetAtPath(path);
                if (imp == null)
                    continue;

                imp.assetBundleName = bundleName;
                count++;
            }

            return count;
        }

        /// <summary>
        /// Assigns <paramref name="bundleName"/> to the importer of the given asset.
        /// If <paramref name="asset"/> is null, finds the asset in the project by key name.
        /// </summary>
        private static void AssignBundleToEntry(string key, UnityEngine.Object asset,
            string typeName, string bundleName, string localeFolderPath)
        {
            string path = asset != null
                ? AssetDatabase.GetAssetPath(asset)
                : FindAssetPathByKey(key, typeName);

            if (string.IsNullOrEmpty(path))
            {
                if (!string.IsNullOrEmpty(key))
                    Debug.LogWarning($"[Localization] Asset not found for key '{key}' (t:{typeName}). Bundle label skipped.");
                return;
            }

            string normalizedLocalePath = localeFolderPath.Replace('\\', '/');
            string normalizedAssetPath = path.Replace('\\', '/');
            if (!normalizedAssetPath.StartsWith(normalizedLocalePath + "/", StringComparison.OrdinalIgnoreCase))
            {
                Debug.LogWarning(
                    $"[Localization] '{path}' is outside '{localeFolderPath}'. " +
                    "Assigning it to the localization bundle may duplicate memory with the experiment bundle. " +
                    "Prefer a copy inside the locale folder.");
            }

            AssetImporter imp = AssetImporter.GetAtPath(path);
            if (imp != null)
            {
                imp.assetBundleName = bundleName;
                Debug.Log($"[Localization] '{bundleName}' → {path}");
            }
        }

        /// <summary>
        /// Searches the project for an asset whose filename (without extension) exactly
        /// matches <paramref name="key"/> (case-insensitive). Falls back to first result.
        /// </summary>
        private static string FindAssetPathByKey(string key, string typeName)
        {
            if (string.IsNullOrEmpty(key)) return null;

            string[] guids = AssetDatabase.FindAssets($"{key} t:{typeName}");
            foreach (string guid in guids)
            {
                string p = AssetDatabase.GUIDToAssetPath(guid);
                if (string.Equals(Path.GetFileNameWithoutExtension(p), key,
                        System.StringComparison.OrdinalIgnoreCase))
                    return p;
            }
            return guids.Length > 0 ? AssetDatabase.GUIDToAssetPath(guids[0]) : null;
        }

        private void AssignAudioSourcesToControllerInScene()
        {
            ExperimentLocalizationController controller =
                GameObject.FindObjectOfType<ExperimentLocalizationController>();

            if (controller == null)
            {
                EditorUtility.DisplayDialog("Not Found", "No ExperimentLocalizationController found in the scene.",
                    "OK");

                return;
            }

            AudioSource[] audioSources = GameObject.FindObjectsOfType<AudioSource>(true);

            Undo.RecordObject(controller, "Assign AudioSources");
            controller.audioSources = audioSources;

            EditorUtility.SetDirty(controller);

            EditorUtility.DisplayDialog("Assigned", $"{audioSources.Length} AudioSources assigned to the controller.",
                "OK");
        }

        private void AssignAudioSourcesToControllerInPrefab()
        {
            var prefabStage = UnityEditor.SceneManagement.PrefabStageUtility.GetCurrentPrefabStage();

            if (prefabStage == null)
            {
                EditorUtility.DisplayDialog("Error", "This operation is only available in prefab mode.", "OK");

                return;
            }

            var controller = prefabStage.prefabContentsRoot
                .GetComponentInChildren<ExperimentLocalizationController>(true);

            if (controller == null)
            {
                EditorUtility.DisplayDialog("Not Found", "No ExperimentLocalizationController found in the prefab.",
                    "OK");

                return;
            }

            AudioSource[] audioSources = prefabStage.prefabContentsRoot
                .GetComponentsInChildren<AudioSource>(true);

            Undo.RecordObject(controller, "Assign AudioSources");
            controller.audioSources = audioSources;

            EditorUtility.SetDirty(controller);

            EditorUtility.DisplayDialog("Assigned", $"{audioSources.Length} AudioSources assigned to the controller.",
                "OK");
        }


        private void UpdateAllTextLocalizationComponentIdsInScene()
        {
            TextLocalizationComponent[] all = GameObject.FindObjectsOfType<TextLocalizationComponent>(true);
            int count = 0;

            foreach (TextLocalizationComponent component in all)
            {
                Undo.RecordObject(component, "Update Localization ID");
                component.UpdateId();
                EditorUtility.SetDirty(component);
                count++;
            }

            EditorUtility.DisplayDialog("Updated", $"UpdateId() called on {count} components in scene.", "OK");
        }

        private void UpdateAllTextLocalizationComponentIdsInPrefab()
        {
            var prefabStage = PrefabStageUtility.GetCurrentPrefabStage();

            if (prefabStage == null)
            {
                EditorUtility.DisplayDialog("Error", "Not in prefab mode.", "OK");

                return;
            }

            TextLocalizationComponent[] all = prefabStage.prefabContentsRoot
                .GetComponentsInChildren<TextLocalizationComponent>(true);

            int count = 0;

            foreach (TextLocalizationComponent component in all)
            {
                Undo.RecordObject(component, "Update Localization ID");
                component.UpdateId();
                EditorUtility.SetDirty(component);
                count++;
            }

            EditorUtility.DisplayDialog("Updated", $"UpdateId() called on {count} components in prefab.", "OK");
        }

        private void PopulateTextEntriesFromScene()
        {
            var components = GameObject.FindObjectsOfType<TextLocalizationComponent>(true);

            if (components.Length == 0)
            {
                EditorUtility.DisplayDialog("No Components Found", "No TextLocalizationComponent found in the scene.",
                    "OK");

                return;
            }

            _textEntries.Clear();
            int count = 0;

            foreach (var comp in components)
            {
                string key = comp.TextId;
                string localized = comp.GetUkrainianText();

                if (!string.IsNullOrEmpty(key))
                {
                    _textEntries.Add(new TextEntry(key, localized));
                    count++;
                }
            }

            EditorUtility.DisplayDialog("Done", $"{count} text entries added from scene components.", "OK");
        }

        private void PopulateTextEntriesFromPrefab()
        {
            PrefabStage prefabStage = PrefabStageUtility.GetCurrentPrefabStage();

            if (prefabStage == null)
            {
                EditorUtility.DisplayDialog("Error", "Not in prefab mode.", "OK");

                return;
            }

            TextLocalizationComponent[] components = prefabStage.prefabContentsRoot
                .GetComponentsInChildren<TextLocalizationComponent>(true);

            if (components.Length == 0)
            {
                EditorUtility.DisplayDialog("No Components Found", "No TextLocalizationComponent found in the prefab.",
                    "OK");

                return;
            }

            _textEntries.Clear();
            int count = 0;

            foreach (TextLocalizationComponent comp in components)
            {
                string key = comp.TextId;
                string localized = comp.GetUkrainianText();

                if (!string.IsNullOrEmpty(key))
                {
                    _textEntries.Add(new TextEntry(key, localized));
                    count++;
                }
            }

            EditorUtility.DisplayDialog("Done", $"{count} text entries added from prefab components.", "OK");
        }

        private void PopulateVoiceoverEntriesFromScene()
        {
            AudioSource[] sources = FindObjectsOfType<AudioSource>(true);

            List<AudioClip> clips = sources
                .Select(source => source.clip)
                .Where(clip => clip != null)
                .Distinct()
                .ToList();

            if (clips.Count == 0)
            {
                EditorUtility.DisplayDialog("No AudioClips Found", "No AudioClips found on AudioSources in the scene.",
                    "OK");

                return;
            }

            _voiceoverEntries.Clear();

            foreach (var clip in clips)
            {
                _voiceoverEntries.Add(new VoiceoverEntry(clip.name, true, clip));
            }

            EditorUtility.DisplayDialog("Done", $"{clips.Count} voiceover entries added from scene.", "OK");
        }

        private void PopulateVoiceoverEntriesFromPrefab()
        {
            PrefabStage prefabStage = PrefabStageUtility.GetCurrentPrefabStage();

            if (prefabStage == null)
            {
                EditorUtility.DisplayDialog("Error", "Not in prefab mode.", "OK");

                return;
            }

            AudioSource[] sources = prefabStage.prefabContentsRoot
                .GetComponentsInChildren<AudioSource>(true);

            List<AudioClip> clips = sources
                .Select(source => source.clip)
                .Where(clip => clip != null)
                .Distinct()
                .ToList();

            if (clips.Count == 0)
            {
                EditorUtility.DisplayDialog("No AudioClips Found", "No AudioClips found on AudioSources in the prefab.",
                    "OK");

                return;
            }

            _voiceoverEntries.Clear();

            foreach (var clip in clips)
            {
                _voiceoverEntries.Add(new VoiceoverEntry(clip.name, true, clip));
            }

            EditorUtility.DisplayDialog("Done", $"{clips.Count} voiceover entries added from prefab.", "OK");
        }


        private List<TextLocalizationModel> SerializeTextEntries()
        {
            return _textEntries.Select(entry => new TextLocalizationModel()
                { Key = entry.Key, LocalizedText = entry.LocalizedText }).ToList();
        }

        private List<TextureLocalizationModel> SerializeTextureEntries()
        {
            return _textureEntries.Select(entry => new TextureLocalizationModel()
                { Key = entry.Key, MaterialPropertyName = entry.MaterialPropertyName }).ToList();
        }

        private List<VoiceLocalizationModel> SerializeVoiceoverEntries()
        {
            return _voiceoverEntries.Select(entry => new VoiceLocalizationModel() { Key = entry.Key }).ToList();
        }

        private List<MaterialLocalizationModel> SerializeMaterialEntries()
        {
            return _materialEntries.Select(entry => new MaterialLocalizationModel()
            {
                Key = entry.Key,
                MaterialIndex = entry.MaterialIndex
            }).ToList();
        }

        private void DrawMaterialEntries()
        {
            for (int i = 0; i < _materialEntries.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();
                string pathOverride = "";
                Material mat = null;

                bool isPathOverrideToggle = _materialEntries[i].IsPathOverride;

                bool newIsPathOverrideToggle = GUILayout.Toggle(isPathOverrideToggle,
                    new GUIContent("", "Use custom material name"), GUILayout.Width(20));

                if (isPathOverrideToggle)
                {
                    pathOverride = EditorGUILayout.TextField(
                        new GUIContent("Key", "Asset name to load from bundle"), _materialEntries[i].Key);
                }
                else
                {
                    mat = (Material)EditorGUILayout.ObjectField(_materialEntries[i].Material, typeof(Material),
                        allowSceneObjects: false);
                }

                string newKey = isPathOverrideToggle ? pathOverride :
                    _materialEntries[i].Material ? _materialEntries[i].Material.name : "";

                int newMaterialIndex = EditorGUILayout.IntField(
                    new GUIContent("Idx", "Renderer material slot index"), _materialEntries[i].MaterialIndex, GUILayout.Width(40));

                if (GUILayout.Button("X", GUILayout.Width(20)))
                {
                    _materialEntries.RemoveAt(i);
                }
                else
                {
                    _materialEntries[i] = new MaterialEntry(newKey, newMaterialIndex, newIsPathOverrideToggle, mat);
                }

                EditorGUILayout.EndHorizontal();
            }

            if (GUILayout.Button("Add Material Entry"))
            {
                _materialEntries.Add(new MaterialEntry("NewMaterialKey", 0, false, null));
            }
        }

        private void PopulateTextureEntriesFromScene()
        {
            TextureLocalizationComponent[] components =
                GameObject.FindObjectsOfType<TextureLocalizationComponent>(true);

            if (components.Length == 0)
            {
                EditorUtility.DisplayDialog("No Components Found",
                    "No TextureLocalizationComponent found in the scene.", "OK");
                return;
            }

            _textureEntries.Clear();
            HashSet<string> seen = new HashSet<string>();
            int count = 0;

            foreach (TextureLocalizationComponent comp in components)
            {
                string key = comp.TextureId;
                if (!string.IsNullOrEmpty(key) && seen.Add(key))
                {
                    _textureEntries.Add(new TextureEntry(key, "", true, null));
                    count++;
                }
            }

            EditorUtility.DisplayDialog("Done", $"{count} texture entries added from scene components.", "OK");
        }

        private void PopulateTextureEntriesFromPrefab()
        {
            PrefabStage prefabStage = PrefabStageUtility.GetCurrentPrefabStage();

            if (prefabStage == null)
            {
                EditorUtility.DisplayDialog("Error", "Not in prefab mode.", "OK");
                return;
            }

            TextureLocalizationComponent[] components = prefabStage.prefabContentsRoot
                .GetComponentsInChildren<TextureLocalizationComponent>(true);

            if (components.Length == 0)
            {
                EditorUtility.DisplayDialog("No Components Found",
                    "No TextureLocalizationComponent found in the prefab.", "OK");
                return;
            }

            _textureEntries.Clear();
            HashSet<string> seen = new HashSet<string>();
            int count = 0;

            foreach (TextureLocalizationComponent comp in components)
            {
                string key = comp.TextureId;
                if (!string.IsNullOrEmpty(key) && seen.Add(key))
                {
                    _textureEntries.Add(new TextureEntry(key, "", true, null));
                    count++;
                }
            }

            EditorUtility.DisplayDialog("Done", $"{count} texture entries added from prefab components.", "OK");
        }

        private void PopulateMaterialEntriesFromScene()
        {
            MaterialLocalizationComponent[] components =
                GameObject.FindObjectsOfType<MaterialLocalizationComponent>(true);

            if (components.Length == 0)
            {
                EditorUtility.DisplayDialog("No Components Found",
                    "No MaterialLocalizationComponent found in the scene.", "OK");
                return;
            }

            _materialEntries.Clear();

            foreach (MaterialLocalizationComponent comp in components)
                _materialEntries.Add(new MaterialEntry("", comp.MaterialIndex, false, null));

            EditorUtility.DisplayDialog("Done", $"{components.Length} material entries added from scene components.", "OK");
        }

        private void PopulateMaterialEntriesFromPrefab()
        {
            PrefabStage prefabStage = PrefabStageUtility.GetCurrentPrefabStage();

            if (prefabStage == null)
            {
                EditorUtility.DisplayDialog("Error", "Not in prefab mode.", "OK");
                return;
            }

            MaterialLocalizationComponent[] components = prefabStage.prefabContentsRoot
                .GetComponentsInChildren<MaterialLocalizationComponent>(true);

            if (components.Length == 0)
            {
                EditorUtility.DisplayDialog("No Components Found",
                    "No MaterialLocalizationComponent found in the prefab.", "OK");
                return;
            }

            _materialEntries.Clear();

            foreach (MaterialLocalizationComponent comp in components)
                _materialEntries.Add(new MaterialEntry("", comp.MaterialIndex, false, null));

            EditorUtility.DisplayDialog("Done", $"{components.Length} material entries added from prefab components.", "OK");
        }
    }

    internal class TextEntry
    {
        public string Key;
        public string LocalizedText;

        public TextEntry(string key, string localizedText)
        {
            Key = key;
            LocalizedText = localizedText;
        }
    }

    internal class TextureEntry
    {
        public string Key;
        public string MaterialPropertyName;
        public bool IsPathOverride;
        public Texture Texture;

        public TextureEntry(string key, string materialPropertyName, bool isPathOverride, Texture texture)
        {
            Key = key;
            MaterialPropertyName = materialPropertyName;
            IsPathOverride = isPathOverride;
            Texture = texture;
        }
    }

    internal class VoiceoverEntry
    {
        public string Key;
        public bool IsPathOverride;
        public AudioClip AudioClip;

        public VoiceoverEntry(string key, bool isPathOverride, AudioClip audioClip)
        {
            Key = key;
            IsPathOverride = isPathOverride;
            AudioClip = audioClip;
        }
    }

    internal class MaterialEntry
    {
        public string Key; // asset name in bundle (locale-specific)
        public int MaterialIndex;
        public bool IsPathOverride;
        public Material Material;

        public MaterialEntry(string key, int materialIndex, bool isPathOverride, Material material)
        {
            Key = key;
            MaterialIndex = materialIndex;
            IsPathOverride = isPathOverride;
            Material = material;
        }
    }
}