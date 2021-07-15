using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEditor;
using UnityEngine.TestTools;
using Unity.Robotics.UrdfImporter;
using Object = UnityEngine.Object;
using Collision = Unity.Robotics.UrdfImporter.Link.Collision;
using Geometry = Unity.Robotics.UrdfImporter.Link.Geometry;
using Box = Unity.Robotics.UrdfImporter.Link.Geometry.Box;
using Mesh = Unity.Robotics.UrdfImporter.Link.Geometry.Mesh;

namespace Unity.Robotics.UrdfImporter.Tests
{
    public class UrdfGeometryCollisionTests
    {
        const float centerDelta = 1e-5f;
        const float scaleDelta = 1e-2f;

        [SetUp]
        public void SetUp()
        {
            // a robot tag needs to be added in project settings before runtime import can work
            RuntimeUrdf.SetRuntimeMode(false);
            UrdfRobotExtensions.CreateTag(); 
            RuntimeUrdf.AssetDatabase_CreateFolder("Assets", "Tests");
            RuntimeUrdf.AssetDatabase_CreateFolder("Assets/Tests", "Runtime");
            RuntimeUrdf.AssetDatabase_CreateFolder("Assets/Tests/Runtime", "GeometryTests");
        }

        [Test]
        public void Create_Box_UnityPrimitive()
        {
            var parent = new GameObject("Parent").transform;
            UrdfGeometryCollision.Create(parent, GeometryTypes.Box);
            
            // Verify geometry created in Hierarchy
            var box = parent.Find("Box").gameObject;
            Assert.IsTrue(box.activeInHierarchy);
            Assert.IsNotNull(box.GetComponent<BoxCollider>());
            Assert.AreEqual(new Bounds(Vector3.zero, Vector3.one), box.GetComponent<BoxCollider>().bounds);

            Object.DestroyImmediate(parent.gameObject);
        }
        
        [Test]
        public void Create_CylinderMesh_AssetCreated()
        {
            // Force runtime mode to set testing package root
            RuntimeUrdf.runtimeModeEnabled = true;
            UrdfAssetPathHandler.SetPackageRoot("Assets/Tests/Runtime/GeometryTests");
            RuntimeUrdf.runtimeModeEnabled = false;
            var parent = new GameObject("Parent").transform;
            UrdfGeometryCollision.Create(parent, GeometryTypes.Cylinder);
            
            // Verify Cylinder created in Hierarchy
            var createdCylinder = parent.Find("Cylinder").gameObject;
            Assert.IsTrue(createdCylinder.activeInHierarchy);
            Assert.IsNotNull(createdCylinder.GetComponent<MeshCollider>());

            // Check for standard values on collider
            Assert.AreEqual(1864, createdCylinder.GetComponent<MeshCollider>().sharedMesh.vertexCount); 
            Assert.IsTrue(Vector3.Distance(Vector3.zero, createdCylinder.GetComponent<MeshCollider>().sharedMesh.bounds.center) < centerDelta); 
            Assert.IsTrue(Vector3.Distance(new Vector3(0.5f, 1f, 0.5f), createdCylinder.GetComponent<MeshCollider>().sharedMesh.bounds.extents) < scaleDelta); 
            // Verify Cylinder created in Assets
            Assert.IsNotNull(RuntimeUrdf.AssetDatabase_FindAssets("Cylinder t:mesh", new string[] {"Assets/Tests/Runtime/GeometryTests"}));
            
            AssetDatabase.DeleteAsset("Assets/Tests/Runtime/GeometryTests/Cylinder.asset");
            Object.DestroyImmediate(parent.gameObject);
        }

        [Test]
        public void Create_Sphere_UnityPrimitive()
        {
            var parent = new GameObject("Parent").transform;
            UrdfGeometryCollision.Create(parent, GeometryTypes.Sphere);
            
            // Verify geometry created in Hierarchy
            var sphere = parent.Find("Sphere").gameObject;
            Assert.IsTrue(sphere.activeInHierarchy);
            Assert.IsNotNull(sphere.GetComponent<SphereCollider>());
            Assert.AreEqual(new Bounds(Vector3.zero, Vector3.one), sphere.GetComponent<SphereCollider>().bounds);

            Object.DestroyImmediate(parent.gameObject);
        }

