using System;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using Assert = UnityEngine.Assertions.Assert;

namespace Unity.Robotics.UrdfImporter
{
    public class UrdfExportTests
    {
        Robot m_Original;
        Robot m_Export;
        string m_UrdfFilePath = $"{Application.dataPath.Substring(0, Application.dataPath.Length - "Assets".Length)}" +
            "Packages/com.unity.robotics.urdf-importer/Tests/Runtime/Assets/URDF/cube/";
        string m_OriginalUrdfFile = "cube_export.urdf";

        [Test]
        public void ExportRobotToUrdf_UrdfFile_ExportedFileEqual()
        {
            var robotEnumerator =  UrdfRobotExtensions.Create(Path.Combine(m_UrdfFilePath,m_OriginalUrdfFile), ImportSettings.DefaultSettings());
            robotEnumerator.MoveNext();
            var robot = robotEnumerator.Current;
            UrdfRobot robotObject = robot.GetComponent<UrdfRobot>();
            Directory.CreateDirectory(m_UrdfFilePath + "tmp");
            robotObject.ExportRobotToUrdf(m_UrdfFilePath, "tmp");
            string exportedFilePath = Path.Combine(m_UrdfFilePath, "tmp", robot.name + ".urdf");
            m_Original = new Robot(Path.Combine(m_UrdfFilePath, m_OriginalUrdfFile));
            m_Export = new Robot(exportedFilePath);
            Compare();
        }

        void Compare()
        {
            Assert.AreEqual(m_Original.links.Count, m_Export.links.Count, $"Number of links in exported and source file are unequal: {m_Export.links.Count}, {m_Original.links.Count}");
            Assert.AreEqual(m_Original.joints.Count, m_Export.joints.Count, $"Number of joints in exported and source file are unequal: {m_Export.joints.Count}, {m_Original.joints.Count}");

            CompareLink(m_Original.root, m_Export.root);
        }

        /// <summary>
        /// Compares two links of a robot
        /// </summary>
        /// <param name="source">First link to be compared</param>
        /// <param name="exported">Second link to be compared</param>
        /// <returns></returns>
        void CompareLink(Link source, Link exported)
        {
            Assert.AreEqual(source.name, exported.name, $"Joint names are not the same: Expected: {source.name} Actual: {exported.name}");
            CompareInertial(source.inertial, exported.inertial);

            Assert.AreEqual(source.visuals.Count, exported.visuals.Count, $"Number of visual meshes are unequal: Expected: {source.visuals.Count} Actual: {exported.visuals.Count}");
            for (var i = 0; i < source.visuals.Count; i++) 
                CompareVisual(source.visuals[i], exported.visuals[i]);

            Assert.AreEqual(source.collisions.Count, exported.collisions.Count, $"Number of collision meshes are unequal: Expected: {source.collisions.Count} Actual: {exported.collisions.Count}");
            for (var i = 0; i < source.collisions.Count; i++) 
                CompareCollisions(source.collisions[i], exported.collisions[i]);

            Assert.AreEqual(source.joints.Count, exported.joints.Count, $"Number of joints connected to {source.name} link are unequal: Expected: {source.joints.Count} Actual: {exported.joints.Count}");
            foreach (var jointSource in source.joints)
            {
                var jointExported = exported.joints.Find(x => x.name == jointSource.name); // Check for no match
                Assert.IsNotNull(jointExported, $"Joint name not found in the exported URDF file");
                CompareJoint(jointSource, jointExported);
            }
        }

