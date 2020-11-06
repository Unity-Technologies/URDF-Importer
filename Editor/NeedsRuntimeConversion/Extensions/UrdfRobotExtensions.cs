 

using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using RosSharp;

namespace RosSharp.Urdf.Editor
{
    public static class UrdfRobotExtensions
    {
        static string tagName = "robot";
        public static ImportSettings importsettings;

        public static void Create()
        {
            CreateTag();
            GameObject robotGameObject = new GameObject("Robot");
            
            robotGameObject.tag = tagName;
            robotGameObject.AddComponent<UrdfRobot>();
            robotGameObject.AddComponent<RosSharp.Control.Controller>();

            UrdfPlugins.Create(robotGameObject.transform);

            UrdfLink urdfLink = UrdfLinkExtensions.Create(robotGameObject.transform);
            urdfLink.name = "base_link";
            urdfLink.IsBaseLink = true;
        }

        #region Import

        public static void Create(string filename, ImportSettings settings)
        {
            CreateTag();
            importsettings = settings;
            Robot robot = new Robot(filename);

            if (!UrdfAssetPathHandler.IsValidAssetPath(robot.filename))
            {
                Debug.LogError("URDF file and ressources must be placed in Assets Folder:\n" + Application.dataPath);
                return;
            }

            GameObject robotGameObject = new GameObject(robot.name);
            robotGameObject.tag = tagName;

            robotGameObject.AddComponent<UrdfRobot>();


            robotGameObject.AddComponent<RosSharp.Control.Controller>();

            robotGameObject.GetComponent<UrdfRobot>().SetAxis(settings.choosenAxis);

            UrdfAssetPathHandler.SetPackageRoot(Path.GetDirectoryName(robot.filename));
            UrdfMaterial.InitializeRobotMaterials(robot);
            UrdfPlugins.Create(robotGameObject.transform, robot.plugins);

            UrdfLinkExtensions.Create(robotGameObject.transform, robot.root);

            GameObjectUtility.SetParentAndAlign(robotGameObject, Selection.activeObject as GameObject);
            Undo.RegisterCreatedObjectUndo(robotGameObject, "Create " + robotGameObject.name);
            Selection.activeObject = robotGameObject;

            CorrectAxis(robotGameObject, settings.choosenAxis);
        }

        public static void CorrectAxis(GameObject robot, ImportSettings.axisType axis = ImportSettings.axisType.yAxis)
        {
            UrdfVisual[] visualMeshList = robot.GetComponentsInChildren<UrdfVisual>();
            UrdfCollision[] collisionMeshList = robot.GetComponentsInChildren<UrdfCollision>();
            UrdfRobot robotScript = robot.GetComponent<UrdfRobot>();

            robotScript.choosenAxis = axis;
            Quaternion correctZtoY = Quaternion.Euler(-90, 0, 90);
            Quaternion correction = Quaternion.identity;

            if (axis == ImportSettings.axisType.zAxis)
                correction = correctZtoY;


            foreach (UrdfVisual visual in visualMeshList)
            {
                visual.transform.localRotation = correction;
            }

            foreach (UrdfCollision collision in collisionMeshList)
            {
                collision.transform.localRotation = correction;
            }

        }
        #endregion

        #region Export

        public static void ExportRobotToUrdf(this UrdfRobot urdfRobot, string exportRootFolder, string exportDestination)
        {
            UrdfExportPathHandler.SetExportPath(exportRootFolder, exportDestination);

            urdfRobot.FilePath = Path.Combine(UrdfExportPathHandler.GetExportDestination(), urdfRobot.name + ".urdf");
    
            Robot robot = urdfRobot.ExportRobotData();
            if (robot == null) return;

            robot.WriteToUrdf();

            Debug.Log(robot.name + " was exported to " + UrdfExportPathHandler.GetExportDestination());

            UrdfMaterial.Materials.Clear();
            UrdfExportPathHandler.Clear();
            AssetDatabase.Refresh();
        }

        private static Robot ExportRobotData(this UrdfRobot urdfRobot)
        {
            Robot robot = new Robot(urdfRobot.FilePath, urdfRobot.gameObject.name);

            List<string> linkNames = new List<string>();

            foreach (UrdfLink urdfLink in urdfRobot.GetComponentsInChildren<UrdfLink>())
            {
                //Link export
                if (linkNames.Contains(urdfLink.name))
                {
                    EditorUtility.DisplayDialog("URDF Export Error",
                        "URDF export failed. There are several links with the name " +
                        urdfLink.name + ". Make sure all link names are unique before exporting this robot.",
                        "Ok");
                    return null;
                }
                robot.links.Add(urdfLink.ExportLinkData());
                linkNames.Add(urdfLink.name);

                //Joint export
                UrdfJoint urdfJoint = urdfLink.gameObject.GetComponent<UrdfJoint>();
                if (urdfJoint != null)
                    robot.joints.Add(urdfJoint.ExportJointData());
                else if (!urdfLink.IsBaseLink) 
                    //Make sure that links with no rigidbodies are still connected to the robot by a default joint
                    robot.joints.Add(UrdfJoint.ExportDefaultJoint(urdfLink.transform));
            }

            robot.materials = UrdfMaterial.Materials.Values.ToList();
            robot.plugins = urdfRobot.GetComponentInChildren<UrdfPlugins>().ExportPluginsData();

            return robot;
        }

        #endregion

        public static void CreateTag()
        {
            // Open tag manager
            SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
            SerializedProperty tagsProp = tagManager.FindProperty("tags");


            // First check if it is not already present
            bool found = false;
            for (int i = 0; i < tagsProp.arraySize; i++)
            {
                SerializedProperty t = tagsProp.GetArrayElementAtIndex(i);
                if (t.stringValue.Equals(tagName)) { found = true; break; }
            }

            // if not found, add it
            if (!found)
            {
                tagsProp.InsertArrayElementAtIndex(0);
                SerializedProperty n = tagsProp.GetArrayElementAtIndex(0);
                n.stringValue = tagName;
            }

            tagManager.ApplyModifiedProperties();
        }
    }
}