        [Test]
        public void Create_Mesh_UnityPrimitive()
        {
            var parent = new GameObject("Parent").transform;
            UrdfGeometryCollision.Create(parent, GeometryTypes.Mesh);
            
            // Verify geometry created in Hierarchy
            var mesh = parent.Find("Mesh").gameObject;
            Assert.IsTrue(mesh.activeInHierarchy);
            Assert.IsNotNull(mesh.GetComponent<MeshCollider>());
            Assert.AreEqual(new Bounds(Vector3.zero, Vector3.zero), mesh.GetComponent<MeshCollider>().bounds);

            Object.DestroyImmediate(parent.gameObject);
        }

        [Test]
        public void Create_FromStlUnity_CubeMesh()
        {
            // Force runtime mode to set testing package root
            RuntimeUrdf.runtimeModeEnabled = true;
            UrdfAssetPathHandler.SetPackageRoot("Packages/com.unity.robotics.urdf-importer/Tests/Runtime/Assets/URDF/cube/");
            RuntimeUrdf.runtimeModeEnabled = false;
            UrdfRobotExtensions.importsettings = ImportSettings.DefaultSettings();
            UrdfRobotExtensions.importsettings.convexMethod = ImportSettings.convexDecomposer.unity;
            
            var parent = new GameObject("Parent").transform;
            string path = "package://meshes/cube.stl";
            var meshGeometry = new Geometry(mesh: new Mesh(path, new double[] {1,1,1}));
            UrdfCollisionExtensions.Create(parent, new Collision(meshGeometry));

            // Verify geometry created in Hierarchy
            var urdfCollision = parent.GetComponentInChildren<UrdfCollision>().transform;
            var mesh = urdfCollision.Find("cube/cube_0").gameObject;
            Assert.IsTrue(mesh.activeInHierarchy);
            Assert.IsNotNull(mesh.GetComponent<MeshCollider>());
            Assert.AreEqual(36, mesh.GetComponent<MeshCollider>().sharedMesh.vertexCount); 
            Assert.IsTrue(Vector3.Distance(Vector3.one * 15f, mesh.GetComponent<MeshCollider>().sharedMesh.bounds.extents) < scaleDelta); 
            
            // Verify geometry created in Assets
            Assert.IsNotNull(AssetDatabase.FindAssets("cube t:mesh", new string[] {"Packages/com.unity.robotics.urdf-importer/Tests/Runtime/Assets/URDF/cube/meshes"}));

            AssetDatabase.DeleteAsset("Packages/com.unity.robotics.urdf-importer/Tests/Runtime/Assets/URDF/cube/meshes/cube_1.asset");
            Object.DestroyImmediate(parent.gameObject);
        }

        [Test]
        public void Create_FromStlVhacdNotRuntime_CubeMesh()
        {
            // Force runtime mode to set testing package root
            LogAssert.ignoreFailingMessages = true;
            RuntimeUrdf.runtimeModeEnabled = true;
            UrdfAssetPathHandler.SetPackageRoot("Packages/com.unity.robotics.urdf-importer/Tests/Runtime/Assets/URDF/cube/");
            RuntimeUrdf.runtimeModeEnabled = false;
            UrdfRobotExtensions.importsettings = ImportSettings.DefaultSettings();
            
            var parent = new GameObject("Parent").transform;
            string path = "package://meshes/cube.stl";
            var meshGeometry = new Geometry(mesh: new Mesh(path, new double[] {1,1,1}));
            UrdfCollisionExtensions.Create(parent, new Collision(meshGeometry));

            // Verify geometry created in Hierarchy
            var urdfCollision = parent.GetComponentInChildren<UrdfCollision>().transform;
            var mesh = urdfCollision.Find("cube/cube_0").gameObject;
            Assert.IsTrue(mesh.activeInHierarchy);
            Assert.IsNotNull(mesh.GetComponent<MeshCollider>());
            Assert.AreEqual(8, mesh.GetComponent<MeshCollider>().sharedMesh.vertexCount); 
            Assert.IsTrue(Vector3.Distance(Vector3.one * 15f, mesh.GetComponent<MeshCollider>().sharedMesh.bounds.extents) < scaleDelta); 
            
            // Verify geometry created in Assets
            Assert.IsNotNull(AssetDatabase.FindAssets("cube t:mesh", new string[] {"Packages/com.unity.robotics.urdf-importer/Tests/Runtime/Assets/URDF/cube/meshes"}));

            LogAssert.ignoreFailingMessages = false;
            AssetDatabase.DeleteAsset("Packages/com.unity.robotics.urdf-importer/Tests/Runtime/Assets/URDF/cube/meshes/cube_1.asset");
            Object.DestroyImmediate(parent.gameObject);
        }

