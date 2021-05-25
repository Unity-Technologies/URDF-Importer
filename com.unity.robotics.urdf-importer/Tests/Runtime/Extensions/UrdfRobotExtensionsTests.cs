using System.IO;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;
using Object = UnityEngine.Object;
using Controller = RosSharp.Control.Controller;

namespace RosSharp.Urdf.Tests
{
    public class UrdfRobotExtensionsTests
    {
        string assetRoot;
        string urdfFile;

        [SetUp]
        public void SetUp()
        {
            assetRoot = "Assets/Tests/Runtime/UrdfRobotExtensionsTests";
            urdfFile = $"{Application.dataPath.Substring(0, Application.dataPath.Length - "Assets".Length)}Packages/com.unity.robotics.urdf-importer/Tests/Runtime/Assets/URDF/cube/cube.urdf";

            Directory.CreateDirectory(assetRoot);
            RuntimeURDF.runtimeModeEnabled = false;
        }

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
        public IEnumerator CreateCoroutine_FromFileDefaultSettings_Success()
        {
            var coroutineCreate = UrdfRobotExtensions.Create(urdfFile, ImportSettings.DefaultSettings(), forceRuntimeMode: true);
            yield return coroutineCreate;

            var robotGO = coroutineCreate.Current;
            Assert.IsNotNull(robotGO);
            Assert.IsNotNull(robotGO.GetComponent<UrdfRobot>());
            Assert.IsNotNull(robotGO.GetComponent<Controller>());
            Assert.AreEqual("Plugins", robotGO.transform.GetChild(0).name);
            Assert.IsTrue(robotGO.GetComponentsInChildren<UrdfVisual>().Length > 0);
            Assert.IsTrue(robotGO.GetComponentsInChildren<UrdfCollision>().Length > 0);

            coroutineCreate.Dispose();
            Object.DestroyImmediate(robotGO);
        }

        [Test]
        public void CreateRuntime_FromCubeUrdfDefaultSettings_Success()
        {
            var robot = UrdfRobotExtensions.CreateRuntime(urdfFile, ImportSettings.DefaultSettings());

            Assert.IsNotNull(robot);
            Assert.IsNotNull(robot.GetComponent<UrdfRobot>());
            Assert.IsNotNull(robot.GetComponent<Controller>());
            Assert.AreEqual("Plugins", robot.transform.GetChild(0).name);
            Assert.IsTrue(robot.GetComponentsInChildren<UrdfVisual>().Length > 0);
            Assert.IsTrue(robot.GetComponentsInChildren<UrdfCollision>().Length > 0);

            Object.DestroyImmediate(robot);
        }

        [Test]
        public void CorrectAxis_CubeUrdf_Success()
        {
            var robot = UrdfRobotExtensions.CreateRuntime($"{Application.dataPath.Substring(0, Application.dataPath.Length - "Assets".Length)}Packages/com.unity.robotics.urdf-importer/Tests/Runtime/Assets/URDF/cube/cube.urdf", ImportSettings.DefaultSettings());
            Assert.AreEqual(ImportSettings.axisType.yAxis, robot.GetComponent<UrdfRobot>().chosenAxis);
            UrdfRobotExtensions.CorrectAxis(robot);
            Assert.IsTrue(Vector3.zero == robot.GetComponentInChildren<UrdfVisual>().transform.localEulerAngles);
            Assert.IsTrue(Quaternion.Euler(0, 0, 0) == robot.GetComponentInChildren<UrdfCollision>().transform.localRotation);

            robot.GetComponent<UrdfRobot>().chosenAxis = ImportSettings.axisType.zAxis;
            UrdfRobotExtensions.CorrectAxis(robot);
            Assert.IsTrue(Quaternion.Euler(-90, 0, 90) == robot.GetComponentInChildren<UrdfVisual>().transform.localRotation);
            Assert.IsTrue(Vector3.zero == robot.GetComponentInChildren<UrdfCollision>().transform.localEulerAngles);

            Object.DestroyImmediate(robot);
        }

#if UNITY_EDITOR
        [Test]
        [UnityPlatform(RuntimePlatform.OSXEditor, RuntimePlatform.WindowsEditor, RuntimePlatform.LinuxEditor)]
        public void ExportRobotToUrdf_NoParameters_Success()
        {
            UrdfRobotExtensions.Create();
            var emptyRobot = GameObject.Find("Robot");
            var robot = emptyRobot.GetComponent<UrdfRobot>();
            robot.ExportRobotToUrdf(assetRoot);

            Assert.IsTrue(Directory.Exists($"{assetRoot}/meshes"));
            Assert.IsTrue(Directory.Exists($"{assetRoot}/Resources"));
            Assert.IsTrue(File.Exists($"{assetRoot}/Robot.urdf"));

            Object.DestroyImmediate(emptyRobot);
        }
#endif

        [Test]
        public void CreateTag_RobotTag_Success()
        {
            // Open tag manager
            SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
            SerializedProperty tagsProp = tagManager.FindProperty("tags");

            bool found = false;
            for (int i = 0; i < tagsProp.arraySize; i++)
            {
                SerializedProperty t = tagsProp.GetArrayElementAtIndex(i);
                if (t.stringValue.Equals("robot"))
                {
                    found = true; 
                    break; 
                }
            }

            Assert.IsTrue(found);
        }

        [TearDown]
        public void TearDown()
        {
            List<string> outFailedPaths = new List<string>();
            AssetDatabase.DeleteAssets(new string[] {"Assets/Tests", "Packages/com.unity.robotics.urdf-importer/Tests/Runtime/Assets/URDF/cube/Materials", "Packages/com.unity.robotics.urdf-importer/Tests/Runtime/Assets/URDF/cube/meshes/cube_1.asset", "Packages/com.unity.robotics.urdf-importer/Tests/Runtime/Assets/URDF/cube/meshes/cube_1.asset.meta"}, outFailedPaths);
        }
    }
}
