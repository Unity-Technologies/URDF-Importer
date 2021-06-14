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

namespace Unity.Robotics.UrdfImporter.Tests
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
