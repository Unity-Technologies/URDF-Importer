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
    public class UrdfGeometryTests
    {
        Link.Geometry box;
        Link.Geometry cylinder;
        Link.Geometry sphere;

        [SetUp]
        public void SetUp()
        {
            box = new Link.Geometry(box: new Link.Geometry.Box(new double[] { 2, 3, 4 }));
            cylinder = new Link.Geometry(cylinder: new Link.Geometry.Cylinder(1, 4));
            sphere = new Link.Geometry(sphere: new Link.Geometry.Sphere(1));
        }

        [Test]
        public void ExportGeometryData_Box()
        {
            var parent = new GameObject("Parent").transform;
            UrdfGeometryCollision.Create(parent, GeometryTypes.Box);
            var t = parent.transform.Find("Box");
            Assert.IsNotNull(UrdfGeometry.ExportGeometryData(GeometryTypes.Box, t));
        }

        [Test]
        public void ExportGeometryData_Cylinder()
        {
            // Force runtime mode to avoid creating asset file
            RuntimeURDF.runtimeModeEnabled = true;
            var parent = new GameObject("Parent").transform;
            UrdfGeometryCollision.Create(parent, GeometryTypes.Cylinder);

            var t = parent.transform.Find("Cylinder");
            Assert.IsNotNull(UrdfGeometry.ExportGeometryData(GeometryTypes.Cylinder, t));
        }

        [Test]
        public void ExportGeometryData_Sphere()
        {
            var parent = new GameObject("Parent").transform;
            UrdfGeometryCollision.Create(parent, GeometryTypes.Sphere);
            var t = parent.transform.Find("Sphere");
            Assert.IsNotNull(UrdfGeometry.ExportGeometryData(GeometryTypes.Sphere, t));
        }

        [Test]
        public void GetGeometryTypes_All()
        {
            Assert.AreEqual(GeometryTypes.Box, UrdfGeometry.GetGeometryType(box));
            Assert.AreEqual(GeometryTypes.Cylinder, UrdfGeometry.GetGeometryType(cylinder));
            Assert.AreEqual(GeometryTypes.Sphere, UrdfGeometry.GetGeometryType(sphere));
        }

        [Test]
        public void SetScale_Box()
        {
            var parent = new GameObject("Parent").transform;
            UrdfGeometry.SetScale(parent, box, UrdfGeometry.GetGeometryType(box));
            Assert.AreEqual(new Vector3(3, 4, 2), parent.transform.localScale);
        }

        [Test]
        public void SetScale_Cylinder()
        {
            var parent = new GameObject("Parent").transform;
            UrdfGeometry.SetScale(parent, cylinder, UrdfGeometry.GetGeometryType(cylinder));
            Assert.AreEqual(Vector3.one * 2f, parent.transform.localScale);
        }

        [Test]
        public void SetScale_Sphere()
        {
            var parent = new GameObject("Parent").transform;
            UrdfGeometry.SetScale(parent, sphere, UrdfGeometry.GetGeometryType(sphere));
            Assert.AreEqual(Vector3.one * 2f, parent.transform.localScale);
        }

        [Test]
        public void IsTransformed_False()
        {
            var parent = new GameObject("Parent").transform;
            Assert.IsFalse(UrdfGeometry.IsTransformed(parent, UrdfGeometry.GetGeometryType(box)));
        }

        [Test]
        public void IsTransformed_Position()
        {
            var parent = new GameObject("Parent").transform;
            parent.transform.localPosition = Vector3.one;
            Assert.IsTrue(UrdfGeometry.IsTransformed(parent, UrdfGeometry.GetGeometryType(box)));
        }

        [Test]
        public void IsTransformed_Scale()
        {
            var parent = new GameObject("Parent").transform;
            parent.transform.localScale = Vector3.one * 2f;
            Assert.IsTrue(UrdfGeometry.IsTransformed(parent, UrdfGeometry.GetGeometryType(box)));
        }

        [Test]
        public void IsTransformed_Rotation()
        {
            var parent = new GameObject("Parent").transform;
            parent.transform.localRotation = Quaternion.Euler(0, 30, 0);
            Assert.IsTrue(UrdfGeometry.IsTransformed(parent, UrdfGeometry.GetGeometryType(box)));
        }
    }
}