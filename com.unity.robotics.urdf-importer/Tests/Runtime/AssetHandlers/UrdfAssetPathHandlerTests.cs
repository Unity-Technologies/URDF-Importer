using System;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

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

            RuntimeUrdf.runtimeModeEnabled = false;
            Directory.CreateDirectory(assetRoot);
            Directory.CreateDirectory(packageRoot);
        }

        [Test]
        public void GetSetPackageRoot_RuntimeModeEnabled_Success()
        {
            // Force runtime mode to set testing package root
            RuntimeUrdf.runtimeModeEnabled = true;
            UrdfAssetPathHandler.SetPackageRoot(assetRoot); // Set oldPackagePath
            UrdfAssetPathHandler.SetPackageRoot(assetRoot, true); // Run correctPackageRoot
            Assert.AreEqual(assetRoot, UrdfAssetPathHandler.GetPackageRoot());
        }

        [Test]
        public void GetRelativeAssetPath_Nonruntime_Success()
        {
            RuntimeUrdf.runtimeModeEnabled = false;
            Assert.AreEqual("Invalid/Asset/Path", UrdfAssetPathHandler.GetRelativeAssetPath("Invalid/Asset/Path"));
            Assert.AreEqual($"{assetRoot}/TestAsset.txt",
                UrdfAssetPathHandler.GetRelativeAssetPath($"{assetRoot}/TestAsset.txt"));
#if UNITY_EDITOR
            Assert.AreEqual(
                "Packages/com.unity.robotics.urdf-importer/Tests/Runtime/AssetHandlers/UrdfAssetPathHandlerTests.cs",
                UrdfAssetPathHandler.GetRelativeAssetPath(
                    $"{Application.dataPath.Substring(0, Application.dataPath.Length - "Assets".Length)}" +
                    "Packages/com.unity.robotics.urdf-importer/Tests/Runtime/AssetHandlers/UrdfAssetPathHandlerTests.cs"));
#endif
        }

        [Test]
        public void GetRelativeAssetPath_Runtime_Success()
        {
            RuntimeUrdf.runtimeModeEnabled = true;

            // Starting with Application.dataPath
            Assert.AreEqual("Assets/Valid/Path",
                UrdfAssetPathHandler.GetRelativeAssetPath($"{Application.dataPath}/Valid/Path"));

            // Not starting with dataPath
            Assert.AreEqual("Assets/Valid/Path", UrdfAssetPathHandler.GetRelativeAssetPath("Assets/Valid/Path"));
        }

        [Test]
        public void GetFullAssetPath_AssetAndPackageRoot_Success()
        {
            Assert.AreEqual($"{Application.dataPath}/Tests/Runtime/UrdfAssetPathHandler",
                UrdfAssetPathHandler.GetFullAssetPath(assetRoot));
            var projectPath = Application.dataPath.Substring(0, Application.dataPath.Length - "Assets".Length);
            Assert.AreEqual(
                $"{projectPath}Packages/com.unity.robotics.urdf-importer/Tests/Runtime/UrdfAssetPathHandler",
                UrdfAssetPathHandler.GetFullAssetPath(packageRoot));
        }

        [Test]
        public void GetRelativeAssetPathFromUrdfPath_CubeUrdf_Success()
        {
            var urdfRoot = "Packages/com.unity.robotics.urdf-importer/Tests/Runtime/Assets/URDF/cube";

            // Force runtime mode to set testing package root
            RuntimeUrdf.runtimeModeEnabled = true;
            UrdfAssetPathHandler.SetPackageRoot(urdfRoot);
            RuntimeUrdf.runtimeModeEnabled = false;

            Assert.AreEqual($"{urdfRoot}/meshes/cube.prefab",
                UrdfAssetPathHandler.GetRelativeAssetPathFromUrdfPath("package://meshes/cube.stl"));
            Assert.AreEqual($"{urdfRoot}/meshes/cube.prefab",
                UrdfAssetPathHandler.GetRelativeAssetPathFromUrdfPath("../meshes/cube.stl"));
        }

        [Test]
        public void IsValidAssetPath_Nonruntime_Success()
        {
            RuntimeUrdf.runtimeModeEnabled = false;
            AssetDatabase.CreateAsset(new TextAsset("TestAsset"), $"{assetRoot}/TestAsset.txt");

            Assert.IsFalse(UrdfAssetPathHandler.IsValidAssetPath("Invalid/Asset/Path"));
            Assert.IsTrue(UrdfAssetPathHandler.IsValidAssetPath($"{assetRoot}/TestAsset.txt"));
            Assert.IsTrue(UrdfAssetPathHandler.IsValidAssetPath(
                "Packages/com.unity.robotics.urdf-importer/Tests/Runtime/AssetHandlers/UrdfAssetPathHandlerTests.cs"));

            RuntimeUrdf.runtimeModeEnabled = true;

            // Everything returns true during runtime
            Assert.IsTrue(UrdfAssetPathHandler.IsValidAssetPath("Invalid/Asset/Path"));
            Assert.IsTrue(UrdfAssetPathHandler.IsValidAssetPath($"{assetRoot}/TestAsset.txt"));
            Assert.IsTrue(UrdfAssetPathHandler.IsValidAssetPath(
                "Packages/com.unity.robotics.urdf-importer/Tests/Runtime/AssetHandlers/UrdfAssetPathHandlerTests.cs"));
        }

        [Test]
        public void GetMaterialAssetPath_AssetAndPackage_Success()
        {
            // Force runtime mode to set testing package root
            RuntimeUrdf.runtimeModeEnabled = true;
            UrdfAssetPathHandler.SetPackageRoot(assetRoot);
            RuntimeUrdf.runtimeModeEnabled = false;
            Assert.AreEqual($"{assetRoot}/Materials/TestMaterial.mat",
                UrdfAssetPathHandler.GetMaterialAssetPath("TestMaterial"));

            // Force runtime mode to set testing package root
            RuntimeUrdf.runtimeModeEnabled = true;
            UrdfAssetPathHandler.SetPackageRoot(packageRoot);
            RuntimeUrdf.runtimeModeEnabled = false;
            Assert.AreEqual($"{packageRoot}/Materials/TestMaterial.mat",
                UrdfAssetPathHandler.GetMaterialAssetPath("TestMaterial"));
        }

        [TearDown]
        public void TearDown()
        {
            var outFailedPaths = new List<string>();
            AssetDatabase.DeleteAssets(
                new[]
                {
                    "Assets/Tests", "Packages/com.unity.robotics.urdf-importer/Tests/Runtime/UrdfAssetPathHandler"
                }, outFailedPaths);
        }
    }
}
