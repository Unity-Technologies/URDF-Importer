using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEditor;
using UnityEngine.TestTools;
using RosSharp.Urdf;
using Object = UnityEngine.Object;
using Collision = RosSharp.Urdf.Link.Collision;
using Geometry = RosSharp.Urdf.Link.Geometry;
using Box = RosSharp.Urdf.Link.Geometry.Box;

namespace RosSharp.Urdf.Tests
{
    public class UrdfCollisionExtensionsTests
    {
        [Test]
        public void Create_GeometryBox_DefaultGeometry()
        {
            RuntimeURDF.runtimeModeEnabled = false;
            var parent = new GameObject("Parent").transform;

            var collision = UrdfCollisionExtensions.Create(parent, GeometryTypes.Box);
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

            var collision = UrdfCollisionExtensions.Create(parent, GeometryTypes.Box, copy);
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

            var collision = UrdfCollisionExtensions.Create(parent, GeometryTypes.Mesh);
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

            var collision = UrdfCollisionExtensions.Create(parent, GeometryTypes.Mesh, copy);
            Assert.IsNotNull(collision);
            Assert.IsTrue(collision.gameObject.activeInHierarchy);
            Assert.AreEqual(GeometryTypes.Mesh, collision.geometryType);
            Assert.AreEqual(Vector3.one * 2f, collision.transform.localScale);

            Object.DestroyImmediate(parent.gameObject);
        }

        [Test]
        public void Create_LinkCollision_DefaultGeometry()
        {
            RuntimeURDF.runtimeModeEnabled = false;
            var parent = new GameObject("Parent").transform;
            var geometry = new Geometry(box:new Box(new double[] {1, 1, 1}));

            var collision = UrdfCollisionExtensions.Create(parent, new Collision(geometry));
            Assert.IsNotNull(collision);
            Assert.IsTrue(collision.gameObject.activeInHierarchy);
            Assert.AreEqual(GeometryTypes.Box, collision.geometryType);
            Assert.IsTrue(collision.GetComponentsInChildren<BoxCollider>().Length > 0);

            Object.DestroyImmediate(parent.gameObject);
        }

        [Test]
        public void ExportCollisionData_DefaultGeometry_Succeeds()
        {
            RuntimeURDF.runtimeModeEnabled = false;
            var parent = new GameObject("Parent").transform;

            var collisionComponent = UrdfCollisionExtensions.Create(parent, GeometryTypes.Box);
            var exported = UrdfCollisionExtensions.ExportCollisionData(collisionComponent);
            Assert.IsNotNull(exported.geometry.box);
            Assert.AreEqual(new double[] {1,1,1}, exported.geometry.box.size);
            Assert.IsNull(exported.name);

            Object.DestroyImmediate(parent.gameObject);
        }
    }
}
