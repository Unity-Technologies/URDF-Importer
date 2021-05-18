using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEditor;
using UnityEngine.TestTools;
using RosSharp.Urdf;
using Object = UnityEngine.Object;

/// Sample STL from Obijuan.cube, Public domain, via Wikimedia Commons
/// https://commons.wikimedia.org/wiki/File:3D_model_of_a_Cube.stl

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
            var t = parent.Find("Box");
            var export = UrdfGeometry.ExportGeometryData(GeometryTypes.Box, t);
            Assert.IsNotNull(export);
            Assert.AreEqual(new double[] {1, 1, 1}, export.box.size);
            Assert.AreEqual(typeof(Link.Geometry.Box), export.box.GetType());
            Assert.IsNull(export.cylinder);
            Assert.IsNull(export.sphere);
            Assert.IsNull(export.mesh);
            
            Object.DestroyImmediate(parent.gameObject);
        }

        [Test]
        public void ExportGeometryData_Cylinder_DefaultGeometry()
        {
            // Force runtime mode to avoid creating asset file
            RuntimeURDF.runtimeModeEnabled = true;
            var parent = new GameObject("Parent").transform;
            UrdfGeometryCollision.Create(parent, GeometryTypes.Cylinder);

            var t = parent.Find("Cylinder");
            var export = UrdfGeometry.ExportGeometryData(GeometryTypes.Cylinder, t);
            Assert.IsNotNull(export);
            Assert.AreEqual(0.5, export.cylinder.radius);
            Assert.AreEqual(2, export.cylinder.length);
            Assert.AreEqual(typeof(Link.Geometry.Cylinder), export.cylinder.GetType());
            Assert.IsNull(export.box);
            Assert.IsNull(export.sphere);
            Assert.IsNull(export.mesh);

            Object.DestroyImmediate(parent.gameObject);
        }

        [Test]
        public void ExportGeometryData_Sphere_DefaultGeometry()
        {
            var parent = new GameObject("Parent").transform;
            UrdfGeometryCollision.Create(parent, GeometryTypes.Sphere);
            var t = parent.Find("Sphere");
            var export = UrdfGeometry.ExportGeometryData(GeometryTypes.Sphere, t);
            Assert.IsNotNull(export);
            Assert.AreEqual(0.5, export.sphere.radius);
            Assert.AreEqual(typeof(Link.Geometry.Sphere), export.sphere.GetType());
            Assert.IsNull(export.box);
            Assert.IsNull(export.cylinder);
            Assert.IsNull(export.mesh);

            Object.DestroyImmediate(parent.gameObject);
        }

        [Test]
        public void ExportGeometryData_Mesh_DefaultGeometry()
        {
            // Force runtime mode to set testing package root
            RuntimeURDF.runtimeModeEnabled = true;
            UrdfAssetPathHandler.SetPackageRoot("Packages/com.unity.robotics.urdf-importer/Tests/Runtime/Assets/URDF/cube/");
            RuntimeURDF.runtimeModeEnabled = false;
            UrdfRobotExtensions.importsettings = ImportSettings.DefaultSettings();
            UrdfRobotExtensions.importsettings.convexMethod = ImportSettings.convexDecomposer.unity;
            
            var parent = new GameObject("Parent").transform;
            string path = "package://meshes/cube.stl";
            var meshGeometry = new Link.Geometry(mesh: new Link.Geometry.Mesh(path, new double[] {1,1,1}));
            UrdfCollisionExtensions.Create(parent, new Link.Collision(meshGeometry));

            UrdfExportPathHandler.SetExportPath("Assets");
            var t = parent.GetComponentInChildren<UrdfCollision>().transform.GetChild(0);
            var export = UrdfGeometry.ExportGeometryData(GeometryTypes.Mesh, t);
            Assert.IsNotNull(export);

            Object.DestroyImmediate(parent.gameObject);
            List<string> outFailedPaths = new List<string>();
            AssetDatabase.DeleteAssets(new string[] {"Assets/meshes"}, outFailedPaths);
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

            Object.DestroyImmediate(parent.gameObject);
        }

        [Test]
        public void SetScale_Cylinder_IncreaseLocalScale()
        {
            var parent = new GameObject("Parent").transform;
            UrdfGeometry.SetScale(parent, cylinder, UrdfGeometry.GetGeometryType(cylinder));
            Assert.AreEqual(Vector3.one * 2f, parent.transform.localScale);

            Object.DestroyImmediate(parent.gameObject);
        }

        [Test]
        public void SetScale_Sphere_IncreaseLocalScale()
        {
            var parent = new GameObject("Parent").transform;
            UrdfGeometry.SetScale(parent, sphere, UrdfGeometry.GetGeometryType(sphere));
            Assert.AreEqual(Vector3.one * 2f, parent.transform.localScale);

            Object.DestroyImmediate(parent.gameObject);
        }

        [Test]
        public void CheckForUrdfCompatibility_UnityPrimitive_True()
        {
            var parent = new GameObject("Parent").transform;
            var child = new GameObject("Child").transform;
            child.parent = parent;
            Assert.IsTrue(UrdfGeometry.CheckForUrdfCompatibility(parent, GeometryTypes.Box));

            Object.DestroyImmediate(parent.gameObject);
        }

        [Test]
        public void CheckForUrdfCompatibility_NoChildren_False()
        {
            var parent = new GameObject("Parent").transform;
            Assert.IsFalse(UrdfGeometry.CheckForUrdfCompatibility(parent, GeometryTypes.Box));

            Object.DestroyImmediate(parent.gameObject);
        }

        [Test]
        public void CheckForUrdfCompatibility_Transformed_False()
        {
            var parent = new GameObject("Parent").transform;
            var child = new GameObject("Child").transform;
            child.parent = parent;
            child.localPosition = Vector3.one;
            Assert.IsFalse(UrdfGeometry.CheckForUrdfCompatibility(parent, GeometryTypes.Box));

            Object.DestroyImmediate(parent.gameObject);
        }

        [Test]
        public void CheckForUrdfCompatibility_MultipleChildren_False()
        {
            var parent = new GameObject("Parent").transform;
            var child0 = new GameObject("Child0").transform;
            var child1 = new GameObject("Child1").transform;
            child0.parent = parent;
            child1.parent = parent;
            Assert.IsFalse(UrdfGeometry.CheckForUrdfCompatibility(parent, GeometryTypes.Box));

            Object.DestroyImmediate(parent.gameObject);
        }

        [Test]
        public void IsTransformed_Default_False()
        {
            var parent = new GameObject("Parent").transform;
            Assert.IsFalse(UrdfGeometry.IsTransformed(parent, UrdfGeometry.GetGeometryType(box)));

            Object.DestroyImmediate(parent.gameObject);
        }

        [Test]
        public void IsTransformed_Position_True()
        {
            var parent = new GameObject("Parent").transform;
            parent.localPosition = Vector3.one;
            Assert.IsTrue(UrdfGeometry.IsTransformed(parent, UrdfGeometry.GetGeometryType(box)));

            Object.DestroyImmediate(parent.gameObject);
        }

        [Test]
        public void IsTransformed_Scale_True()
        {
            var parent = new GameObject("Parent").transform;
            parent.localScale = Vector3.one * 2f;
            Assert.IsTrue(UrdfGeometry.IsTransformed(parent, UrdfGeometry.GetGeometryType(box)));

            Object.DestroyImmediate(parent.gameObject);
        }

        [Test]
        public void IsTransformed_Rotation_True()
        {
            var parent = new GameObject("Parent").transform;
            parent.localRotation = Quaternion.Euler(0, 30, 0);
            Assert.IsTrue(UrdfGeometry.IsTransformed(parent, UrdfGeometry.GetGeometryType(box)));

            Object.DestroyImmediate(parent.gameObject);
        }
    }
}
