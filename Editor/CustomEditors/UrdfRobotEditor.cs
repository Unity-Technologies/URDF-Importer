﻿/*
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

using UnityEditor;
using UnityEngine;

namespace RosSharp.Urdf.Editor
{
    [CustomEditor(typeof(UrdfRobot))]
    public class UrdfRobotEditor : UnityEditor.Editor
    {
        private UrdfRobot urdfRobot;
        private static GUIStyle buttonStyle;
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

            GUILayout.Space(5);
            GUILayout.Label("All Rigidbodies", EditorStyles.boldLabel);
            DisplaySettingsToggle(new GUIContent("Use Gravity"), urdfRobot.SetRigidbodiesUseGravity);
            DisplaySettingsToggle(new GUIContent("Use Inertia from URDF", "If disabled, Unity will generate new inertia tensor values automatically."),
                urdfRobot.SetUseUrdfInertiaData);
            DisplaySettingsToggle(new GUIContent("Default Space"), urdfRobot.ChangeToCorrectedSpace);

            GUILayout.Space(5);
            GUILayout.Label("All Colliders", EditorStyles.boldLabel);
            DisplaySettingsToggle(new GUIContent("Convex"), urdfRobot.SetCollidersConvex);

            GUILayout.Space(5);
            GUILayout.Label("All Joints", EditorStyles.boldLabel);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel("Generate Unique Joint Names");
            if (GUILayout.Button("Generate", new GUIStyle (EditorStyles.miniButton) {fixedWidth = 155}))
                urdfRobot.GenerateUniqueJointNames();
            EditorGUILayout.EndHorizontal();

            GUILayout.Space(5);
            EditorGUILayout.PropertyField(axisType, new GUIContent("Axis Type"));
            serializedObject.ApplyModifiedProperties();
            UrdfRobotExtensions.CorrectAxis(urdfRobot.gameObject);
            GUILayout.Label("Helper Scripts", EditorStyles.boldLabel);
            DisplaySettingsToggle(new GUIContent("Controller Script"), urdfRobot.AddController);
            DisplaySettingsToggle(new GUIContent("Forward Kinematics Script"), urdfRobot.AddFkRobot);

            GUILayout.Space(5);
            if (GUILayout.Button("Export robot to URDF file"))
            {
                // Get existing open window or if none, make a new one:
                UrdfExportEditorWindow window = (UrdfExportEditorWindow)EditorWindow.GetWindow(typeof(UrdfExportEditorWindow));
                window.urdfRobot = urdfRobot;
                window.minSize = new Vector2(500, 200);
                window.GetEditorPrefs();
                window.Show();
            }

            GUILayout.Space(5);
            if(GUILayout.Button("Compare URDF Files"))
            {
                CompareURDF window = (CompareURDF)EditorWindow.GetWindow(typeof(CompareURDF));
                window.minSize = new Vector2(500, 200);
                window.GetEditorPrefs();
                window.Show();
            }
        }

        private delegate void SettingsHandler(bool enable);

        private static void DisplaySettingsToggle(GUIContent label, SettingsHandler handler)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel(label);
            if (GUILayout.Button("Enable", buttonStyle))
                handler(true);
            if (GUILayout.Button("Disable", buttonStyle))
                handler(false);
            EditorGUILayout.EndHorizontal();
        }

    }
}