        /// <summary>
        /// Compares two joint of a robot
        /// </summary>
        /// <param name="source">First joint to be compared</param>
        /// <param name="exported">Second joint to be compared</param>
        /// <returns></returns>
        void CompareJoint(Joint source, Joint exported) // This function does not test for Mimic, Calibration and SafetyController as they are not imported in Unity
        {
            Assert.AreEqual(source.name, exported.name, $"Name of the joint is unequal: Expected: {source.name} Actual: {exported.name}");
            Assert.AreEqual(source.type, exported.type, $"Joint type is unequal: Expected: {source.type} Actual: {exported.type}");

            CompareOrigin(source.origin, exported.origin);
            Assert.AreEqual(source.parent, exported.parent, $"Parent name is unequal: Expected: {source.parent} Actual: {exported.parent}");

            CompareAxis(source.axis, exported.axis);
            CompareDynamics(source.dynamics, exported.dynamics);
            if (source.type == "revolute" || source.type == "prismatic") 
                CompareLimit(source.limit, exported.limit);

            Assert.AreEqual(source.child, exported.child, $"Child name is unequal: Expected: {source.child} Actual: {exported.child}");
            if (source.child != null)
                CompareLink(source.ChildLink, exported.ChildLink);
        }

        /// <summary>
        /// Compares inertial information of two links
        /// </summary>
        /// <param name="source">First link's inertial information to be compared</param>
        /// <param name="exported">Second link's inertial information to be compared</param>
        /// <returns></returns>
        static void CompareInertial(Link.Inertial source, Link.Inertial exported)
        {
            if (source == null && exported == null) 
                return;

            Assert.IsTrue(source == null && exported == null || source != null && exported != null, $"Both joints' inertial is not null");
            Assert.IsTrue(source.mass.EqualsDelta(exported.mass, .005), $"Mass is unequal: Expected: {source.mass} Actual: {exported.mass}");

            CompareOrigin(source.origin, exported.origin);
        }

        /// <summary>
        /// Compares two origin information of two links
        /// </summary>
        /// <param name="source">First link's origin information to be compared</param>
        /// <param name="exported">Second link's origin information to be compared</param>
        /// <returns></returns>
        static void CompareOrigin(Origin source, Origin exported)
        {
            if (source == null && exported == null) 
                return;

            Assert.IsTrue(source != null && exported == null && source.Xyz.ToVector3() == new Vector3(0, 0, 0) && source.Rpy.ToVector3() == new Vector3(0, 0, 0)
                || exported != null && source == null && exported.Xyz.ToVector3() == new Vector3(0, 0, 0) && exported.Rpy.ToVector3() == new Vector3(0, 0, 0)
                || source != null && exported != null);

            if (source != null && exported != null)
            {
                // XYZ checks
                Assert.IsTrue(source.Xyz != null && exported.Xyz != null && source.Xyz.DoubleDeltaCompare(exported.Xyz, 0.01) || 
                    source.Xyz == null && exported.Xyz == null || 
                    source.Xyz == null && exported.Xyz.ToVector3() == new Vector3(0, 0, 0) || 
                    source.Xyz.ToVector3() == new Vector3(0, 0, 0) && exported.Xyz == null);
                //RPY checks
                Assert.IsTrue(source?.Rpy != null && exported?.Rpy != null && RpyCheck(source.Rpy, exported.Rpy, .05) || 
                    source.Rpy == null && exported.Rpy == null || 
                    source.Rpy == null && exported.Rpy.ToVector3() == new Vector3(0, 0, 0) || 
                    source.Rpy.ToVector3() == new Vector3(0, 0, 0) && exported.Rpy == null);
            }
        }

        /// <summary>
        /// Compares visual information of two links
        /// </summary>
        /// <param name="source">First link's visual information to be compared</param>
        /// <param name="exported">Second link's visual information to be compared</param>
        /// <returns></returns>
        static void CompareVisual(Link.Visual source, Link.Visual exported)
        {
            Assert.AreEqual(source.name, exported.name, $"Name of the visual is unequal: Expected: {source.name} Actual: {exported.name}");
            CompareOrigin(source.origin, exported.origin);
            CompareGeometry(source.geometry, exported.geometry);

            if (source.material != null && exported.material != null)
                CompareMaterial(source.material, exported.material);
        }

