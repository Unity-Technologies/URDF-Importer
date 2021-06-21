using System.IO;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEditor;
using UnityEngine.TestTools;
using Unity.Robotics.UrdfImporter;
using Object = UnityEngine.Object;

namespace Unity.Robotics.UrdfImporter.Tests
{
    public class UrdfAssetPathHandlerTests
    {
        string assetRoot;
        string packageRoot;

        [SetUp]
        public void SetUp()
        {
            assetRoot = "Assets/Tests/Runtime/UrdfAssetPathHandler";
            packageRoot = "Packages/com.unity.robotics.urdf-importer/Tests/Runtime/UrdfAssetPathHandler";

            RuntimeURDF.runtimeModeEnabled = false;
            Directory.CreateDirectory(assetRoot);
            Directory.CreateDirectory(packageRoot);
        }
        
        [Test]
        public void GetSetPackageRoot_RuntimeModeEnabled_Success()
        {
            // Force runtime mode to set testing package root
            RuntimeURDF.runtimeModeEnabled = true;
            UrdfAssetPathHandler.SetPackageRoot(assetRoot); // Set oldPackagePath
            UrdfAssetPathHandler.SetPackageRoot(assetRoot, true); // Run correctPackageRoot
            Assert.AreEqual(assetRoot, UrdfAssetPathHandler.GetPackageRoot());
        }

        [Test]
        public void GetRelativeAssetPath_Nonruntime_Success()
        {
            RuntimeURDF.runtimeModeEnabled = false;
            Assert.AreEqual("Invalid/Asset/Path", UrdfAssetPathHandler.GetRelativeAssetPath("Invalid/Asset/Path"));
            Assert.AreEqual($"{assetRoot}/TestAsset.txt", UrdfAssetPathHandler.GetRelativeAssetPath($"{assetRoot}/TestAsset.txt"));
#if UNITY_EDITOR
            Assert.AreEqual($"Packages/com.unity.robotics.urdf-importer/Tests/Runtime/AssetHandlers/UrdfAssetPathHandlerTests.cs", UrdfAssetPathHandler.GetRelativeAssetPath($"{Application.dataPath.Substring(0, Application.dataPath.Length - "Assets".Length)}Packages/com.unity.robotics.urdf-importer/Tests/Runtime/AssetHandlers/UrdfAssetPathHandlerTests.cs"));
#endif
        }

        [Test]
        public void GetRelativeAssetPath_Runtime_Success()
        {
            RuntimeURDF.runtimeModeEnabled = true;
            // Starting with Application.dataPath
            Assert.AreEqual("Assets/Valid/Path", UrdfAssetPathHandler.GetRelativeAssetPath($"{Application.dataPath}/Valid/Path"));
            // Not starting with dataPath
            Assert.AreEqual("Assets/Valid/Path", UrdfAssetPathHandler.GetRelativeAssetPath($"Assets/Valid/Path"));
        }

        [Test]
        public void GetFullAssetPath_AssetAndPackageRoot_Success()
        {
            Assert.AreEqual($"{Application.dataPath}/Tests/Runtime/UrdfAssetPathHandler", UrdfAssetPathHandler.GetFullAssetPath(assetRoot));
            string projectPath = Application.dataPath.Substring(0, Application.dataPath.Length - "Assets".Length);
            Assert.AreEqual($"{projectPath}Packages/com.unity.robotics.urdf-importer/Tests/Runtime/UrdfAssetPathHandler", UrdfAssetPathHandler.GetFullAssetPath(packageRoot));
        }

        [Test]
        public void GetRelativeAssetPathFromUrdfPath_CubeUrdf_Success()
        {
            string urdfRoot = "Packages/com.unity.robotics.urdf-importer/Tests/Runtime/Assets/URDF/cube";
            // Force runtime mode to set testing package root
            RuntimeURDF.runtimeModeEnabled = true;
            UrdfAssetPathHandler.SetPackageRoot(urdfRoot);
            RuntimeURDF.runtimeModeEnabled = false;
            
            Assert.AreEqual($"{urdfRoot}/meshes/cube.prefab", UrdfAssetPathHandler.GetRelativeAssetPathFromUrdfPath("package://meshes/cube.stl"));
            Assert.AreEqual($"{urdfRoot}/meshes/cube.prefab", UrdfAssetPathHandler.GetRelativeAssetPathFromUrdfPath("../meshes/cube.stl"));
        }

        [Test]
        public void IsValidAssetPath_Nonruntime_Success()
        {
            RuntimeURDF.runtimeModeEnabled = false;
            AssetDatabase.CreateAsset(new TextAsset("TestAsset"), $"{assetRoot}/TestAsset.txt");

            Assert.IsFalse(UrdfAssetPathHandler.IsValidAssetPath("Invalid/Asset/Path"));
            Assert.IsTrue(UrdfAssetPathHandler.IsValidAssetPath($"{assetRoot}/TestAsset.txt"));
            Assert.IsTrue(UrdfAssetPathHandler.IsValidAssetPath($"Packages/com.unity.robotics.urdf-importer/Tests/Runtime/AssetHandlers/UrdfAssetPathHandlerTests.cs"));

            RuntimeURDF.runtimeModeEnabled = true;
            // Everything returns true during runtime
            Assert.IsTrue(UrdfAssetPathHandler.IsValidAssetPath("Invalid/Asset/Path"));
            Assert.IsTrue(UrdfAssetPathHandler.IsValidAssetPath($"{assetRoot}/TestAsset.txt"));
            Assert.IsTrue(UrdfAssetPathHandler.IsValidAssetPath($"Packages/com.unity.robotics.urdf-importer/Tests/Runtime/AssetHandlers/UrdfAssetPathHandlerTests.cs"));
        }

        [Test]
        public void GetMaterialAssetPath_AssetAndPackage_Success()
        {
            // Force runtime mode to set testing package root
            RuntimeURDF.runtimeModeEnabled = true;
            UrdfAssetPathHandler.SetPackageRoot(assetRoot);
            RuntimeURDF.runtimeModeEnabled = false;
            Assert.AreEqual($"{assetRoot}/Materials/TestMaterial.mat", UrdfAssetPathHandler.GetMaterialAssetPath("TestMaterial"));


            // Force runtime mode to set testing package root
            RuntimeURDF.runtimeModeEnabled = true;
            UrdfAssetPathHandler.SetPackageRoot(packageRoot);
            RuntimeURDF.runtimeModeEnabled = false;
            Assert.AreEqual($"{packageRoot}/Materials/TestMaterial.mat", UrdfAssetPathHandler.GetMaterialAssetPath("TestMaterial"));
        }

        [TearDown]
        public void TearDown()
        {
            List<string> outFailedPaths = new List<string>();
            AssetDatabase.DeleteAssets(new string[] {"Assets/Tests", "Packages/com.unity.robotics.urdf-importer/Tests/Runtime/UrdfAssetPathHandler"}, outFailedPaths);
        }
    }
}
