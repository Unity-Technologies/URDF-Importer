using System.IO;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace Unity.Robotics.UrdfImporter.Tests
{
    public class StlAssetPostProcessorTests
    {
        const string k_AssetRoot = "Assets/Tests/Runtime/StlAssetPostProcessorTests";
        const string k_StlCubeSourcePath = "Packages/com.unity.robotics.urdf-importer/Tests/Runtime/Assets/URDF/cube/meshes/cube.stl";
        string m_StlCubeCopyPath;

        [SetUp]
        public void SetUp()
        {
            m_StlCubeCopyPath = k_AssetRoot + "/cube.stl";
            RuntimeUrdf.SetRuntimeMode(false);
            Directory.CreateDirectory(k_AssetRoot);
        }
        
        [Test]
        public void StlPostprocess_NewStl_DontCreatePrefab()
        {
            // make a new copy of the stl file
            Assert.IsTrue(AssetDatabase.CopyAsset(k_StlCubeSourcePath, m_StlCubeCopyPath));
            Assert.IsTrue(RuntimeUrdf.AssetExists(m_StlCubeCopyPath));

            // make sure the .asset file is not automatically created
            var meshAssetPath =  StlAssetPostProcessor.GetMeshAssetPath(m_StlCubeCopyPath, 0);            
            Assert.IsFalse(RuntimeUrdf.AssetExists(meshAssetPath));

            // make sure the .prefab file is not automatically created
            var prefabPath = StlAssetPostProcessor.GetPrefabAssetPath(m_StlCubeCopyPath);
            Assert.IsFalse(RuntimeUrdf.AssetExists(prefabPath));
            
            // make sure the .asset and .prefab file are created when requested
            StlAssetPostProcessor.PostprocessStlFile(m_StlCubeCopyPath);
            Assert.IsTrue(RuntimeUrdf.AssetExists(meshAssetPath));
            Assert.IsTrue(RuntimeUrdf.AssetExists(prefabPath));
        }
        
        [TearDown]
        public void TearDown()
        {
            List<string> outFailedPaths = new List<string>();
            AssetDatabase.DeleteAssets(new string[] {"Assets/Tests"}, outFailedPaths);
        }
    }
}