        /// <summary>
        /// Compares geometry information of two visuals
        /// </summary>
        /// <param name="source">First visual's geometry information to be compared</param>
        /// <param name="exported">Second visual's geometry information to be compared</param>
        /// <returns></returns>
        static void CompareGeometry(Link.Geometry source, Link.Geometry exported)
        {
            if (source.box != null)
            {
                Assert.IsTrue(exported != null && source.box.size.DoubleDeltaCompare(exported.box.size, .0001), "Box size is not equal");
                return;
            }

            if (source.cylinder != null)
            {
                Assert.IsTrue(exported.cylinder != null);
                Assert.IsTrue(source.cylinder.radius.EqualsDelta(exported.cylinder.radius, .001), "Cylinder radius is not equal");
                Assert.IsTrue(source.cylinder.length.EqualsDelta(exported.cylinder.length, .001), "Cylinder length is not equal");
                return;
            }

            if (source.sphere != null)
            {
                Assert.IsTrue(exported.sphere != null && source.sphere.radius.EqualsDelta(exported.sphere.radius, .0001), "Sphere radius is not equal");
                return;
            }

            if (source.mesh != null)
            {
                Assert.IsTrue(exported.mesh != null);
                Assert.IsTrue(Path.GetFileName(source.mesh.filename) == Path.GetFileName(exported.mesh.filename), "Filename is not same");
                if (source.mesh.scale != null && exported.mesh.scale != null)
                    for (var i = 0; i < source.mesh.scale.Length; i++)
                        Assert.IsTrue(source.mesh.scale[i].EqualsDelta(exported.mesh.scale[i], .0001), "Mesh scale is not equal");
                else
                    Assert.IsTrue(source.mesh.scale == null && exported.mesh.scale == null || 
                        exported.mesh.scale == null && source.mesh.scale.DoubleDeltaCompare(new double[] { 1, 1, 1 }, 0) || 
                        source.mesh.scale == null && exported.mesh.scale.DoubleDeltaCompare(new double[] { 1, 1, 1 }, 0));
            }
        }

        /// <summary>
        /// Compares material information of two visuals
        /// </summary>
        /// <param name="source">First visual's material information to be compared</param>
        /// <param name="exported">Second visual's material information to be compared</param>
        /// <returns></returns>
        static void CompareMaterial(Link.Visual.Material source, Link.Visual.Material exported)
        {
            Assert.AreEqual(source.name, exported.name, $"Name of the material is unequal: Expected: {source.name} Actual: {exported.name}");

            if (source.color != null && exported.color != null)
                for (var i = 0; i < source.color.rgba.Length; i++)
                    Assert.IsTrue(source.color.rgba[i].EqualsDelta(exported.color.rgba[i], .005), "Material not equal");
            else
                Assert.IsTrue(source.color == null && exported.color == null);

            if (source.texture != null && exported.texture != null)
                Assert.IsTrue(source.texture.filename != exported.texture.filename, "Texture names are not equal");
            else
                Assert.IsTrue(source.texture == null && source.texture == null);
        }

        /// <summary>
        /// Compares collision information of two links
        /// </summary>
        /// <param name="source">First link's collision information to be compared</param>
        /// <param name="exported">Second link's collision information to be compared</param>
        /// <returns></returns>
        static void CompareCollisions(Link.Collision source, Link.Collision exported)
        {
            Assert.AreEqual(source.name,exported.name, "Collision mesh name not equal");
            CompareOrigin(source.origin, exported.origin);
            CompareGeometry(source.geometry, exported.geometry);
        }

        /// <summary>
        /// Compares axis information of two links
        /// </summary>
        /// <param name="source">First joint's axis information to be compared</param>
        /// <param name="exported">Second joint's axis information to be compared</param>
        /// <returns></returns>
        static void CompareAxis(Joint.Axis source, Joint.Axis exported)
        {
            Assert.IsTrue(source == null && exported == null || (source == null && exported != null && exported.xyz.DoubleDeltaCompare(new double[] { 1, 0, 0 }, 0))
                || (exported == null && source != null && source.xyz.DoubleDeltaCompare(new double[] { 1, 0, 0 }, 0))
                || (source != null && exported !=null && source.xyz.DoubleDeltaCompare(exported.xyz, .001)));
        }

