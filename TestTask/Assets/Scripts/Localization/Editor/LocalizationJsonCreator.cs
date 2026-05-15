using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Features.Localization.Models;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;

namespace Features.Localization.Editor
{
    public class LocalizationJsonCreator
    {
        private const string AudioFileName = "AudiosForLocalization";
        private const string TextsFileName = "TextsForLocalization";

        private static List<string> _splitList = new ();
        private static string _jsonText = "";

        [MenuItem("Tools/Localization/Generate localization JSON")]
        public static void GenerateLocalizationJson()
        {
            TextAsset textAsset = Resources.Load<TextAsset>("Localization/TextsForLocalization");
            
            if (textAsset == null)
            {
                Debug.LogError("File 'ForLocalization.json' not found in Resources folder.");
                return;
            }

            _jsonText = textAsset.text;

            if (string.IsNullOrEmpty(_jsonText))
            {
                Debug.LogWarning("JSON file is empty.");
                return;
            }
            
            _splitList = new List<string>(_jsonText.Split("//"));

            Debug.Log("JSON text successfully split into " + _splitList.Count + " parts.");

            LocalizationModel generateLocalizationModel = new LocalizationModel()
            {
                Texts = GetTextLocalizationModel()
            };
            
            GenerateJsonFile(generateLocalizationModel);
        }

        private static List<TextLocalizationModel> GetTextLocalizationModel()
        {
            List<TextLocalizationModel> textLocalizationModels = new List<TextLocalizationModel>();

            // textLocalizationModels.AddRange(_splitList.Select(x => new TextLocalizationModel
            // {
            //      Key = ValidateAndFormatString(x)
            // }));

            return textLocalizationModels;
        }
        
        private static string ValidateAndFormatString(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return string.Empty;

            input = Regex.Replace(input, @"^[^\wА-Яа-яЁё]+|[^\wА-Яа-яЁё.]+$", "");
            input = Regex.Replace(input, @"\.{2,}", ".");
            input = Regex.Replace(input, @"\s{2,}", " ").Trim();

            return input;
        }
        
        private static void GenerateJsonFile(LocalizationModel model)
        {
            if (model == null)
            {
                Debug.LogError("GenerateLocalizationModel is null. Please assign it.");
                return;
            }

            try
            {
                string jsonData = JsonConvert.SerializeObject(model, Formatting.Indented);

                string directoryPath = Path.Combine(Application.dataPath, "Resources", "Localization");
                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }

                string baseFileName = "GeneratedLocalization";
                string filePath = GetUniqueFilePath(directoryPath, baseFileName);

                File.WriteAllText(filePath, jsonData);

                Debug.Log("JSON файл успішно створено: " + filePath);

#if UNITY_EDITOR
                AssetDatabase.Refresh();
#endif
            }
            catch (System.Exception ex)
            {
                Debug.LogError("Помилка під час створення JSON файлу: " + ex.Message);
            }
        }

        private static string GetUniqueFilePath(string directoryPath, string baseFileName)
        {
            string filePath = Path.Combine(directoryPath, baseFileName + ".json");
            int counter = 1;

            while (File.Exists(filePath))
            {
                filePath = Path.Combine(directoryPath, $"{baseFileName} ({counter}).json");
                counter++;
            }

            return filePath;
        }
        
        [MenuItem("Tools/Localization/Create TextsForLocalization file")]
        public static void GenerateTextsForLocalizationFile() => CreateFile(TextsFileName);

        [MenuItem("Tools/Localization/Create AudiosForLocalization file")]
        public static void GenerateAudiosForLocalizationFile() => CreateFile(AudioFileName);

        private static void CreateFile(string fileName)
        {
            string directoryPath = Path.Combine(Application.dataPath, "Resources", "Localization");
            string filePath = Path.Combine(directoryPath, $"{fileName}.txt");

            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
                Debug.Log("Створено директорію: " + directoryPath);
            }

            if (File.Exists(filePath))
            {
                Debug.LogWarning("Файл вже існує: " + filePath);
                return;
            }

            string defaultContent = "Введіть елементи озвучки або тексту тут\nКожен елемент розділяйте символами \"//\"";
            File.WriteAllText(filePath, defaultContent);

            Debug.Log("Файл успішно створено: " + filePath);

#if UNITY_EDITOR
            AssetDatabase.Refresh();
#endif
        }
    }
}
