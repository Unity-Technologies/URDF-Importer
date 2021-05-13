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
    public class UrdfCollisionExtensionsTest
    {
        [Test]
        public void Create_Geometry_Box()
        {
            RuntimeURDF.runtimeModeEnabled = false;
            var parent = new GameObject("Parent").transform;
            UrdfCollisionExtensions.Create(parent, GeometryTypes.Box);
            Assert.IsNotNull(parent.Find("unnamed").gameObject.activeInHierarchy);
            Assert.IsNotNull(parent.Find("unnamed").GetComponent<UrdfCollision>());
        }

        [Test]
        public void Create_Geometry_Box_WithVisual()
        {
            RuntimeURDF.runtimeModeEnabled = false;
            var parent = new GameObject("Parent").transform;
            var copy = new GameObject("Copy").transform;
            copy.transform.localScale = Vector3.one * 2f;
            UrdfCollisionExtensions.Create(parent, GeometryTypes.Box, copy);
            Assert.IsNotNull(parent.Find("unnamed").gameObject.activeInHierarchy);
            Assert.IsNotNull(parent.Find("unnamed").GetComponent<UrdfCollision>());
            Assert.AreEqual(Vector3.one * 2f, parent.Find("unnamed").localScale);
        }

        [Test]
        public void Create_Geometry_Mesh()
        {
            RuntimeURDF.runtimeModeEnabled = false;
            var parent = new GameObject("Parent").transform;
            UrdfCollisionExtensions.Create(parent, GeometryTypes.Mesh);
            Assert.IsNotNull(parent.Find("unnamed").gameObject.activeInHierarchy);
            Assert.IsNotNull(parent.Find("unnamed").GetComponent<UrdfCollision>());
        }

        [Test]
        public void Create_Geometry_Mesh_WithVisual()
        {
            RuntimeURDF.runtimeModeEnabled = false;
            var parent = new GameObject("Parent").transform;
            var copy = new GameObject("Copy").transform;
            copy.transform.localScale = Vector3.one * 2f;
            UrdfCollisionExtensions.Create(parent, GeometryTypes.Mesh, copy);
            Assert.IsNotNull(parent.Find("unnamed").gameObject.activeInHierarchy);
            Assert.IsNotNull(parent.Find("unnamed").GetComponent<UrdfCollision>());
            Assert.AreEqual(Vector3.one * 2f, parent.Find("unnamed").localScale);
        }

        [Test]
        public void Create_LinkCollision_Box()
        {
            RuntimeURDF.runtimeModeEnabled = false;
            var parent = new GameObject("Parent").transform;
            UrdfCollisionExtensions.Create(parent, new Link.Collision(new Link.Geometry(box:new Link.Geometry.Box(new double[] {1, 1, 1}))));
        }

        [Test]
        public void ExportCollisionData_Box()
        {
            RuntimeURDF.runtimeModeEnabled = false;
            var parent = new GameObject("Parent").transform;
            UrdfCollisionExtensions.Create(parent, GeometryTypes.Box);
            var collisionComponent = parent.Find("unnamed").GetComponent<UrdfCollision>();
            var exported = UrdfCollisionExtensions.ExportCollisionData(collisionComponent);
            Assert.IsNotNull(exported);
            Assert.IsNull(exported.name);
        }
    }
}