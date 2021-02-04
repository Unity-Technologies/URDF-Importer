/*
© Siemens AG, 2018
Author: Suzannah Smith (suzannah.smith@siemens.com)
Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at
<http://www.apache.org/licenses/LICENSE-2.0>.
Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/

using System.Collections.Generic;
using System;
using System.Collections;
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
        static string collisionObjectName = "Collisions";
        public static ImportSettings importsettings;

        public static void Create()
        {
           CreateTag();
           GameObject robotGameObject = new GameObject("Robot");
            
           robotGameObject.tag = tagName;
           robotGameObject.AddComponent<UrdfRobot>();
           robotGameObject.AddComponent<RosSharp.Control.Controller>();

           UrdfPlugins.Create(robotGameObject.transform);

           UrdfLink urdfLink = UrdfLinkExtensions.Create(robotGameObject.transform).GetComponent<UrdfLink>();
           urdfLink.name = "base_link";
           urdfLink.IsBaseLink = true;
        }

        #region Import

        public static IEnumerator Create(string filename, ImportSettings settings, bool loadStatus = false)
        {
            CreateTag();
            importsettings = settings;
            Robot robot = new Robot(filename);

            settings.totalLinks = robot.links.Count;

            if (!UrdfAssetPathHandler.IsValidAssetPath(robot.filename))
            {
                Debug.LogError("URDF file and ressources must be placed in Assets Folder:\n" + Application.dataPath);
                yield break;
            }

            GameObject robotGameObject = new GameObject(robot.name);
            robotGameObject.tag = tagName;

            robotGameObject.AddComponent<UrdfRobot>();


            robotGameObject.AddComponent<RosSharp.Control.Controller>();

            robotGameObject.GetComponent<UrdfRobot>().SetAxis(settings.choosenAxis);

            UrdfAssetPathHandler.SetPackageRoot(Path.GetDirectoryName(robot.filename));
            UrdfMaterial.InitializeRobotMaterials(robot);
            UrdfPlugins.Create(robotGameObject.transform, robot.plugins);

            Stack<Tuple<Link,Transform,Joint>> importStack = new Stack<Tuple<Link, Transform,Joint>>();
            importStack.Push(new Tuple<Link, Transform,Joint>(robot.root, robotGameObject.transform,null));
            while(importStack.Count != 0)
            {
                Tuple<Link, Transform,Joint> currentLink = importStack.Pop();
                GameObject importedLink = UrdfLinkExtensions.Create(currentLink.Item2, currentLink.Item1, currentLink.Item3);
                settings.linksLoaded++;
                foreach (Joint childJoint in currentLink.Item1.joints)
                {
                    Link child = childJoint.ChildLink;
                    importStack.Push(new Tuple<Link, Transform, Joint>(child, importedLink.transform, childJoint));
                }
                
                if(loadStatus)
                    yield return null;
            }

            GameObjectUtility.SetParentAndAlign(robotGameObject, Selection.activeObject as GameObject);
            Undo.RegisterCreatedObjectUndo(robotGameObject, "Create " + robotGameObject.name);
            Selection.activeObject = robotGameObject;

            CorrectAxis(robotGameObject);
            CreateCollisionExceptions(robot, robotGameObject);
        }

        public static void CorrectAxis(GameObject robot)
        {
            UrdfRobot robotScript = robot.GetComponent<UrdfRobot>();
            if (robotScript.CheckOrientation())
                return;
            Quaternion correctYtoZ = Quaternion.Euler(-90, 0, 90);
            Quaternion correctZtoY = Quaternion.Inverse((correctYtoZ));
            Quaternion correction = new Quaternion();

            if (robotScript.choosenAxis == ImportSettings.axisType.zAxis)
                correction = correctYtoZ;
            else
                correction = correctZtoY;

            UrdfVisual[] visualMeshList = robot.GetComponentsInChildren<UrdfVisual>();
            UrdfCollision[] collisionMeshList = robot.GetComponentsInChildren<UrdfCollision>();
            foreach (UrdfVisual visual in visualMeshList)
            {
                visual.transform.localRotation = visual.transform.localRotation * correction;
            }

            foreach (UrdfCollision collision in collisionMeshList)
            {
                collision.transform.localRotation = collision.transform.localRotation * correction;
            }
            robotScript.SetOrientation();
        }

        private static void CreateCollisionExceptions(Robot robot, GameObject robotGameObject)
        {
            List<CollisionIgnore> ColliisonList = new List<CollisionIgnore>();
            if (robot.ignoreCollisionPair.Count > 0)
            {
                foreach(System.Tuple<string,string> ignoreCollision in robot.ignoreCollisionPair)
                {
                    Transform colliisonObject1 = GameObject.Find(ignoreCollision.Item1).transform.Find(collisionObjectName);
                    Transform collisionObject2 = GameObject.Find(ignoreCollision.Item2).transform.Find(collisionObjectName);

                    ColliisonList.Add(new CollisionIgnore(colliisonObject1, collisionObject2));
                }
            }
            robotGameObject.GetComponent<UrdfRobot>().collisionExceptions = ColliisonList;
        }
        
        #endregion

        #region Export

        public static void ExportRobotToUrdf(this UrdfRobot urdfRobot, string exportRootFolder, string exportDestination="")
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
