 

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

            Rigidbody _rigidbody = (Rigidbody)target;
            //_rigidbody.centerOfMass = EditorGUILayout.Vector3Field("Center Of Mass", _rigidbody.centerOfMass);
            _rigidbody.inertiaTensor = EditorGUILayout.Vector3Field("Inertia Tensor", _rigidbody.inertiaTensor);

            Vector3 inertiaTensorEuler = _rigidbody.inertiaTensorRotation.eulerAngles;
            Vector3 newEuler = EditorGUILayout.Vector3Field("Inertia Tensor Rotation", inertiaTensorEuler);
            if (inertiaTensorEuler != newEuler)
            {
                _rigidbody.inertiaTensorRotation = Quaternion.Euler(newEuler);
            }
        }
    }
}