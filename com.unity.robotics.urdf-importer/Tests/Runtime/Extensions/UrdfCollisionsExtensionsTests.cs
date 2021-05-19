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
    public class UrdfCollisionsExtensionsTests
    {
        [Test]
        public void Create_NullCollisions_DefaultComponents()
        {
            var parent = new GameObject("Parent").transform;
            UrdfCollisionsExtensions.Create(parent);
            Assert.IsNotNull(parent.Find("Collisions"));
            Assert.IsNotNull(parent.Find("Collisions").GetComponent<UrdfCollisions>());

            Object.DestroyImmediate(parent.gameObject);
        }

        [Test]
        public void Create_WithCollisions_DefaultComponents()
        {
            var parent = new GameObject("Parent").transform;
            var geom = new Link.Geometry(box: new Link.Geometry.Box(new double[] {1, 1, 1}));
            UrdfCollisionsExtensions.Create(parent, new List<Link.Collision>() {new Link.Collision(geom)});
            Assert.IsNotNull(parent.Find("Collisions"));
            Assert.IsTrue(parent.GetComponentsInChildren<UrdfCollisions>().Length > 0);

            Object.DestroyImmediate(parent.gameObject);
        }

        [Test]
        public void ExportCollisionsData_Box_DefaultComponents()
        {
            var parent = new GameObject("Parent").transform;
            var geom = new Link.Geometry(box: new Link.Geometry.Box(new double[] {1, 1, 1}));
            UrdfCollisionsExtensions.Create(parent, new List<Link.Collision>() {new Link.Collision(geom)});
            var urdfColl = parent.Find("Collisions").GetComponent<UrdfCollisions>();
            Assert.IsTrue(urdfColl.ExportCollisionsData().Count > 0);

            Object.DestroyImmediate(parent.gameObject);
        }
    }
}
