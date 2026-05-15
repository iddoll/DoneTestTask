using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CreateCustomMaterial))]
public class CreateCustomMaterial : EditorWindow
{
    private static Material _mat;
    
    [MenuItem("Assets/Create/Material (FlexReality)/Standart_FlexReality", false, 1)]

    static void Standard_FlexReality()
    {
        _mat = new Material(Shader.Find("Custom_FlexReality/Standart_FlexReality_Shader/Standart_FlexReality_Shader"));
        CreateMaterials();
    }

    [MenuItem("Assets/Create/Material (FlexReality)/PBR_FlexReality_Shader", false, 1)]
    static void CreatePBR_FlexReality_Shader()
    {
        _mat = new Material(Shader.Find("Custom_FlexReality/PBR_FlexReality_Shader"));
        CreateMaterials();
    }
    
    static void CreateMaterials()
    {
        ProjectWindowUtil.CreateAsset(_mat, "New_Material" + ".mat");
    }
    

    // private static string GetClickedDirFullPath()
    // {
    //     string clickedAssetGuid = Selection.assetGUIDs[0];
    //     string clickedPath      = AssetDatabase.GUIDToAssetPath(clickedAssetGuid);
    //     string clickedPathFull  = Path.Combine(Directory.GetCurrentDirectory(), clickedPath);
    //
    //     FileAttributes attr = File.GetAttributes(clickedPathFull);
    //     return attr.HasFlag(FileAttributes.Directory) ? clickedPathFull : Path.GetDirectoryName(clickedPathFull);
    // }

}