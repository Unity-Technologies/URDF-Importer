using System;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Unity.Robotics.UrdfImporter.Tests
{
    public class UrdfMeshExportHandlerTests
    {
        static string k_ParentObjectName = "UrdfMeshExportHandlerParent";
        static string k_AssetPath = "Packages/com.unity.robotics.urdf-importer/Tests/Runtime/Assets/URDF";
        Transform m_InstantiatedParent;
        
        static IEnumerable<TestCaseData> MeshExtensionData
        {
            get
            {
                yield return new TestCaseData(
                    new Link.Geometry(mesh: new Link.Geometry.Mesh("package://meshes/cube.fbx",
                        new double[] {1, 1, 1})), "fbx");
                yield return new TestCaseData(
                    new Link.Geometry(mesh: new Link.Geometry.Mesh("package://meshes/cube.obj",
                        new double[] {1, 1, 1})), "obj");
                // Requires Blender to be installed on Bokken image
                // yield return new TestCaseData(
                //     new Link.Geometry(mesh: new Link.Geometry.Mesh("package://meshes/cube.blend",
                //         new double[] {1, 1, 1})), "blend");
                yield return new TestCaseData(
                    new Link.Geometry(mesh: new Link.Geometry.Mesh("package://meshes/cube.stl",
                        new double[] {1, 1, 1})), "stl");
            }
        }

        [Test, TestCaseSource(nameof(MeshExtensionData))]
        public void CopyOrCreateMesh_MeshExtensions_Success(Link.Geometry geom, string ext)
        {
            var folder = ext == "stl" ? "cube" : $"cube_{ext}";
            // Force runtime mode to set testing package root
            RuntimeUrdf.runtimeModeEnabled = true;
            UrdfAssetPathHandler.SetPackageRoot($"{k_AssetPath}/{folder}/");
            UrdfExportPathHandler.SetExportPath($"{k_AssetPath}/{folder}/export_test/");
            
            RuntimeUrdf.runtimeModeEnabled = false;
            UrdfRobotExtensions.importsettings = ImportSettings.DefaultSettings();
            UrdfRobotExtensions.importsettings.convexMethod = ImportSettings.convexDecomposer.unity;
            
            if (m_InstantiatedParent == null)
            {
                m_InstantiatedParent = new GameObject(k_ParentObjectName).transform;
            }
            var coll = UrdfCollisionExtensions.Create(m_InstantiatedParent, new Link.Collision(geom));
            var t = coll.transform.GetChild(0);
            var path = UrdfMeshExportHandler.CopyOrCreateMesh(t.gameObject, true);
            Assert.IsNotEmpty(path);
            Assert.AreEqual(path, $"{k_AssetPath}/{folder}/export_test/meshes/cube.{ext}");
            
            Object.DestroyImmediate(m_InstantiatedParent.gameObject);
            var outFailedPaths = new List<string>();
            AssetDatabase.DeleteAssets(new string[] {$"{k_AssetPath}/{folder}/export_test/"}, outFailedPaths);
        }
    }
}
