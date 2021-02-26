using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public static class RuntimeURDF
{
    public static T AssetDatabase_LoadAssetAtPath<T>(string fileAssetPath) where T : UnityEngine.Object 
    {
#if UNITY_EDITOR
        return AssetDatabase.LoadAssetAtPath<T>(fileAssetPath);
#else
        return default(T);
#endif        
    }  

    public static int EditorUtility_DisplayDialogComplex(string title, string message, string ok, string cancel, string alt) 
    {
#if UNITY_EDITOR
        return EditorUtility.DisplayDialogComplex(title, message, ok, cancel, alt);
#else
    return 0;
#endif        
    }

    public static UnityEngine.Object AssetDatabase_LoadAssetAtPath(string assetPath, Type type) 
    {
#if UNITY_EDITOR
        return AssetDatabase.LoadAssetAtPath(assetPath, type);
#else
        return null;
#endif        
    }

    public static string EditorUtility_OpenFolderPanel(string title, string folder, string defaultName) 
    {
#if UNITY_EDITOR
        return EditorUtility_OpenFolderPanel(title, folder, defaultName);
#else
    return "";
#endif        
    }

    public static string EditorUtility_OpenFilePanel(string title, string directory, string extension) 
    {
#if UNITY_EDITOR
        return EditorUtility.OpenFilePanel(title, directory, extension);
#else
    return "";
#endif        
    }

    public static bool EditorUtility_DisplayDialog(string title, string message, string ok, [UnityEngine.Internal.DefaultValue("\"\"")] string cancel) 
    {
#if UNITY_EDITOR
        return EditorUtility.DisplayDialog(title, message, ok, cancel);
#else
    return false;
#endif        
    }

    public static bool AssetDatabase_IsValidFolder(string path) 
    {
#if UNITY_EDITOR
        return AssetDatabase.IsValidFolder(path);
#else
        return false;
#endif        
    }

    public static string AssetDatabase_CreateFolder(string parentFolder, string newFolderName) 
    {
#if UNITY_EDITOR
        return AssetDatabase.CreateFolder(parentFolder, newFolderName);
#else
    return "";
#endif        
    }

    public static string AssetDatabase_MoveAsset(string oldPath, string newPath) 
    {
#if UNITY_EDITOR
        return AssetDatabase.MoveAsset(oldPath, newPath);
#else
    return "";
#endif        
    }

    public static string[] AssetDatabase_FindAssets(string filter, string[] searchInFolders)
    {
#if UNITY_EDITOR
        return AssetDatabase.FindAssets(filter, searchInFolders);
#else
    return [];
#endif        
    }

    public static string AssetDatabase_GUIDToAssetPath(string guid) 
    {
#if UNITY_EDITOR
        return AssetDatabase.GUIDToAssetPath(guid);
#else
    return "";
#endif        
    }

    public static string AssetDatabase_GetAssetPath(UnityEngine.Object assetObject) 
    {
#if UNITY_EDITOR
        return AssetDatabase.GetAssetPath(assetObject);
#else
    return "";
#endif        
    }

    public static TObject PrefabUtility_GetCorrespondingObjectFromSource<TObject>(TObject componentOrGameObject) where TObject : UnityEngine.Object 
    {
#if UNITY_EDITOR
        return PrefabUtility.GetCorrespondingObjectFromSource(componentOrGameObject);
#else
        return default(TObject);
#endif        
    }

    public static GameObject PrefabUtility_SaveAsPrefabAsset(GameObject instanceRoot, string assetPath) 
    {
#if UNITY_EDITOR
        return PrefabUtility.SaveAsPrefabAsset(instanceRoot, assetPath);
#else
    return null;
#endif        
    }

    public static T AssetDatabase_GetBuiltinExtraResource<T>(string path) where T : UnityEngine.Object 
    {
#if UNITY_EDITOR
        return AssetDatabase.GetBuiltinExtraResource<T>(path);
#else
        return default(T);
#endif        
    }

    public static void AssetDatabase_CreateAsset(UnityEngine.Object asset, string path) 
    {
#if UNITY_EDITOR
        AssetDatabase.CreateAsset(asset, path);
#endif        
    }

    public static UnityEngine.Object PrefabUtility_InstantiatePrefab(UnityEngine.Object assetComponentOrGameObject) 
    {
#if UNITY_EDITOR
        return PrefabUtility.InstantiatePrefab(assetComponentOrGameObject);
#else
        return null;
#endif        
    }
}
