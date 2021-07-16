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
using Cylinder = Unity.Robotics.UrdfImporter.Link.Geometry.Cylinder;
using Sphere = Unity.Robotics.UrdfImporter.Link.Geometry.Sphere;
using Mesh = Unity.Robotics.UrdfImporter.Link.Geometry.Mesh;

/// Sample STL from Obijuan.cube, Public domain, via Wikimedia Commons
/// https://commons.wikimedia.org/wiki/File:3D_model_of_a_Cube.stl

namespace Unity.Robotics.UrdfImporter.Tests
{
    public class UrdfGeometryTests
    {
        private static IEnumerable<TestCaseData> GeometryTypesData
        {
            get
            {
                yield return new TestCaseData(new Geometry(box: new Box(new double[] { 2, 3, 4 })), new Vector3(3, 4, 2), GeometryTypes.Box);
                yield return new TestCaseData(new Geometry(cylinder: new Cylinder(1, 4)), Vector3.one * 2,GeometryTypes.Cylinder);
                yield return new TestCaseData(new Geometry(sphere: new Sphere(1)), Vector3.one * 2,GeometryTypes.Sphere);
            }
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
            Assert.AreEqual(typeof(Box), export.box.GetType());
            Assert.IsNull(export.cylinder);
            Assert.IsNull(export.sphere);
            Assert.IsNull(export.mesh);
            
            Object.DestroyImmediate(parent.gameObject);
        }

        [Test]
        public void ExportGeometryData_Cylinder_DefaultGeometry()
        {
            // Force runtime mode to avoid creating asset file
            RuntimeUrdf.runtimeModeEnabled = true;
            var parent = new GameObject("Parent").transform;
            UrdfGeometryCollision.Create(parent, GeometryTypes.Cylinder);

            var t = parent.Find("Cylinder");
            var export = UrdfGeometry.ExportGeometryData(GeometryTypes.Cylinder, t);
            Assert.IsNotNull(export);
            Assert.AreEqual(0.5, export.cylinder.radius);
            Assert.AreEqual(2, export.cylinder.length);
            Assert.AreEqual(typeof(Cylinder), export.cylinder.GetType());
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
            Assert.AreEqual(typeof(Sphere), export.sphere.GetType());
            Assert.IsNull(export.box);
            Assert.IsNull(export.cylinder);
            Assert.IsNull(export.mesh);

            Object.DestroyImmediate(parent.gameObject);
        }

        [Test]
        public void ExportGeometryData_MeshUnityDecomposer_DefaultGeometry()
        {
            // Force runtime mode to set testing package root
            RuntimeUrdf.runtimeModeEnabled = true;
            UrdfAssetPathHandler.SetPackageRoot("Packages/com.unity.robotics.urdf-importer/Tests/Runtime/Assets/URDF/cube/");
            RuntimeUrdf.runtimeModeEnabled = false;
            UrdfRobotExtensions.importsettings = ImportSettings.DefaultSettings();
            UrdfRobotExtensions.importsettings.convexMethod = ImportSettings.convexDecomposer.unity;
            
            var parent = new GameObject("Parent").transform;
            string path = "package://meshes/cube.stl";
            var meshGeometry = new Geometry(mesh: new Mesh(path, new double[] {1,1,1}));
            UrdfCollisionExtensions.Create(parent, new Collision(meshGeometry));

            UrdfExportPathHandler.SetExportPath("Assets");
            var t = parent.GetComponentInChildren<UrdfCollision>().transform.GetChild(0);
            var export = UrdfGeometry.ExportGeometryData(GeometryTypes.Mesh, t);
            Assert.IsNotNull(export);

            Object.DestroyImmediate(parent.gameObject);
            List<string> outFailedPaths = new List<string>();
            AssetDatabase.DeleteAssets(new string[] {"Assets/meshes"}, outFailedPaths);
        }

        [Test, TestCaseSource("GeometryTypesData")]
        public void GetGeometryTypes_All_DefaultEnums(Geometry geometry, Vector3 scale, GeometryTypes type)
        {
            Assert.AreEqual(type, UrdfGeometry.GetGeometryType(geometry));
        }

        [Test, TestCaseSource("GeometryTypesData")]
        public void SetScale_Box_IncreaseLocalScale(Geometry geometry, Vector3 scale, GeometryTypes type)
        {
            var parent = new GameObject("Parent").transform;
            UrdfGeometry.SetScale(parent, geometry, UrdfGeometry.GetGeometryType(geometry));
            Assert.AreEqual(scale, parent.transform.localScale);

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

        [Test, TestCaseSource("GeometryTypesData")]
        public void IsTransformed_Default_False(Geometry geometry, Vector3 scale, GeometryTypes type)
        {
            var parent = new GameObject("Parent").transform;
            Assert.IsFalse(UrdfGeometry.IsTransformed(parent, UrdfGeometry.GetGeometryType(geometry)));

            Object.DestroyImmediate(parent.gameObject);
        }

        [Test, TestCaseSource("GeometryTypesData")]
        public void IsTransformed_Position_True(Geometry geometry, Vector3 scale, GeometryTypes type)
        {
            var parent = new GameObject("Parent").transform;
            parent.localPosition = Vector3.one;
            Assert.IsTrue(UrdfGeometry.IsTransformed(parent, UrdfGeometry.GetGeometryType(geometry)));

            Object.DestroyImmediate(parent.gameObject);
        }

        [Test, TestCaseSource("GeometryTypesData")]
        public void IsTransformed_Scale_True(Geometry geometry, Vector3 scale, GeometryTypes type)
        {
            var parent = new GameObject("Parent").transform;
            parent.localScale = Vector3.one * 2f;
            Assert.IsTrue(UrdfGeometry.IsTransformed(parent, UrdfGeometry.GetGeometryType(geometry)));

            Object.DestroyImmediate(parent.gameObject);
        }

        [Test, TestCaseSource("GeometryTypesData")]
        public void IsTransformed_Rotation_True(Geometry geometry, Vector3 scale, GeometryTypes type)
        {
            var parent = new GameObject("Parent").transform;
            parent.localRotation = Quaternion.Euler(0, 30, 0);
            Assert.IsTrue(UrdfGeometry.IsTransformed(parent, UrdfGeometry.GetGeometryType(geometry)));

            Object.DestroyImmediate(parent.gameObject);
        }
    }
}
