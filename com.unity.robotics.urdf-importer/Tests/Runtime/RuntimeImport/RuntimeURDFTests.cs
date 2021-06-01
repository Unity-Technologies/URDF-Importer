using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEditor;
using UnityEngine.TestTools;
using RosSharp.Urdf;

public class RuntimeURDFTests
{
    string createAssetPath = "Assets/Tests/Runtime";
    string createFolderPath = "Assets/Tests/Runtime/RuntimeURDF";
    UnityEngine.Object testObject;

    [SetUp]
    public void SetUp()
    {
        RuntimeURDF.runtimeModeEnabled = false;
        AssetDatabase.CreateFolder("Assets", "Tests");
        AssetDatabase.CreateFolder("Assets/Tests", "Runtime");

        testObject = PrefabUtility.SaveAsPrefabAsset(new GameObject("TestAsset"), $"{createAssetPath}/TestAsset.prefab");
    }

    [Test]
    public void IsRuntimeMode_True()
    {
        RuntimeURDF.runtimeModeEnabled = true;
        Assert.IsTrue(RuntimeURDF.IsRuntimeMode());
    }

    [Test]
    public void SetRuntimeMode_False()
    {
        RuntimeURDF.SetRuntimeMode(false);
        Assert.IsFalse(RuntimeURDF.IsRuntimeMode());
    }

    [Test]
    public void AssetDatabase_LoadAssetAtPath_Script()
    {
        RuntimeURDF.SetRuntimeMode(false);
        string path = "Packages/com.unity.robotics.urdf-importer/Tests/Runtime/RuntimeImport/RuntimeURDFTests.cs";
        Assert.IsNotNull(RuntimeURDF.AssetDatabase_LoadAssetAtPath<UnityEngine.Object>(path));
    }

    [Test]
    public void AssetDatabase_LoadAssetAtPath_Object()
    {
        RuntimeURDF.SetRuntimeMode(false);
        string path = "Packages/com.unity.robotics.urdf-importer/Tests/Runtime/RuntimeImport/RuntimeURDFTests.cs";
        Assert.IsNotNull(RuntimeURDF.AssetDatabase_LoadAssetAtPath(path, typeof(UnityEngine.Object)));
    }

    [Test]
    public void AssetDatabase_IsValidFolder_EditorMode()
    {
        RuntimeURDF.runtimeModeEnabled = false;
        Assert.IsTrue(RuntimeURDF.AssetDatabase_IsValidFolder("Packages/com.unity.robotics.urdf-importer/Tests/Runtime/RuntimeImport"));
        Assert.IsFalse(RuntimeURDF.AssetDatabase_IsValidFolder("Packages/com.unity.robotics.urdf-importer/Tests/Runtime/RuntimeImport/Does/Not/Exist"));
    }

    [Test]
    public void AssetDatabase_CreateFolder_AssetAsRoot()
    {
        RuntimeURDF.runtimeModeEnabled = false;
        string createdGUID = RuntimeURDF.AssetDatabase_CreateFolder("Assets/Tests/Runtime", "RuntimeURDF");
        string compareGUID = AssetDatabase.GUIDFromAssetPath(createFolderPath).ToString();
        
        // Verify valid folder
        Assert.IsTrue(AssetDatabase.IsValidFolder(createFolderPath));
        // Verify matching GUID
        Assert.AreEqual(createdGUID, compareGUID);

        // Try creating the same folder again
        createdGUID = RuntimeURDF.AssetDatabase_CreateFolder("Assets/Tests/Runtime", "RuntimeURDF");
        Assert.AreEqual(createdGUID, compareGUID);
    }

    [Test]
    public void AssetDatabase_MoveAsset_AssetAsRoot()
    {
        RuntimeURDF.runtimeModeEnabled = false;
        Assert.IsTrue(RuntimeURDF.AssetDatabase_MoveAsset($"{createAssetPath}/TestAsset.prefab", "Assets/Tests/TestAsset.prefab").Length == 0);

        // Move back for other tests
        AssetDatabase.MoveAsset("Assets/Tests/TestAsset.prefab", $"{createAssetPath}/TestAsset.prefab");
    }

    [Test]
    public void AssetDatabase_FindAssets_AssetAsRoot()
    {
        RuntimeURDF.runtimeModeEnabled = false;
        Assert.IsTrue(RuntimeURDF.AssetDatabase_FindAssets("TestAsset", new string[] {"Assets/Tests"}).Length > 0);
    }

    [Test]
    public void AssetDatabase_GUIDToAssetPath_AssetAsRoot()
    {
        RuntimeURDF.runtimeModeEnabled = false;
        string createdGUID = RuntimeURDF.AssetDatabase_CreateFolder(createAssetPath, "RuntimeURDF");
        Assert.AreEqual(createFolderPath, RuntimeURDF.AssetDatabase_GUIDToAssetPath(AssetDatabase.AssetPathToGUID(createFolderPath)));
    }

    [Test]
    public void AssetDatabase_GetAssetPath_AssetAsRoot()
    {
        RuntimeURDF.runtimeModeEnabled = false;
        Assert.AreEqual($"{createAssetPath}/TestAsset.prefab", RuntimeURDF.AssetDatabase_GetAssetPath(testObject));
    }

    [Test]
    public void PrefabUtility_GetCorrespondingObjectFromSource_AssetAsRoot()
    {
        RuntimeURDF.runtimeModeEnabled = false;
        var instantiated = PrefabUtility.InstantiatePrefab(testObject);
        Assert.AreEqual(testObject, RuntimeURDF.PrefabUtility_GetCorrespondingObjectFromSource(instantiated));
    }

    [Test]
    public void PrefabUtility_SaveAsPrefabAsset_AssetAsRoot()
    {
        RuntimeURDF.runtimeModeEnabled = false;
        Assert.IsNotNull(RuntimeURDF.PrefabUtility_SaveAsPrefabAsset(new GameObject("TestAsset2"), $"{createAssetPath}/TestAsset2.prefab"));
    }

    [Test]
    public void AssetDatabase_GetBuiltinExtraResource_Sprite()
    {
        RuntimeURDF.runtimeModeEnabled = false;
        Assert.IsNotNull(RuntimeURDF.AssetDatabase_GetBuiltinExtraResource<Sprite>("UI/Skin/Background.psd"));
    }

    [Test]
    public void AssetDatabase_CreateAsset_GameObject()
    {
        RuntimeURDF.runtimeModeEnabled = false;
        RuntimeURDF.AssetDatabase_CreateAsset(new TextAsset("TextAsset content"), $"{createAssetPath}/TestAsset3", true);
        Assert.IsNotNull(AssetDatabase.FindAssets("TestAsset3", new string[] {createAssetPath}));
    }

    [Test]
    public void PrefabUtility_InstantiatePrefab_GameObject()
    {
        RuntimeURDF.runtimeModeEnabled = false;
        Assert.IsNotNull(RuntimeURDF.PrefabUtility_InstantiatePrefab(testObject));
    }
    
    [TearDown]
    public void TearDown()
    {
        List<string> outFailedPaths = new List<string>();
        AssetDatabase.DeleteAssets(new string[] {"Assets/Tests"}, outFailedPaths);
    }
}
