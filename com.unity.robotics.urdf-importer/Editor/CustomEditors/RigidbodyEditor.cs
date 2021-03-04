 

using UnityEditor;
using UnityEngine;

namespace RosSharp.Urdf.Editor
{
    [CustomEditor(typeof(Rigidbody))]
    public class RigidbodyEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            Rigidbody rigidbody = (Rigidbody)target;

            EditorGUI.BeginChangeCheck();
            Vector3 centerOfMass = EditorGUILayout.Vector3Field("Center Of Mass", rigidbody.centerOfMass);
            if (EditorGUI.EndChangeCheck())
            {
                rigidbody.centerOfMass = centerOfMass;
            }

            EditorGUI.BeginChangeCheck();
            Vector3 inertiaTensor = EditorGUILayout.Vector3Field("Inertia Tensor", rigidbody.inertiaTensor);
            if (EditorGUI.EndChangeCheck())
            {
                rigidbody.inertiaTensor = inertiaTensor;
            }

            EditorGUI.BeginChangeCheck();
            Vector3 inertiaTensorRotation = EditorGUILayout.Vector3Field("Inertia Tensor Rotation", rigidbody.inertiaTensorRotation.eulerAngles);
            if (EditorGUI.EndChangeCheck())
            {
                rigidbody.inertiaTensorRotation = Quaternion.Euler(inertiaTensorRotation);
            }
        }
    }
}
