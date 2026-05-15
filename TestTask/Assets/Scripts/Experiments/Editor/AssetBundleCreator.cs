#if UNITY_EDITOR

using System;
using System.IO;
using UnityEditor;
using UnityEngine;

public class AssetBundleCreator : MonoBehaviour
{
    private const string PathTempFolder = "TempAssetsBundle/";
    private const string PathResources = "Assets/Resources/" + PathTempFolder;
    private const string PathResultIOS = PathResources + "iOS/";
    private const string PathResultAndroid = PathResources + "Android/";
    private const string PathResultPrefabs = PathResources + "Prefabs/";
    private const string PathResourcePrefabs = PathTempFolder + "Prefabs/";
    private const string FilterFileByLabel = "Bundle";

    private static string[] AllFileNames
    {
        get
        {
            var guids = AssetDatabase.FindAssets("l:" + FilterFileByLabel, null);
            var result = new string[guids.Length];

            for (var i = 0; i < guids.Length; i++)
            {
                result[i] = guids[i];
            }

            return result;
        }
    }


    [MenuItem("AssetsBundles/Set Asset Bundle From File Name", false, 0)]
    static void SetAssetBundlesFromFileNames()
    {
        if (Selection.assetGUIDs.Length > 0)
        {
            string nameAsset = null;
            foreach (var asset in Selection.objects)
            {
                string path = AssetDatabase.GetAssetPath(asset);
                AssetImporter assetImporter = AssetImporter.GetAtPath(path);
                nameAsset = asset.name;
                nameAsset = nameAsset.Replace(" ", "");
                assetImporter.assetBundleName = nameAsset;
                Debug.Log(Selection.assetGUIDs.Length + " Asset Bundles Assigned");
            }
        }
        else
        {
            Debug.Log("No Assets Selected");
        }
        AssetDatabase.Refresh();
    }

    [MenuItem("AssetsBundles/Split into UA-EN", false,0)]
    static void SplitIntoUAEN()
    {
        Split_Android();
        Split_iOS();
        AssetDatabase.Refresh();
    }

    private static void Split_Android()
    {
        if (!Directory.Exists("AssetBundles/Android") || !Directory.Exists("AssetBundles/iOS"))
        {
            return;
        }
       
        if (!Directory.Exists("AssetBundles/Android/UA"))
        {
            if (!Directory.Exists("AssetBundles/Android/UA"))
            {
                Directory.CreateDirectory("AssetBundles/Android/UA");
            }
        }
        if (!Directory.Exists("AssetBundles/Android/EN"))
        {
            if (!Directory.Exists("AssetBundles/Android/EN"))
            {
                Directory.CreateDirectory("AssetBundles/Android/EN");
            }
        }

        var dir = new DirectoryInfo("AssetBundles/Android");

        var info_ua = dir.GetFiles("*ua.*");
        
        for (int i = 0; i < info_ua.Length; i++)
        {
            var file = info_ua[i];
            var assetPath = file.FullName;
            var fileName = Path.GetFileName(assetPath);
            var sourcePath = "AssetBundles/Android/UA/" + fileName;
            File.Move(assetPath, sourcePath);
        }

        var info_en = dir.GetFiles("*en.*");
        
        for (int i = 0; i < info_en.Length; i++)
        {
            var file = info_en[i];
            var assetPath = file.FullName;
            var fileName = Path.GetFileName(assetPath);
            var sourcePath = "AssetBundles/Android/EN/" + fileName;
            File.Move(assetPath, sourcePath);
        }
    }

    private static void Split_iOS()
    {
        if (!Directory.Exists("AssetBundles/iOS"))
        {
            return;
        }
       
        if (!Directory.Exists("AssetBundles/iOS/UA"))
        {
            if (!Directory.Exists("AssetBundles/iOS/UA"))
            {
                Directory.CreateDirectory("AssetBundles/iOS/UA");
            }
        }
        if (!Directory.Exists("AssetBundles/iOS/EN"))
        {
            if (!Directory.Exists("AssetBundles/iOS/EN"))
            {
                Directory.CreateDirectory("AssetBundles/iOS/EN");
            }
        }

        var dir = new DirectoryInfo("AssetBundles/iOS");

        var info_ua = dir.GetFiles("*ua.*");
        
        for (int i = 0; i < info_ua.Length; i++)
        {
            var file = info_ua[i];
            var assetPath = file.FullName;
            var fileName = Path.GetFileName(assetPath);
            var sourcePath = "AssetBundles/iOS/UA/" + fileName;
            File.Move(assetPath, sourcePath);
        }

        var info_en = dir.GetFiles("*en.*");
        
        for (int i = 0; i < info_en.Length; i++)
        {
            var file = info_en[i];
            var assetPath = file.FullName;
            var fileName = Path.GetFileName(assetPath);
            var sourcePath = "AssetBundles/iOS/EN/" + fileName;
            File.Move(assetPath, sourcePath);
        } 
    }


    //Auto build menu
    [MenuItem("AssetsBundles/Auto Build/Copy temp files")]
    private static void AutoBuildCopyTempFilesAndroid()
    {
        CopyTempFiles();
    }

    [MenuItem("AssetsBundles/Auto Build/Build Android")]
    private static void AutoBuildAndroid()
    {
        SetAssetBundleNameTempFiles();
        BuildAssetBundles_Android();
    }

    [MenuItem("AssetsBundles/Auto Build/Build iOS")]
    private static void AutoBuild_iOS()
    {
        SetAssetBundleNameTempFiles();
        BuildAssetBundles_iOS();
    }

