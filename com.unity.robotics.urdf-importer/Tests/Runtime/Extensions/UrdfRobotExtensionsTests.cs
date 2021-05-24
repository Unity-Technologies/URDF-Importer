using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Object = UnityEngine.Object;
using Controller = RosSharp.Control.Controller;

namespace RosSharp.Urdf.Tests
{
    public class UrdfRobotExtensionsTests
    {
        [Test]
        public void Create_NoParameters_Success()
        {
            UrdfRobotExtensions.Create();
            var robot = GameObject.Find("Robot");
            Assert.IsNotNull(robot);
            Assert.IsNotNull(robot.GetComponent<UrdfRobot>());
            Assert.IsNotNull(robot.GetComponent<Controller>());
            Assert.IsTrue(robot.GetComponentsInChildren<UrdfLink>().Length > 0);
            Assert.AreEqual("base_link", robot.GetComponentInChildren<UrdfLink>().name);

            Object.DestroyImmediate(robot);
        }

        [UnityTest]
        public IEnumerator Create_FromFileDefaultSettings_Success()
        {
            var urdfFile = $"{Application.dataPath.Substring(0, Application.dataPath.Length - "Assets".Length)}Packages/com.unity.robotics.urdf-importer/Tests/Runtime/Assets/URDF/cube/cube.urdf";
            yield return UrdfRobotExtensions.Create(urdfFile, ImportSettings.DefaultSettings());
        }

        // [Test]
        // public void CreateRuntime_FromCubeUrdfDefaultSettings_Success()
        // {

        // }

        // [Test]
        // public void CorrectAxis_()
        // {

        // }

        // [Test]
        // public void ExportRobotToUrdf()
        // {

        // }

        // [Test]
        // public void CreateTag()
        // {

        // }
    }
}
