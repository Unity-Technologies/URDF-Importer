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
    public class UrdfCollisionsExtensionsTests
    {
        Geometry geometryBox = new Geometry(box: new Box(new double[] {1, 1, 1}));

        [Test]
        public void Create_NullCollisions_DefaultComponents()
        {
            var parent = new GameObject("Parent").transform;
            var collisions = UrdfCollisionsExtensions.Create(parent);
            Assert.IsNotNull(collisions);
            Assert.IsNotNull(collisions.GetComponent<UrdfCollisions>());

            Object.DestroyImmediate(parent.gameObject);
        }

        [Test]
        public void Create_WithCollisions_DefaultComponents()
        {
            var parent = new GameObject("Parent").transform;
            var collisions = UrdfCollisionsExtensions.Create(parent, new List<Collision>() {new Collision(geometryBox)});
            Assert.IsNotNull(collisions);
            Assert.IsTrue(parent.GetComponentsInChildren<UrdfCollisions>().Length > 0);

            Object.DestroyImmediate(parent.gameObject);
        }

        [Test]
        public void ExportCollisionsData_Box_DefaultComponents()
        {
            var parent = new GameObject("Parent").transform;
            var collisions = UrdfCollisionsExtensions.Create(parent, new List<Collision>() {new Collision(geometryBox)});
            var urdfColl = collisions.GetComponent<UrdfCollisions>();
            Assert.IsTrue(urdfColl.ExportCollisionsData().Count > 0);

            Object.DestroyImmediate(parent.gameObject);
        }
    }
}
