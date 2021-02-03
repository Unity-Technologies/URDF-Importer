using System.IO;
using System;
using UnityEditor;
using UnityEngine;

namespace RosSharp.Urdf.Editor
{
    public class FileImportMenu : EditorWindow
    {
        public string urdfFile;
        public ImportSettings settings = new ImportSettings();
        private static string[] windowOptions = { };

        private void Awake()
        {
            this.titleContent = new GUIContent("URDF Import Settings");
        }

        private void OnGUI()
        {
            //Styles definitions

            GUIStyle titleStyle = new GUIStyle(EditorStyles.boldLabel)
            {
                alignment = TextAnchor.MiddleLeft,
                fontSize = 13
            };

            GUIStyle buttonStyle = new GUIStyle(EditorStyles.miniButtonRight) { fixedWidth = 75 };

            //Window title
            GUILayout.Space(10);
            GUILayout.Label("Select Axis Type", titleStyle);

            //Select the original up axis of the imported mesh
            GUILayout.Space(5);
            EditorGUILayout.BeginHorizontal();
            settings.choosenAxis = (ImportSettings.axisType)EditorGUILayout.EnumPopup(
                "Select Axis Type" , settings.choosenAxis);
            EditorGUILayout.EndHorizontal();

            //Window title
            GUILayout.Space(10);
            GUILayout.Label("Select Convex Decomposer", titleStyle);

            //Select the mesh decomposer
            GUILayout.Space(5);
            EditorGUILayout.BeginHorizontal();
            settings.convexMethod = (ImportSettings.convexDecomposer)EditorGUILayout.EnumPopup(
                "Mesh Decomposer", settings.convexMethod);
            EditorGUILayout.EndHorizontal();

            //Import Robot button
            GUILayout.Space(10);
            if (GUILayout.Button("Import URDF"))
            {
                if (urdfFile != "")
                    UrdfRobotExtensions.Create(urdfFile,settings);
                Close();
            }

        }

    }
}