        /// <summary>
        /// Compares dynamics information of two links
        /// </summary>
        /// <param name="source">First link's dynamics information to be compared</param>
        /// <param name="exported">Second link's dynamics information to be compared</param>
        /// <returns></returns>
        static void CompareDynamics(Joint.Dynamics source, Joint.Dynamics exported)
        {
            Assert.IsTrue(source == null && exported == null ||
                source != null && exported != null &&
                (!double.IsNaN(source.damping) && !double.IsNaN(exported.damping) && source.damping.EqualsDelta(exported.damping, .001) || 
                    double.IsNaN(source.damping) && exported.damping == 0 || 
                    double.IsNaN(exported.damping) && source.damping == 0 || 
                    double.IsNaN(source.damping) && double.IsNaN(exported.damping)) && (!double.IsNaN(source.friction) && !double.IsNaN(exported.friction) && source.friction.EqualsDelta(exported.friction, .001) || 
                    double.IsNaN(source.friction) && exported.friction == 0 || 
                    double.IsNaN(exported.friction) && source.friction == 0 || 
                    double.IsNaN(source.friction) && double.IsNaN(exported.friction)) || 
                (source == null && exported.damping == 0 && exported.friction == 0 || 
                    exported == null && source.damping == 0 && source.friction == 0)
            );
        }

        /// <summary>
        /// Compares limit information of two joints
        /// </summary>
        /// <param name="source">First joint's limit information to be compared</param>
        /// <param name="exported">Second joint's limit information to be compared</param>
        /// <returns></returns>
        static void CompareLimit(Joint.Limit source, Joint.Limit exported)
        {
            //Lower
            Assert.IsFalse(double.IsNaN(source.lower) && exported.lower != 0 || source.lower != 0 && double.IsNaN(exported.lower), "Nullity Check failed");
            Assert.IsTrue(source.lower.EqualsDelta(exported.lower, .05), "Lower Limit is not equal");

            //Upper
            Assert.IsFalse(double.IsNaN(source.upper) && exported.upper != 0 || source.upper != 0 && double.IsNaN(exported.upper), "Nullity Check failed");
            Assert.IsTrue(source.upper.EqualsDelta(exported.upper, .05), "Lower Limit is not equal");

            Assert.IsTrue(source.effort.EqualsDelta(exported.effort, .05), "Effort is not equal");
            Assert.IsTrue(source.velocity.EqualsDelta(exported.velocity, .05), "Velocity is not equal");
        }

        /// <summary>
        /// Function to compare Roll, Pitch Yaw.
        /// It is implemented to take into account equality of angles.
        /// </summary>
        /// <param name="source">First RPY array</param>
        /// <param name="exported">Second RPY array</param>
        /// <param name="delta">Amount difference allowed in comparison</param>
        /// <returns></returns>
        static bool RpyCheck(double[] source, double[] exported, double delta)
        {
            for (var i = 0; i < 3; i++)
                if (!(source[i].EqualsDelta(exported[i], delta) || source[i] > 0 || source[i].EqualsDelta(exported[i] + 2 * Mathf.PI, delta) || source[i] <= 0 || source[i].EqualsDelta(exported[i] - 2 * Mathf.PI, delta)))
                    return false;
            return true;
        }

        [TearDown]
        public void TearDown()
        {
            var outFailedPaths = new List<string>();
            AssetDatabase.DeleteAssets(
                new[]
                {
                    "Packages/com.unity.robotics.urdf-importer/Tests/Runtime/Assets/URDF/cube/tmp",
                    "Assets/Tests/",
                    "Packages/com.unity.robotics.urdf-importer/Tests/Runtime/Assets/URDF/cube/Materials",
                    "Packages/com.unity.robotics.urdf-importer/Tests/Runtime/Assets/URDF/cube/meshes/cube_1.asset",
                    "Packages/com.unity.robotics.urdf-importer/Tests/Runtime/Assets/URDF/cube/meshes/cube_1.asset.meta"
                }, outFailedPaths);
        }
    }
}