    [MenuItem("AssetsBundles/Remove temp resources")]
    private static void RemoveTempResources()
    {
        RemoveTemp();
    }

    //Manual build menu by steps
    [MenuItem("AssetsBundles/ManualBuild/Step 1 CreateTempFolders", false, 1)]
    private static void CheckFolder()
    {
        //Create folders
        if (!Directory.Exists(PathResultAndroid))
        {
            Directory.CreateDirectory(PathResultAndroid);
        }

        if (!Directory.Exists(PathResultIOS))
        {
            Directory.CreateDirectory(PathResultIOS);
        }

        if (!Directory.Exists(PathResultPrefabs))
        {
            Directory.CreateDirectory(PathResultPrefabs);
        }

        AssetDatabase.Refresh();
    }

    [MenuItem("AssetsBundles/ManualBuild/Step 2 CopyTempFiles", false, 1)]
    private static void CopyTempFiles()
    {
        RemoveAllFile();
        CheckFolder();

        for (int i = 0; i < AllFileNames.Length; i++)
        {
            var fileNameFull = AllFileNames[i];
            var assetPath = AssetDatabase.GUIDToAssetPath(fileNameFull);
            var fileName = Path.GetFileName(assetPath);
            var sourcePath = PathResultPrefabs + fileName;
            File.Copy(assetPath, sourcePath);
        }

        AssetDatabase.Refresh();
        OpenExplorerBy(PathResultPrefabs);
    }

    [MenuItem("AssetsBundles/ManualBuild/Step 3 SetAssetBundleName", false, 1)]
    private static void SetAssetBundleNameTempFiles()
    {
        SetNameAssetBundleFiles();
    }

    [MenuItem("AssetsBundles/ManualBuild/Step 4 BuildAssetBundles Android", false, 1)]
    private static void BuildAssetBundles_Android()
    {
        TryBuildAssetBundles(BuildTarget.Android);
    }

    [MenuItem("AssetsBundles/ManualBuild/Step 5 BuildAssetBundles iOS", false, 1)]
    private static void BuildAssetBundles_iOS()
    {
        TryBuildAssetBundles(BuildTarget.iOS);
    }

    [MenuItem("AssetsBundles/ManualBuild/Clear asset name")]
    private static void ClearAssetName()
    {
        SetNameAssetBundleFiles("");
    }

    [MenuItem("AssetsBundles/ManualBuild/Remove temp AssetBundles")]
    private static void RemoveAssetBundles()
    {
        RemoveAllFileFromExportBy(PathResultIOS);
        RemoveAllFileFromExportBy(PathResultAndroid);
        AssetDatabase.Refresh();
    }

    [MenuItem("AssetsBundles/ManualBuild/Remove temp prefabs")]
    private static void RemoveImport()
    {
        RemoveAllFileFromExportBy(PathResultPrefabs);
        AssetDatabase.Refresh();
    }

    [MenuItem("AssetsBundles/ManualBuild/Remove all temp files")]
    private static void RemoveAllFile()
    {
        RemoveAssetBundles();
        RemoveImport();
        AssetDatabase.Refresh();
    }

    [MenuItem("AssetsBundles/ManualBuild/Remove temp folder")]
    private static void RemoveTemp()
    {
        FileUtil.DeleteFileOrDirectory(PathResources);
        AssetDatabase.Refresh();
    }

    private static void RemoveAllFileFromExportBy(string path)
    {
        var dir = new DirectoryInfo(path);
        if (!Directory.Exists(path))
        {
            return;
        }

        var info = dir.GetFiles("*.*");

        if (info.Length == 0) return;

        for (int i = 0; i < info.Length; i++)
        {
            var file = info[i];
            var fileNameDelete = path + file.Name;
            FileUtil.DeleteFileOrDirectory(fileNameDelete);
        }

        AssetDatabase.Refresh();
    }

    private static void TryBuildAssetBundles(BuildTarget targetPlatform)
    {
        string outputPath = PathResultIOS;

        switch (targetPlatform)
        {
            case BuildTarget.Android:
                outputPath = PathResultAndroid;
                break;
            case BuildTarget.iOS:
                outputPath = PathResultIOS;
                break;
        }

        try
        {
            BuildPipeline.BuildAssetBundles(outputPath, BuildAssetBundleOptions.CollectDependencies, targetPlatform);
            AssetDatabase.Refresh();
        }
        catch (Exception e)
        {
            // ignored
        }

        OpenExplorerBy(outputPath);
    }

    private static void SetNameAssetBundleFiles(string newName = null)
    {
        var dir = new DirectoryInfo(PathResultPrefabs);
        if (!Directory.Exists(PathResultPrefabs))
        {
            return;
        }

        var info = dir.GetFiles("*.prefab");

        if (info.Length == 0) return;

        for (int i = 0; i < info.Length; i++)
        {
            var file = info[i];
            var filename = file.Name.Replace(file.Extension, "");
            var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(filename);
            var go = Resources.Load<GameObject>(PathResourcePrefabs + fileNameWithoutExtension);
            var assetPath = AssetDatabase.GetAssetPath(go);

            string selectName = newName != null ? newName : fileNameWithoutExtension;
            var assetImporter = AssetImporter.GetAtPath(assetPath);
            assetImporter.SetAssetBundleNameAndVariant(selectName, "");
        }

        AssetDatabase.Refresh();
    }

    private static void OpenExplorerBy(string outputPath)
    {
        outputPath = outputPath.Replace(@"/", @"\");
        System.Diagnostics.Process.Start("explorer.exe", outputPath);
        // System.Diagnostics.Process.Start("explorer.exe", "/select," + outputPath); // For open parent folder
    }
}
#endif