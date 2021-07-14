using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEditor;
using UnityEngine.TestTools;
using Unity.Robotics.UrdfImporter;

namespace Unity.Robotics.UrdfImporter.Tests
{
    public class RuntimeUrdfTests
    {
        string createAssetPath = "Assets/Tests/Runtime";
        string createFolderPath = "Assets/Tests/Runtime/RuntimeUrdf";
        UnityEngine.Object testObject;

        [SetUp]
        public void SetUp()
        {
            RuntimeUrdf.runtimeModeEnabled = false;
            AssetDatabase.CreateFolder("Assets", "Tests");
            AssetDatabase.CreateFolder("Assets/Tests", "Runtime");

            testObject = PrefabUtility.SaveAsPrefabAsset(new GameObject("TestAsset"), $"{createAssetPath}/TestAsset.prefab");
        }

        [Test]
        public void IsRuntimeMode_True()
        {
            RuntimeUrdf.runtimeModeEnabled = true;
            Assert.IsTrue(RuntimeUrdf.IsRuntimeMode());
        }

        [Test]
        public void SetRuntimeMode_False()
        {
            RuntimeUrdf.SetRuntimeMode(false);
            Assert.IsFalse(RuntimeUrdf.IsRuntimeMode());
        }

        [Test]
        public void AssetDatabase_LoadAssetAtPath_Script()
        {
            RuntimeUrdf.SetRuntimeMode(false);
            string path = "Packages/com.unity.robotics.urdf-importer/Tests/Runtime/RuntimeImport/RuntimeUrdfTests.cs";
            Assert.IsNotNull(RuntimeUrdf.AssetDatabase_LoadAssetAtPath<UnityEngine.Object>(path));
        }

        [Test]
        public void AssetDatabase_LoadAssetAtPath_Object()
        {
            RuntimeUrdf.SetRuntimeMode(false);
            string path = "Packages/com.unity.robotics.urdf-importer/Tests/Runtime/RuntimeImport/RuntimeUrdfTests.cs";
            Assert.IsNotNull(RuntimeUrdf.AssetDatabase_LoadAssetAtPath(path, typeof(UnityEngine.Object)));
        }

        [Test]
        public void AssetDatabase_IsValidFolder_EditorMode()
        {
            RuntimeUrdf.runtimeModeEnabled = false;
            Assert.IsTrue(RuntimeUrdf.AssetDatabase_IsValidFolder("Packages/com.unity.robotics.urdf-importer/Tests/Runtime/RuntimeImport"));
            Assert.IsFalse(RuntimeUrdf.AssetDatabase_IsValidFolder("Packages/com.unity.robotics.urdf-importer/Tests/Runtime/RuntimeImport/Does/Not/Exist"));
        }

        [Test]
        public void AssetDatabase_CreateFolder_AssetAsRoot()
        {
            RuntimeUrdf.runtimeModeEnabled = false;
            string createdGUID = RuntimeUrdf.AssetDatabase_CreateFolder("Assets/Tests/Runtime", "RuntimeUrdf");
            string compareGUID = AssetDatabase.GUIDFromAssetPath(createFolderPath).ToString();

            // Verify valid folder
            Assert.IsTrue(AssetDatabase.IsValidFolder(createFolderPath));

            // Verify matching GUID
            Assert.AreEqual(createdGUID, compareGUID);

            // Try creating the same folder again
            createdGUID = RuntimeUrdf.AssetDatabase_CreateFolder("Assets/Tests/Runtime", "RuntimeUrdf");
            Assert.AreEqual(createdGUID, compareGUID);
        }

        [Test]
        public void AssetDatabase_MoveAsset_AssetAsRoot()
        {
            RuntimeUrdf.runtimeModeEnabled = false;
            Assert.IsTrue(RuntimeUrdf.AssetDatabase_MoveAsset($"{createAssetPath}/TestAsset.prefab", "Assets/Tests/TestAsset.prefab").Length == 0);

            // Move back for other tests
            AssetDatabase.MoveAsset("Assets/Tests/TestAsset.prefab", $"{createAssetPath}/TestAsset.prefab");
        }

