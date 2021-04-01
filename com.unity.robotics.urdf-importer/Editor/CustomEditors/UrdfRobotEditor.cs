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

using System.IO;
using UnityEditor;
using UnityEngine;

namespace RosSharp.Urdf.Editor
{
    [CustomEditor(typeof(UrdfRobot))]
    public class UrdfRobotEditor : UnityEditor.Editor
    {
        private UrdfRobot urdfRobot;
        private static GUIStyle buttonStyle;
        private string exportRoot = "";
        SerializedProperty axisType;

        public void OnEnable()
        {
            axisType = serializedObject.FindProperty("choosenAxis");
        }
        public override void OnInspectorGUI()
        {
            if (buttonStyle == null)
                buttonStyle = new GUIStyle(EditorStyles.miniButtonRight) { fixedWidth = 75 };

            urdfRobot = (UrdfRobot) target;

            EditorGUILayout.PropertyField(axisType, new GUIContent("Axis Type"));
            serializedObject.ApplyModifiedProperties();
            UrdfRobotExtensions.CorrectAxis(urdfRobot.gameObject);

            GUILayout.Space(5);
            GUILayout.Label("All Rigidbodies", EditorStyles.boldLabel);
            DisplaySettingsToggle(new GUIContent("Use Gravity", "If disabled, robot is not affected by gravity."), urdfRobot.SetRigidbodiesUseGravity, UrdfRobot.useGravity);
            DisplaySettingsToggle(new GUIContent("Use Inertia from URDF", "If disabled, Unity will generate new inertia tensor values automatically."),urdfRobot.SetUseUrdfInertiaData,
                UrdfRobot.useUrdfInertiaData);
            DisplaySettingsToggle(new GUIContent("Default Space"), urdfRobot.ChangeToCorrectedSpace,UrdfRobot.changetoCorrectedSpace);

            GUILayout.Space(5);
            GUILayout.Label("All Colliders", EditorStyles.boldLabel);
            DisplaySettingsToggle(new GUIContent("Convex"), urdfRobot.SetCollidersConvex,UrdfRobot.collidersConvex);

            GUILayout.Space(5);
            GUILayout.Label("All Joints", EditorStyles.boldLabel);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel("Generate Unique Joint Names");
            if (GUILayout.Button("Generate", new GUIStyle (EditorStyles.miniButton) {fixedWidth = 155}))
                urdfRobot.GenerateUniqueJointNames();
            EditorGUILayout.EndHorizontal();

            GUILayout.Space(5);
            EditorGUILayout.PropertyField(axisType, new GUIContent("Axis Type", "Adjust this if the models that make up your robot are facing the wrong direction."));
            serializedObject.ApplyModifiedProperties();
            UrdfRobotExtensions.CorrectAxis(urdfRobot.gameObject);

            if (urdfRobot.GetComponent<RosSharp.Control.Controller>() == null || urdfRobot.GetComponent<RosSharp.Control.FKRobot>() == null)
            {
                GUILayout.Label("Components", EditorStyles.boldLabel);
                GUILayout.BeginHorizontal();
                if (GUILayout.Button(urdfRobot.GetComponent<RosSharp.Control.Controller>() == null? "Add Controller": "Remove Controller"))
                {
                    urdfRobot.AddController();
                }
                if (urdfRobot.GetComponent<RosSharp.Control.FKRobot>() == null)
                {
                    if (GUILayout.Button("Add Forward Kinematics"))
                    {
                        urdfRobot.gameObject.AddComponent<RosSharp.Control.FKRobot>();
                    }
                }
                GUILayout.EndHorizontal();
            }

            GUILayout.Space(5);
            GUILayout.Label("URDF Files", EditorStyles.boldLabel);
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Export robot to URDF"))
            {
                exportRoot = EditorUtility.OpenFolderPanel("Select export directory", exportRoot, "");

                if (exportRoot.Length == 0)
                    return;
                else if (!Directory.Exists(exportRoot))
                    EditorUtility.DisplayDialog("Export Error", "Export root folder must be defined and folder must exist.", "Ok");
                else
                {
                    urdfRobot.ExportRobotToUrdf(exportRoot);
                    SetEditorPrefs();
                }
            }

            GUILayout.Space(5);
            if (GUILayout.Button("Compare URDF Files"))
            {
                CompareURDF window = (CompareURDF)EditorWindow.GetWindow(typeof(CompareURDF));
                window.minSize = new Vector2(500, 200);
                window.GetEditorPrefs();
                window.Show();
            }
            GUILayout.EndHorizontal();
        }

        private delegate void SettingsHandler();

        private static void DisplaySettingsToggle(GUIContent label, SettingsHandler handler, bool currentState)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel(label);
            string buttonName = currentState ? "Disable" : "Enable";
            if (GUILayout.Button(buttonName, buttonStyle))
            {
                handler();
            }
            EditorGUILayout.EndHorizontal();
        }

        private void SetEditorPrefs()
        {
            EditorPrefs.SetString("UrdfExportRoot", exportRoot);
        }

    }
}
