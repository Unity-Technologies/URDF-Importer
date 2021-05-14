using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEditor;
using UnityEngine.TestTools;
using RosSharp.Urdf;
using Object = UnityEngine.Object;

namespace RosSharp.Urdf.Tests
{
    public class UrdfGeometryCollisionTests
    {
        const float centerDelta = 1e-5f;
        const float scaleDelta = 1e-2f;

        [SetUp]
        public void SetUp()
        {
            RuntimeURDF.AssetDatabase_CreateFolder("Assets", "Tests");
            RuntimeURDF.AssetDatabase_CreateFolder("Assets/Tests", "Runtime");
            RuntimeURDF.AssetDatabase_CreateFolder("Assets/Tests/Runtime", "GeometryTests");
        }

        [Test]
        public void Create_Box_UnityPrimitive()
        {
            var parent = new GameObject("Parent").transform;
            UrdfGeometryCollision.Create(parent, GeometryTypes.Box);
            
            // Verify geometry created in Hierarchy
            var box = parent.Find("Box").gameObject;
            Assert.IsNotNull(box.activeInHierarchy);
            Assert.IsNotNull(box.GetComponent<BoxCollider>());
            Assert.AreEqual(new Bounds(Vector3.zero, Vector3.one), box.GetComponent<BoxCollider>().bounds);

            Object.DestroyImmediate(parent.gameObject);
        }
        
        [Test]
        public void Create_CylinderMesh_AssetCreated()
        {
            // Force runtime mode to set testing package root
            RuntimeURDF.runtimeModeEnabled = true;
            UrdfAssetPathHandler.SetPackageRoot("Assets/Tests/Runtime/GeometryTests");
            RuntimeURDF.runtimeModeEnabled = false;
            var parent = new GameObject("Parent").transform;
            UrdfGeometryCollision.Create(parent, GeometryTypes.Cylinder);
            
            // Verify Cylinder created in Hierarchy
            var createdCylinder = parent.Find("Cylinder").gameObject;
            Assert.IsNotNull(createdCylinder.activeInHierarchy);
            Assert.IsNotNull(createdCylinder.GetComponent<MeshCollider>());

            // Check for standard values on collider
            Assert.AreEqual(1864, createdCylinder.GetComponent<MeshCollider>().sharedMesh.vertexCount); 
            Assert.IsTrue(Vector3.Distance(Vector3.zero, createdCylinder.GetComponent<MeshCollider>().sharedMesh.bounds.center) < centerDelta); 
            Assert.IsTrue(Vector3.Distance(new Vector3(0.5f, 1f, 0.5f), createdCylinder.GetComponent<MeshCollider>().sharedMesh.bounds.extents) < scaleDelta); 
            // Verify Cylinder created in Assets
            Assert.IsNotNull(RuntimeURDF.AssetDatabase_FindAssets("Cylinder t:mesh", new string[] {"Assets/Tests/Runtime/GeometryTests"}));

            Object.DestroyImmediate(parent.gameObject);
        }

        [Test]
        public void Create_Sphere_UnityPrimitive()
        {
            var parent = new GameObject("Parent").transform;
            UrdfGeometryCollision.Create(parent, GeometryTypes.Sphere);
            
            // Verify geometry created in Hierarchy
            var sphere = parent.Find("Sphere").gameObject;
            Assert.IsNotNull(sphere.activeInHierarchy);
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
            Assert.IsNotNull(mesh.activeInHierarchy);
            Assert.IsNotNull(mesh.GetComponent<MeshCollider>());
            Assert.AreEqual(new Bounds(Vector3.zero, Vector3.zero), mesh.GetComponent<MeshCollider>().bounds);

            Object.DestroyImmediate(parent.gameObject);
        }

        [TearDown]
        public void TearDown()
        {
            List<string> outFailedPaths = new List<string>();
            AssetDatabase.DeleteAssets(new string[] {"Assets/Tests"}, outFailedPaths);
        }
    }
}
