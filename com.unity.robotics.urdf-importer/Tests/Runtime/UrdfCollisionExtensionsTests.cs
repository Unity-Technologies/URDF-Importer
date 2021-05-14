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
    public class UrdfCollisionExtensionsTests
    {
        [Test]
        public void Create_GeometryBox_DefaultGeometry()
        {
            RuntimeURDF.runtimeModeEnabled = false;
            var parent = new GameObject("Parent").transform;
            UrdfCollisionExtensions.Create(parent, GeometryTypes.Box);

            var collision = parent.Find("unnamed").GetComponent<UrdfCollision>();
            Assert.IsNotNull(collision);
            Assert.IsTrue(collision.gameObject.activeInHierarchy);
            Assert.AreEqual(GeometryTypes.Box, collision.geometryType);

            Object.DestroyImmediate(parent.gameObject);
        }

        [Test]
        public void Create_GeometryBoxWithVisual_NondefaultScale()
        {
            RuntimeURDF.runtimeModeEnabled = false;
            var parent = new GameObject("Parent").transform;
            var copy = new GameObject("Copy").transform;
            copy.transform.localScale = Vector3.one * 2f;
            UrdfCollisionExtensions.Create(parent, GeometryTypes.Box, copy);

            var collision = parent.Find("unnamed").GetComponent<UrdfCollision>();
            Assert.IsNotNull(collision);
            Assert.IsTrue(collision.gameObject.activeInHierarchy);
            Assert.AreEqual(GeometryTypes.Box, collision.geometryType);
            Assert.AreEqual(Vector3.one * 2f, collision.transform.localScale);

            Object.DestroyImmediate(parent.gameObject);
        }

        [Test]
        public void Create_GeometryMesh_DefaultGeometry()
        {
            RuntimeURDF.runtimeModeEnabled = false;
            var parent = new GameObject("Parent").transform;
            UrdfCollisionExtensions.Create(parent, GeometryTypes.Mesh);

            var collision = parent.Find("unnamed").GetComponent<UrdfCollision>();
            Assert.IsNotNull(collision);
            Assert.IsTrue(collision.gameObject.activeInHierarchy);
            Assert.AreEqual(GeometryTypes.Mesh, collision.geometryType);

            Object.DestroyImmediate(parent.gameObject);
        }

        [Test]
        public void Create_GeometryMeshWithVisual_NondefaultScale()
        {
            RuntimeURDF.runtimeModeEnabled = false;
            var parent = new GameObject("Parent").transform;
            var copy = new GameObject("Copy").transform;
            copy.transform.localScale = Vector3.one * 2f;
            UrdfCollisionExtensions.Create(parent, GeometryTypes.Mesh, copy);

            var collision = parent.Find("unnamed").GetComponent<UrdfCollision>();
            Assert.IsNotNull(collision);
            Assert.IsTrue(collision.gameObject.activeInHierarchy);
            Assert.AreEqual(GeometryTypes.Mesh, collision.geometryType);
            Assert.AreEqual(Vector3.one * 2f, collision.transform.localScale);

            Object.DestroyImmediate(parent.gameObject);
        }

        [Test]
        public void Create_LinkCollision_DefaultBox()
        {
            RuntimeURDF.runtimeModeEnabled = false;
            var parent = new GameObject("Parent").transform;
            var geometry = new Link.Geometry(box:new Link.Geometry.Box(new double[] {1, 1, 1}));
            UrdfCollisionExtensions.Create(parent, new Link.Collision(geometry));

            var collision = parent.Find("unnamed").GetComponent<UrdfCollision>();
            Assert.IsNotNull(collision);
            Assert.IsTrue(collision.gameObject.activeInHierarchy);
            Assert.AreEqual(GeometryTypes.Box, collision.geometryType);
            Assert.IsTrue(collision.GetComponentsInChildren<BoxCollider>().Length > 0);

            Object.DestroyImmediate(parent.gameObject);
        }

        [Test]
        public void ExportCollisionData_DefaultBox()
        {
            RuntimeURDF.runtimeModeEnabled = false;
            var parent = new GameObject("Parent").transform;
            UrdfCollisionExtensions.Create(parent, GeometryTypes.Box);
            var collisionComponent = parent.Find("unnamed").GetComponent<UrdfCollision>();
            var exported = UrdfCollisionExtensions.ExportCollisionData(collisionComponent);
            Assert.IsNotNull(exported);
            Assert.IsNull(exported.name);

            Object.DestroyImmediate(parent.gameObject);
        }
    }
}
