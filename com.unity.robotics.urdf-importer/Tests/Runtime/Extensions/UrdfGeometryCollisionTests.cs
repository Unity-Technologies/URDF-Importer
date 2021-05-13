using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEditor;
using UnityEngine.TestTools;
using RosSharp.Urdf;

namespace RosSharp.Urdf.Tests
{
    public class UrdfGeometryCollisionTests
    {
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
            Assert.IsNotNull(GameObject.Find("Box").activeInHierarchy);
            Assert.IsNotNull(GameObject.Find("Box").GetComponent<BoxCollider>());
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
            Assert.IsNotNull(GameObject.Find("Cylinder").activeInHierarchy);
            Assert.IsNotNull(GameObject.Find("Cylinder").GetComponent<MeshCollider>());
            // Verify Cylinder created in Assets
            Assert.IsNotNull(RuntimeURDF.AssetDatabase_FindAssets("Cylinder t:mesh", new string[] {"Assets/Tests/Runtime/GeometryTests"}));
        }

        [Test]
        public void Create_Sphere_UnityPrimitive()
        {
            var parent = new GameObject("Parent").transform;
            UrdfGeometryCollision.Create(parent, GeometryTypes.Sphere);
            
            // Verify geometry created in Hierarchy
            Assert.IsNotNull(GameObject.Find("Sphere").activeInHierarchy);
            Assert.IsNotNull(GameObject.Find("Sphere").GetComponent<SphereCollider>());
        }

        [Test]
        public void Create_Mesh_UnityPrimitive()
        {
            var parent = new GameObject("Parent").transform;
            UrdfGeometryCollision.Create(parent, GeometryTypes.Mesh);
            
            // Verify geometry created in Hierarchy
            Assert.IsNotNull(GameObject.Find("Mesh").activeInHierarchy);
            Assert.IsNotNull(GameObject.Find("Mesh").GetComponent<MeshCollider>());
        }

        [TearDown]
        public void TearDown()
        {
            List<string> outFailedPaths = new List<string>();
            AssetDatabase.DeleteAssets(new string[] {"Assets/Tests"}, outFailedPaths);
        }
    }

}