        [Test]
        public void AssetDatabase_FindAssets_AssetAsRoot()
        {
            RuntimeUrdf.runtimeModeEnabled = false;
            Assert.IsTrue(RuntimeUrdf.AssetDatabase_FindAssets("TestAsset", new string[] { "Assets/Tests" }).Length > 0);
        }

        [Test]
        public void AssetDatabase_GUIDToAssetPath_AssetAsRoot()
        {
            RuntimeUrdf.runtimeModeEnabled = false;
            string createdGUID = RuntimeUrdf.AssetDatabase_CreateFolder(createAssetPath, "RuntimeUrdf");
            Assert.AreEqual(createFolderPath, RuntimeUrdf.AssetDatabase_GUIDToAssetPath(AssetDatabase.AssetPathToGUID(createFolderPath)));
        }

        [Test]
        public void AssetDatabase_GetAssetPath_AssetAsRoot()
        {
            RuntimeUrdf.runtimeModeEnabled = false;
            Assert.AreEqual($"{createAssetPath}/TestAsset.prefab", RuntimeUrdf.AssetDatabase_GetAssetPath(testObject));
        }

        [Test]
        public void PrefabUtility_GetCorrespondingObjectFromSource_AssetAsRoot()
        {
            RuntimeUrdf.runtimeModeEnabled = false;
            var instantiated = PrefabUtility.InstantiatePrefab(testObject);
            Assert.AreEqual(testObject, RuntimeUrdf.PrefabUtility_GetCorrespondingObjectFromSource(instantiated));
        }

        [Test]
        public void PrefabUtility_SaveAsPrefabAsset_AssetAsRoot()
        {
            RuntimeUrdf.runtimeModeEnabled = false;
            Assert.IsNotNull(RuntimeUrdf.PrefabUtility_SaveAsPrefabAsset(new GameObject("TestAsset2"), $"{createAssetPath}/TestAsset2.prefab"));
        }

        [Test]
        public void AssetDatabase_GetBuiltinExtraResource_Sprite()
        {
            RuntimeUrdf.runtimeModeEnabled = false;
            Assert.IsNotNull(RuntimeUrdf.AssetDatabase_GetBuiltinExtraResource<Sprite>("UI/Skin/Background.psd"));
        }

        [Test]
        public void AssetDatabase_CreateAsset_GameObject()
        {
            RuntimeUrdf.runtimeModeEnabled = false;
            RuntimeUrdf.AssetDatabase_CreateAsset(new TextAsset("TextAsset content"), $"{createAssetPath}/TestAsset3", true);
            Assert.IsNotNull(AssetDatabase.FindAssets("TestAsset3", new string[] { createAssetPath }));
        }

        [Test]
        public void PrefabUtility_InstantiatePrefab_GameObject()
        {
            RuntimeUrdf.runtimeModeEnabled = false;
            Assert.IsNotNull(RuntimeUrdf.PrefabUtility_InstantiatePrefab(testObject));
        }

        [Test]
        public void AssetExists_True()
        {
            RuntimeUrdf.SetRuntimeMode(false);
            Assert.IsTrue(RuntimeUrdf.AssetExists($"{createAssetPath}/TestAsset.prefab"));
            Assert.IsTrue(RuntimeUrdf.AssetExists($"{createAssetPath}/tEstAsset.Prefab", true));
        }
        
        [Test]
        public void AssetExists_False()
        {
            RuntimeUrdf.SetRuntimeMode(false);
            Assert.IsFalse(RuntimeUrdf.AssetExists($"{createAssetPath}/tEstAsset.Prefab")); // case
            Assert.IsFalse(RuntimeUrdf.AssetExists($"{createAssetPath}/TestAsset.prefabs", true));
            Assert.IsFalse(RuntimeUrdf.AssetExists($"{createAssetPath}/TestAsset.prefa", true));
            Assert.IsFalse(RuntimeUrdf.AssetExists($"{createAssetPath}/estAsset.prefab", true));
            Assert.IsFalse(RuntimeUrdf.AssetExists($"{createAssetPath}/sub/TestAsset.prefab", true));
        }
        
        [TearDown]
        public void TearDown()
        {
            List<string> outFailedPaths = new List<string>();
            AssetDatabase.DeleteAssets(new string[] { "Assets/Tests" }, outFailedPaths);
        }
    }
}
