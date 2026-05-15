using System;
using System.Collections.Generic;
using System.IO;
using Features.Localization.Components;
using Features.Localization.Models.Enums;
using UnityEditor;
using UnityEngine;

namespace Experiments.Features.Localization.Editor
{
    [CustomEditor(typeof(ExperimentLocalizationController))]
    public class ExperimentLocalizationControllerEditor : UnityEditor.Editor
    {
        private readonly List<(Locale locale, string bundleName)> _foundBundles = new();

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(serializedObject.FindProperty("audioSources"));

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Editor Testing", EditorStyles.boldLabel);

            SerializedProperty loadProp = serializedObject.FindProperty("loadFromStreamingAssets");
            SerializedProperty nameProp = serializedObject.FindProperty("bundleName");

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(loadProp);
            if (GUILayout.Button("Find Localizations", GUILayout.Width(145)))
                FindLocalizations((ExperimentLocalizationController)target);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.PropertyField(nameProp);

            if (_foundBundles.Count > 0)
            {
                EditorGUILayout.Space(4);
                EditorGUILayout.LabelField("Available bundles:", EditorStyles.miniLabel);

                foreach ((Locale locale, string bundleName) in _foundBundles)
                {
                    bool isCurrent = nameProp.stringValue == bundleName;

                    GUIStyle style = new GUIStyle(GUI.skin.button);
                    if (isCurrent) style.normal.textColor = Color.green;

                    if (GUILayout.Button($"[{locale}]  {bundleName}", style))
                        nameProp.stringValue = bundleName;
                }
            }

            serializedObject.ApplyModifiedProperties();
        }

        private void FindLocalizations(ExperimentLocalizationController controller)
        {
            _foundBundles.Clear();

            string streamingPath = Application.streamingAssetsPath;
            if (!Directory.Exists(streamingPath))
            {
                Debug.LogWarning("[Localization] StreamingAssets folder not found.");
                return;
            }

            // Bundle name formula (LocalizationEditorWindow.AssignObjectsToLocalizationAssetBundle):
            //   "localization_" + locale.ToString() + "_" + _experimentName
            // Unity lowercases assetBundleNames on build, so file is:
            //   localization_{locale.lower()}_{experimentName.lower()}
            //
            // GameObject.name may differ from _experimentName used in the editor:
            //   e.g. GameObject = "Biology_8ex83_ua"  →  bundle = "localization_ua_biology_8ex83_loc"
            //
            // We generate candidate experiment names to cover known naming patterns.

            foreach (Locale locale in Enum.GetValues(typeof(Locale)))
            {
                foreach (string candidate in GetCandidateExpNames(controller.gameObject.name, locale))
                {
                    string expectedName = $"localization_{locale.ToString().ToLower()}_{candidate}";
                    string fullPath = Path.Combine(streamingPath, expectedName);

                    if (File.Exists(fullPath))
                    {
                        _foundBundles.Add((locale, expectedName));
                        break; // один бандл на локаль — далі не перебираємо
                    }
                }
            }

            if (_foundBundles.Count == 0)
                Debug.LogWarning($"[Localization] No bundles found in StreamingAssets for '{controller.gameObject.name}'.");
            else
                Debug.Log($"[Localization] Found {_foundBundles.Count} bundle(s) for '{controller.gameObject.name}'.");
        }

        /// <summary>
        /// Генерує варіанти імені експерименту з gameObject.name для пошуку бандлу.
        ///
        /// Проблема: ім'я GameObject і ім'я що вводилось у Localization Editor можуть відрізнятись:
        ///   GameObject "Biology_8ex83_ua"  →  experiment name в едиторі "Biology_8ex83_loc"
        ///   GameObject "bio_7ex311_loc"    →  experiment name "bio_7ex311_loc"  (збігається)
        ///
        /// Стратегія:
        ///   1. Ім'я як є (нижній регістр)
        ///   2. Якщо закінчується на суфікс поточної локалі (_ua, _en тощо) — відрізати його
        ///   3. Якщо не закінчується на _loc — спробувати з _loc
        ///   4. Комбінація: відрізали суфікс локалі + додали _loc
        /// </summary>
        private static IEnumerable<string> GetCandidateExpNames(string gameObjectName, Locale locale)
        {
            string lower = gameObjectName.ToLower();
            string localeSuffix = "_" + locale.ToString().ToLower(); // "_ua", "_en", ...

            // 1. Ім'я як є
            yield return lower;

            // 2. Без суфікса локалі (якщо є)
            string stripped = lower.EndsWith(localeSuffix)
                ? lower.Substring(0, lower.Length - localeSuffix.Length)
                : null;

            if (stripped != null)
                yield return stripped;

            // 3. Ім'я як є + _loc
            if (!lower.EndsWith("_loc"))
                yield return lower + "_loc";

            // 4. Без суфікса локалі + _loc
            if (stripped != null && !stripped.EndsWith("_loc"))
                yield return stripped + "_loc";
        }
    }
}
