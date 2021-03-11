using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public static class RuntimeURDF
{
#if UNITY_EDITOR
    public static bool isRuntimeMode = false;
#else
    public static bool isRuntimeMode = true;
#endif

    public static T AssetDatabase_LoadAssetAtPath<T>(string fileAssetPath) where T : UnityEngine.Object 
    {
#if UNITY_EDITOR
        if (!isRuntimeMode)
        {
            return AssetDatabase.LoadAssetAtPath<T>(fileAssetPath);
        }
#endif
        return default(T);
    }  

    public static int EditorUtility_DisplayDialogComplex(string title, string message, string ok, string cancel, string alt) 
    {
#if UNITY_EDITOR
        if (!isRuntimeMode)
        {
            return EditorUtility.DisplayDialogComplex(title, message, ok, cancel, alt);
        }
#endif
        return 0;
    }

    public static UnityEngine.Object AssetDatabase_LoadAssetAtPath(string assetPath, Type type) 
    {
#if UNITY_EDITOR
        if (!isRuntimeMode)
        {
            return AssetDatabase.LoadAssetAtPath(assetPath, type);
        }
#endif
        return null;
    }

    public static string EditorUtility_OpenFolderPanel(string title, string folder, string defaultName) 
    {
#if UNITY_EDITOR
        if (!isRuntimeMode)
        {
            return EditorUtility_OpenFolderPanel(title, folder, defaultName);
        }
#endif
    return "";
    }

    public static string EditorUtility_OpenFilePanel(string title, string directory, string extension) 
    {
#if UNITY_EDITOR
        if (!isRuntimeMode)
        {
            return EditorUtility.OpenFilePanel(title, directory, extension);
        }
#endif
        return "";
    }

    public static bool EditorUtility_DisplayDialog(string title, string message, string ok, [UnityEngine.Internal.DefaultValue("\"\"")] string cancel) 
    {
#if UNITY_EDITOR
        if (!isRuntimeMode)
        {
            return EditorUtility.DisplayDialog(title, message, ok, cancel);
        }
#endif
        return false;
    }

    public static bool AssetDatabase_IsValidFolder(string path) 
    {
#if UNITY_EDITOR
        if (!isRuntimeMode)
        {
            return AssetDatabase.IsValidFolder(path);
        }
#endif
        return false;
    }

    public static string AssetDatabase_CreateFolder(string parentFolder, string newFolderName) 
    {
#if UNITY_EDITOR
        if (!isRuntimeMode)
        {
            return AssetDatabase.CreateFolder(parentFolder, newFolderName);
        }
#endif
    return "";
    }

    public static string AssetDatabase_MoveAsset(string oldPath, string newPath) 
    {
#if UNITY_EDITOR
        if (!isRuntimeMode)
        {
            return AssetDatabase.MoveAsset(oldPath, newPath);
        }
#endif
        return "";
    }

    public static string[] AssetDatabase_FindAssets(string filter, string[] searchInFolders)
    {
#if UNITY_EDITOR
        if (!isRuntimeMode)
        {
            return AssetDatabase.FindAssets(filter, searchInFolders);
        }
#endif
        return new string[0];
    }

    public static string AssetDatabase_GUIDToAssetPath(string guid) 
    {
#if UNITY_EDITOR
        if (!isRuntimeMode)
        {
            return AssetDatabase.GUIDToAssetPath(guid);
        }
#endif
        return "";
    }

    public static string AssetDatabase_GetAssetPath(UnityEngine.Object assetObject) 
    {
#if UNITY_EDITOR
        if (!isRuntimeMode)
        {
            return AssetDatabase.GetAssetPath(assetObject);
        }
#endif
        return "";
    }

    public static TObject PrefabUtility_GetCorrespondingObjectFromSource<TObject>(TObject componentOrGameObject) where TObject : UnityEngine.Object 
    {
#if UNITY_EDITOR
        if (!isRuntimeMode)
        {
            return PrefabUtility.GetCorrespondingObjectFromSource(componentOrGameObject);
        }
#endif
        return default(TObject);
    }

    public static GameObject PrefabUtility_SaveAsPrefabAsset(GameObject instanceRoot, string assetPath) 
    {
#if UNITY_EDITOR
        if (!isRuntimeMode)
        {
            return PrefabUtility.SaveAsPrefabAsset(instanceRoot, assetPath);
        }
#endif
        return null;
    }

    public static T AssetDatabase_GetBuiltinExtraResource<T>(string path) where T : UnityEngine.Object 
    {
#if UNITY_EDITOR
        if (!isRuntimeMode)
        {
            return AssetDatabase.GetBuiltinExtraResource<T>(path);
        }
#endif
        return default(T);
    }

    public static void AssetDatabase_CreateAsset(UnityEngine.Object asset, string path) 
    {
#if UNITY_EDITOR
        if (!isRuntimeMode)
        {
            AssetDatabase.CreateAsset(asset, path);
        }
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
