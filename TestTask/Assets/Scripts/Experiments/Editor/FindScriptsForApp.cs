#if UNITY_EDITOR

using System.IO;
using UnityEditor;
using UnityEngine;

public class FindScriptsForApp : MonoBehaviour
{
    private const string PathResources = "Assets/Resources/TempAssetsBundle/";
    private const string PathResultAndroid = PathResources + "ScriptsForApp/";
    private const string FilterFileByLabel = "InApp";

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
    
    [MenuItem("AssetsBundles/")]
    [MenuItem("AssetsBundles/Find scripts for app")]
    private static void AutoBuildCopyTempFilesAndroid()
    {
        CopyTempFiles();
    }
    
    private static void CopyTempFiles()
    {
        RemoveAllFileFromExportBy(PathResultAndroid);
        CheckFolder();

        for (int i = 0; i < AllFileNames.Length; i++)
        {
            var fileNameFull = AllFileNames[i];
            var assetPath = AssetDatabase.GUIDToAssetPath(fileNameFull);
            var fileName = Path.GetFileName(assetPath);
            var sourcePath = PathResultAndroid + fileName;
            File.Copy(assetPath, sourcePath);
        }

        AssetDatabase.Refresh();
        OpenExplorerBy(PathResultAndroid);
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
    
    private static void CheckFolder()
    {
        //Create folders
        if (!Directory.Exists(PathResultAndroid))
        {
            Directory.CreateDirectory(PathResultAndroid);
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