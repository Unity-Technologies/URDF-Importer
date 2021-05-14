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
        public void ExportGeometryData_Box_DefaultGeometry()
        {
            var parent = new GameObject("Parent").transform;
            UrdfGeometryCollision.Create(parent, GeometryTypes.Box);
            var t = parent.transform.Find("Box");
            var export = UrdfGeometry.ExportGeometryData(GeometryTypes.Box, t);
            Assert.IsNotNull(export);
            Assert.AreEqual(new double[] {1, 1, 1}, export.box.size);
            Assert.AreEqual(typeof(Link.Geometry.Box), export.box.GetType());
            Assert.IsNull(export.cylinder);
            Assert.IsNull(export.sphere);
            Assert.IsNull(export.mesh);
        }

        [Test]
        public void ExportGeometryData_Cylinder_DefaultGeometry()
        {
            // Force runtime mode to avoid creating asset file
            RuntimeURDF.runtimeModeEnabled = true;
            var parent = new GameObject("Parent").transform;
            UrdfGeometryCollision.Create(parent, GeometryTypes.Cylinder);

            var t = parent.transform.Find("Cylinder");
            var export = UrdfGeometry.ExportGeometryData(GeometryTypes.Cylinder, t);
            Assert.IsNotNull(export);
            Assert.AreEqual(0.5, export.cylinder.radius);
            Assert.AreEqual(2, export.cylinder.length);
            Assert.AreEqual(typeof(Link.Geometry.Cylinder), export.cylinder.GetType());
            Assert.IsNull(export.box);
            Assert.IsNull(export.sphere);
            Assert.IsNull(export.mesh);
        }

        [Test]
        public void ExportGeometryData_Sphere_DefaultGeometry()
        {
            var parent = new GameObject("Parent").transform;
            UrdfGeometryCollision.Create(parent, GeometryTypes.Sphere);
            var t = parent.transform.Find("Sphere");
            var export = UrdfGeometry.ExportGeometryData(GeometryTypes.Sphere, t);
            Assert.IsNotNull(export);
            Assert.AreEqual(0.5, export.sphere.radius);
            Assert.AreEqual(typeof(Link.Geometry.Sphere), export.sphere.GetType());
            Assert.IsNull(export.box);
            Assert.IsNull(export.cylinder);
            Assert.IsNull(export.mesh);
        }

        [Test]
        public void GetGeometryTypes_All_DefaultEnums()
        {
            Assert.AreEqual(GeometryTypes.Box, UrdfGeometry.GetGeometryType(box));
            Assert.AreEqual(GeometryTypes.Cylinder, UrdfGeometry.GetGeometryType(cylinder));
            Assert.AreEqual(GeometryTypes.Sphere, UrdfGeometry.GetGeometryType(sphere));
        }

        [Test]
        public void SetScale_Box_IncreaseLocalScale()
        {
            var parent = new GameObject("Parent").transform;
            UrdfGeometry.SetScale(parent, box, UrdfGeometry.GetGeometryType(box));
            Assert.AreEqual(new Vector3(3, 4, 2), parent.transform.localScale);
        }

        [Test]
        public void SetScale_Cylinder_IncreaseLocalScale()
        {
            var parent = new GameObject("Parent").transform;
            UrdfGeometry.SetScale(parent, cylinder, UrdfGeometry.GetGeometryType(cylinder));
            Assert.AreEqual(Vector3.one * 2f, parent.transform.localScale);
        }

        [Test]
        public void SetScale_Sphere_IncreaseLocalScale()
        {
            var parent = new GameObject("Parent").transform;
            UrdfGeometry.SetScale(parent, sphere, UrdfGeometry.GetGeometryType(sphere));
            Assert.AreEqual(Vector3.one * 2f, parent.transform.localScale);
        }

        [Test]
        public void IsTransformed_Default_False()
        {
            var parent = new GameObject("Parent").transform;
            Assert.IsFalse(UrdfGeometry.IsTransformed(parent, UrdfGeometry.GetGeometryType(box)));
        }

        [Test]
        public void IsTransformed_Position_True()
        {
            var parent = new GameObject("Parent").transform;
            parent.transform.localPosition = Vector3.one;
            Assert.IsTrue(UrdfGeometry.IsTransformed(parent, UrdfGeometry.GetGeometryType(box)));
        }

        [Test]
        public void IsTransformed_Scale_True()
        {
            var parent = new GameObject("Parent").transform;
            parent.transform.localScale = Vector3.one * 2f;
            Assert.IsTrue(UrdfGeometry.IsTransformed(parent, UrdfGeometry.GetGeometryType(box)));
        }

        [Test]
        public void IsTransformed_Rotation_True()
        {
            var parent = new GameObject("Parent").transform;
            parent.transform.localRotation = Quaternion.Euler(0, 30, 0);
            Assert.IsTrue(UrdfGeometry.IsTransformed(parent, UrdfGeometry.GetGeometryType(box)));
        }
    }
}