        [Test]
        public void Create_FromStlVhacdRuntime_CubeMesh()
        {
            // Force runtime mode to set testing package root
            RuntimeUrdf.runtimeModeEnabled = true;
            UrdfAssetPathHandler.SetPackageRoot("Packages/com.unity.robotics.urdf-importer/Tests/Runtime/Assets/URDF/cube/");
            UrdfRobotExtensions.importsettings = ImportSettings.DefaultSettings();
            
            var parent = new GameObject("Parent").transform;
            string path = "package://meshes/cube.stl";
            var meshGeometry = new Geometry(mesh: new Mesh(path, new double[] {1,1,1}));
            UrdfCollisionExtensions.Create(parent, new Collision(meshGeometry));

            // Verify geometry created in Hierarchy
            var urdfCollision = parent.GetComponentInChildren<UrdfCollision>().transform;
            var mesh = urdfCollision.Find("cube/cube_0").gameObject;
            Assert.IsTrue(mesh.activeInHierarchy);
            Assert.IsNotNull(mesh.GetComponent<MeshCollider>());
            Assert.AreEqual(8, mesh.GetComponent<MeshCollider>().sharedMesh.vertexCount); 
            Assert.IsTrue(Vector3.Distance(Vector3.one * 15f, mesh.GetComponent<MeshCollider>().sharedMesh.bounds.extents) < scaleDelta); 
            
            // Verify geometry created in Assets
            Assert.IsNotNull(AssetDatabase.FindAssets("cube t:mesh", new string[] {"Packages/com.unity.robotics.urdf-importer/Tests/Runtime/Assets/URDF/cube/meshes"}));

            AssetDatabase.DeleteAsset("Packages/com.unity.robotics.urdf-importer/Tests/Runtime/Assets/URDF/cube/meshes/cube_1.asset");
            Object.DestroyImmediate(parent.gameObject);
        }

        [Test]
        public void CreateMatchingMeshCollision_NoVisual_Null()
        {
            var visualToCopy = new GameObject("VisualToCopy").transform;
            var parent = new GameObject("Parent").transform;
            UrdfGeometryCollision.CreateMatchingMeshCollision(parent, visualToCopy);
            
            Assert.AreEqual(0, parent.childCount);

            Object.DestroyImmediate(parent.gameObject); 
            Object.DestroyImmediate(visualToCopy.gameObject); 
        }

        [Test]
        public void CreateMatchingMeshCollision_CubeMesh_CopiedCube()
        {
            var urdfFile = "Packages/com.unity.robotics.urdf-importer/Tests/Runtime/Assets/URDF/cube/cube.urdf";
            UrdfRobotExtensions.importsettings = ImportSettings.DefaultSettings();
            UrdfRobotExtensions.importsettings.convexMethod = ImportSettings.convexDecomposer.unity;
            var robotObject = UrdfRobotExtensions.CreateRuntime(urdfFile, UrdfRobotExtensions.importsettings);
            Transform visualToCopy = robotObject.transform.Find("base_link/Visuals/unnamed/cube");

            var parent = new GameObject("Parent").transform;
            UrdfGeometryCollision.CreateMatchingMeshCollision(parent, visualToCopy);
            
            Assert.AreEqual(1, parent.childCount);
            var mesh = parent.GetChild(0).gameObject;
            Assert.IsTrue(mesh.activeInHierarchy);
            Assert.IsNotNull(mesh.GetComponent<MeshCollider>());
            Assert.AreEqual(36, mesh.GetComponent<MeshCollider>().sharedMesh.vertexCount); 
            Assert.IsTrue(Vector3.Distance(Vector3.one * 15f, mesh.GetComponent<MeshCollider>().sharedMesh.bounds.extents) < scaleDelta); 

            Object.DestroyImmediate(parent.gameObject); 
            Object.DestroyImmediate(robotObject.gameObject); 
        }

        [TearDown]
        public void TearDown()
        {
            List<string> outFailedPaths = new List<string>();
            AssetDatabase.DeleteAssets(new string[] {"Assets/Tests"}, outFailedPaths);
        }
    }
}
