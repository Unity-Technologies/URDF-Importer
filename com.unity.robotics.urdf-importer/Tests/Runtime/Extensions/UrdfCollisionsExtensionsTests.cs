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
        private static IEnumerable<TestCaseData> GeometryData
        {
            get
            {
                yield return new TestCaseData(new Geometry(box: new Box(new double[] {1, 1, 1})));
            }
        }

        [Test]
        public void Create_NullCollisions_DefaultComponents()
        {
            var parent = new GameObject("Parent").transform;
            var collisions = UrdfCollisionsExtensions.Create(parent);
            Assert.IsNotNull(collisions);
            Assert.IsNotNull(collisions.GetComponent<UrdfCollisions>());

            Object.DestroyImmediate(parent.gameObject);
        }

        [Test, TestCaseSource("GeometryData")]
        public void Create_WithCollisions_DefaultComponents(Geometry geometryBox)
        {
            var parent = new GameObject("Parent").transform;
            var collisions = UrdfCollisionsExtensions.Create(parent, new List<Collision>() {new Collision(geometryBox)});
            Assert.IsNotNull(collisions);
            Assert.IsTrue(parent.GetComponentsInChildren<UrdfCollisions>().Length > 0);

            Object.DestroyImmediate(parent.gameObject);
        }

        [Test, TestCaseSource("GeometryData")]
        public void ExportCollisionsData_DefaultGeometry_DefaultComponents(Geometry geometryBox)
        {
            var parent = new GameObject("Parent").transform;
            var collisions = UrdfCollisionsExtensions.Create(parent, new List<Collision>() {new Collision(geometryBox)});
            var urdfColl = collisions.GetComponent<UrdfCollisions>();
            Assert.IsTrue(urdfColl.ExportCollisionsData().Count > 0);

            Object.DestroyImmediate(parent.gameObject);
        }
    }
}
