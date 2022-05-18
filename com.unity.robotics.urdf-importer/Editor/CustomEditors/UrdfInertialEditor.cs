/*
© Siemens AG, 2017
Author: Dr. Martin Bischoff (martin.bischoff@siemens.com)
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
using UnityEngine.Assertions;

namespace Unity.Robotics.UrdfImporter.Editor
{
    [CustomEditor(typeof(UrdfInertial))]
    public class UrdfInertialEditor : UnityEditor.Editor
    {
        private Vector3 testVector;

        bool ResetInertialsButton()
        {
            return GUILayout.Button("Reset Overrides");
        }

        public override void OnInspectorGUI()
        {
            UrdfInertial urdfInertial = (UrdfInertial) target;
            
            GUILayout.Space(5);
            var shouldOverride = 
                EditorGUILayout.BeginToggleGroup("Override URDF Data", !urdfInertial.useUrdfData);
            EditorGUI.BeginChangeCheck();
            var centerOfMass = 
                EditorGUILayout.Vector3Field("URDF Center of Mass", urdfInertial.centerOfMass);
            var inertiaTensor = 
                EditorGUILayout.Vector3Field("URDF Inertia Tensor", urdfInertial.inertiaTensor);
            var inertialChanged = EditorGUI.EndChangeCheck();
            EditorGUI.BeginChangeCheck();
            var eulerAngles = 
                EditorGUILayout.Vector3Field("URDF Inertia Tensor Rotation",
                    urdfInertial.inertiaTensorRotation.eulerAngles);
            var anglesChanged = EditorGUI.EndChangeCheck();
            
            var shouldReset = ResetInertialsButton();
            EditorGUILayout.EndToggleGroup();

            var toggleOccured = urdfInertial.useUrdfData == shouldOverride;
            
            // Leaving a bunch of asserts in here because I'm pretty sure multiple change checks can't be true at the
            // same time, but not positive...
            if (toggleOccured) 
            {
                Assert.IsFalse(inertialChanged || anglesChanged || shouldReset);
                Undo.RecordObject(urdfInertial, "Toggle URDF Overrides");
                urdfInertial.useUrdfData = !shouldOverride;
            }
            else if (inertialChanged)
            {
                Assert.IsFalse(anglesChanged || shouldReset);
                Undo.RecordObject(urdfInertial, "Change URDF Inertial Values");
                urdfInertial.centerOfMass = centerOfMass;
                urdfInertial.inertiaTensor = inertiaTensor;
            }
            else if (anglesChanged)
            {
                Assert.IsFalse(shouldReset);
                Undo.RecordObject(urdfInertial, "Change URDF Inertial tensor rotation");
                urdfInertial.inertiaTensorRotation.eulerAngles = eulerAngles;
            }
            else if (shouldReset)
            {
                Undo.RecordObject(urdfInertial, "Reset URDF Inertial Values");
                urdfInertial.ResetInertial();
                return;
            }
            
            urdfInertial.UpdateLinkData(toggleOccured, inertialChanged || anglesChanged);
        }
    }
}