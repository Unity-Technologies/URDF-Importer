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
            if (EditorApplication.isPlaying)
            {
                EditorGUI.BeginDisabledGroup(true);
                EditorGUILayout.Vector3Field("Center Of Mass", rigidbody.centerOfMass);
                EditorGUILayout.Vector3Field("Inertia Tensor", rigidbody.inertiaTensor);
                EditorGUILayout.Vector3Field("Inertia Tensor Rotation", rigidbody.inertiaTensorRotation.eulerAngles);
                EditorGUI.EndDisabledGroup();
            }
            else
            {
                rigidbody.centerOfMass = EditorGUILayout.Vector3Field("Center Of Mass", rigidbody.centerOfMass);
                rigidbody.inertiaTensor = EditorGUILayout.Vector3Field("Inertia Tensor", rigidbody.inertiaTensor);

                Vector3 inertiaTensorEuler = rigidbody.inertiaTensorRotation.eulerAngles;
                Vector3 newEuler = EditorGUILayout.Vector3Field("Inertia Tensor Rotation", inertiaTensorEuler);
                if (inertiaTensorEuler != newEuler)
                {
                    rigidbody.inertiaTensorRotation = Quaternion.Euler(newEuler);
                }
            }
        }
    }
}
