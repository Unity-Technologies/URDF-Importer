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
    [SetUp]
    public void SetUp()
    {
    }

    [Test]
    public void AssetDatabase_CreateFolder_AssetAsRoot()
    {
        string testPath = "Assets/Tests/Runtime/RuntimeURDF";
        RuntimeURDF.AssetDatabase_CreateFolder("Assets", "Tests");
        RuntimeURDF.AssetDatabase_CreateFolder("Assets/Tests", "Runtime");
        string createdGUID = RuntimeURDF.AssetDatabase_CreateFolder("Assets/Tests/Runtime", "RuntimeURDF");
        string compareGUID = AssetDatabase.GUIDFromAssetPath(testPath).ToString();
        
        // Verify valid folder
        Assert.IsTrue(AssetDatabase.IsValidFolder(testPath));
        // Verify matching GUID
        Assert.AreEqual(createdGUID, compareGUID);
    }
    
    [TearDown]
    public void TearDown()
    {
        List<string> outFailedPaths = new List<string>();
        AssetDatabase.DeleteAssets(new string[] {"Assets/Tests"}, outFailedPaths);
    }
}
