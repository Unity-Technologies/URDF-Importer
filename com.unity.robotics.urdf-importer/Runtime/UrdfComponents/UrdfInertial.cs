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

using System;
using UnityEngine;
using UnityEngine.Assertions;

namespace Unity.Robotics.UrdfImporter
{
    [RequireComponent(typeof(ArticulationBody))]
    public class UrdfInertial : MonoBehaviour
    {
        const int k_RoundDigits = 10;
        const float k_MinInertia = 1e-6f;
        const float k_MinMass = 0.1f;
        
        public bool useUrdfData;
        public Vector3 centerOfMass;
        public Vector3 inertiaTensor;
        public Quaternion inertiaTensorRotation;
        public Quaternion inertialAxisRotation;

        [SerializeField, HideInInspector]
        Link.Inertial m_OriginalValues;
        
        [SerializeField, HideInInspector]
        Link.Inertial m_Overrides;

        public static void Create(GameObject linkObject, Link.Inertial inertialLink = null)
        {
            var inertialUrdf = linkObject.AddComponent<UrdfInertial>();
            if (inertialLink != null)
            {
                inertialUrdf.m_OriginalValues = inertialLink;
                // Initialize overrides to URDF values to start
                inertialUrdf.m_Overrides = inertialLink;
                inertialUrdf.useUrdfData = true;
            }
            else if (inertialUrdf.TryGetComponent<ArticulationBody>(out var robotLink))
            {
                robotLink.mass = Mathf.Max(robotLink.mass, k_MinMass);

                inertialUrdf.m_Overrides = inertialUrdf.ToLinkInertial(robotLink);
                // NOTE: The first time this is set to true, we'll save the current state of m_Overrides as the default,
                //       since there is no actual URDF data to default to
                inertialUrdf.useUrdfData = false;
            }
            inertialUrdf.UpdateLinkData();
        }

        public void ResetInertial()
        {
            m_Overrides = m_OriginalValues;
            AssignUrdfInertiaData(m_Overrides);
        }

#region Runtime

        void Start()
        {
            UpdateLinkData();
        }

        public void UpdateLinkData(bool copyOverrides = false, bool manualInput = false)
        {
            var articulationBody = GetComponent<ArticulationBody>();
            if (articulationBody == null)
            {
                return;
            }
            
            if (useUrdfData)
            {
                if (m_OriginalValues == null)
                {
                    Debug.LogWarning(
                        "This instance doesn't have any urdf data stored - " +
                        "creating some using the current inertial values.");
                    m_OriginalValues = ToLinkInertial(articulationBody);
                }
                Assert.IsNotNull(m_OriginalValues);
                if (copyOverrides)
                {
                    m_Overrides = ToLinkInertial(articulationBody);
                }
                AssignUrdfInertiaData(m_OriginalValues);
                return;
            }
            
            if (copyOverrides)
            {
                m_Overrides ??= new Link.Inertial(m_OriginalValues);
            }
            // Ensure that when this script is hot-loaded for the first time that this previously non-existent variable
            // gets some sensible values (by copying them from the current state of the ArticulationBody)
            else if (m_Overrides == null)
            {
                m_Overrides = ToLinkInertial(articulationBody);
            }
            else if (manualInput)
            {
                m_Overrides = ToLinkInertial(articulationBody, false);
            }
            AssignUrdfInertiaData(m_Overrides);
        }

#endregion

#region Import

        void AssignUrdfInertiaData(Link.Inertial linkInertial)
        {
            Assert.IsNotNull(linkInertial);
            var robotLink = GetComponent<ArticulationBody>();
            robotLink.mass = (float)linkInertial.mass > 0 
                ? (float)linkInertial.mass 
                : k_MinMass;
            
            robotLink.centerOfMass = linkInertial.origin != null 
                ? UrdfOrigin.GetPositionFromUrdf(linkInertial.origin) 
                : Vector3.zero;
            
            Vector3 eigenvalues;
            Vector3[] eigenvectors;
            Matrix3x3 rotationMatrix = ToMatrix3x3(linkInertial.inertia);
            rotationMatrix.DiagonalizeRealSymmetric(out eigenvalues, out eigenvectors);

            Vector3 inertiaEulerAngles;

            if (linkInertial.origin != null)
            {
                inertiaEulerAngles = UrdfOrigin.GetRotationFromUrdf(linkInertial.origin);
            }
            else
            {
                inertiaEulerAngles = new Vector3(0, 0, 0);
            }

            this.inertialAxisRotation.eulerAngles = inertiaEulerAngles;
            

            robotLink.inertiaTensor = ToUnityInertiaTensor(FixMinInertia(eigenvalues));
            var tensorRotation = ToQuaternion(eigenvectors[0], eigenvectors[1], eigenvectors[2]).Ros2Unity() * this.inertialAxisRotation;
            if (float.IsNaN(tensorRotation.x) || float.IsNaN(tensorRotation.y) || float.IsNaN(tensorRotation.z))
                robotLink.inertiaTensorRotation = Quaternion.identity;
            else
                robotLink.inertiaTensorRotation = tensorRotation;

            this.centerOfMass = robotLink.centerOfMass;
            this.inertiaTensor = robotLink.inertiaTensor;
            this.inertiaTensorRotation = robotLink.inertiaTensorRotation;

        }

        private static Vector3 ToUnityInertiaTensor(Vector3 vector3)
        {
            return new Vector3(vector3.y, vector3.z, vector3.x);
        }

        private static Matrix3x3 ToMatrix3x3(Link.Inertial.Inertia inertia)
        {
            return new Matrix3x3(
                new[] { (float)inertia.ixx, (float)inertia.ixy, (float)inertia.ixz,
                                             (float)inertia.iyy, (float)inertia.iyz,
                                                                 (float)inertia.izz });
        }

        static Vector3 FixMinInertia(Vector3 vector3)
        {
            for (var i = 0; i < 3; i++)
            {
                if (vector3[i] < k_MinInertia)
                    vector3[i] = k_MinInertia;
            }
            return vector3;
        }

        private static Quaternion ToQuaternion(Vector3 eigenvector0, Vector3 eigenvector1, Vector3 eigenvector2)
        {
            //From http://www.euclideanspace.com/maths/geometry/rotations/conversions/matrixToQuaternion/
            float tr = eigenvector0[0] + eigenvector1[1] + eigenvector2[2];
            float qw, qx, qy, qz;
            if (tr > 0)
            {
                float s = Mathf.Sqrt(tr + 1.0f) * 2f; // S=4*qw 
                qw = 0.25f * s;
                qx = (eigenvector1[2] - eigenvector2[1]) / s;
                qy = (eigenvector2[0] - eigenvector0[2]) / s;
                qz = (eigenvector0[1] - eigenvector1[0]) / s;
            }
            else if ((eigenvector0[0] > eigenvector1[1]) & (eigenvector0[0] > eigenvector2[2]))
            {
                float s = Mathf.Sqrt(1.0f + eigenvector0[0] - eigenvector1[1] - eigenvector2[2]) * 2; // S=4*qx 
                qw = (eigenvector1[2] - eigenvector2[1]) / s;
                qx = 0.25f * s;
                qy = (eigenvector1[0] + eigenvector0[1]) / s;
                qz = (eigenvector2[0] + eigenvector0[2]) / s;
            }
            else if (eigenvector1[1] > eigenvector2[2])
            {
                float s = Mathf.Sqrt(1.0f + eigenvector1[1] - eigenvector0[0] - eigenvector2[2]) * 2; // S=4*qy
                qw = (eigenvector2[0] - eigenvector0[2]) / s;
                qx = (eigenvector1[0] + eigenvector0[1]) / s;
                qy = 0.25f * s;
                qz = (eigenvector2[1] + eigenvector1[2]) / s;
            }
            else
            {
                float s = Mathf.Sqrt(1.0f + eigenvector2[2] - eigenvector0[0] - eigenvector1[1]) * 2; // S=4*qz
                qw = (eigenvector0[1] - eigenvector1[0]) / s;
                qx = (eigenvector2[0] + eigenvector0[2]) / s;
                qy = (eigenvector2[1] + eigenvector1[2]) / s;
                qz = 0.25f * s;
            }
            return new Quaternion(qx, qy, qz, qw);
        }

#endregion

#region Export
        public Link.Inertial ExportInertialData()
        {
            var robotLink = GetComponent<ArticulationBody>();

            if (robotLink == null)
            {
                Debug.LogWarning("No data to export.");
                return null;
            }

            UpdateLinkData();
            return ToLinkInertial(robotLink);
        }

        Link.Inertial ToLinkInertial(ArticulationBody robotLink, bool fromArticulation = true)
        {
            var originAngles = inertialAxisRotation.eulerAngles;
            var com = fromArticulation ? robotLink.centerOfMass : centerOfMass;
            var inertialOrigin = new Origin(
                com.Unity2Ros().ToRoundedDoubleArray(), 
                new double[] { originAngles.x, originAngles.y, originAngles.z });
            var inertia = ExportInertiaData(fromArticulation);

            return new Link.Inertial(Math.Round(robotLink.mass, k_RoundDigits), inertialOrigin, inertia);
        }

        private Link.Inertial.Inertia ExportInertiaData(bool fromArticulation = true)
        {
            var robotLink = GetComponent<ArticulationBody>();
            var lambdaMatrix = fromArticulation ? new Matrix3x3(new[]
            {
                robotLink.inertiaTensor[0], robotLink.inertiaTensor[1], robotLink.inertiaTensor[2]
            }) : new Matrix3x3(new[]
            {
                inertiaTensor[0], inertiaTensor[1], inertiaTensor[2]
            });

            var qMatrix = fromArticulation
                ? Quaternion2Matrix(robotLink.inertiaTensorRotation * Quaternion.Inverse(inertialAxisRotation))
                : Quaternion2Matrix(inertiaTensorRotation * Quaternion.Inverse(inertialAxisRotation));
            var qMatrixTransposed = qMatrix.Transpose();
            var inertiaMatrix = qMatrix * lambdaMatrix * qMatrixTransposed;

            return ToRosCoordinates(ToInertia(inertiaMatrix));
        }



        private static Matrix3x3 Quaternion2Matrix(Quaternion quaternion)
        {
            Quaternion rosQuaternion = Quaternion.Normalize(quaternion);
            float qx = rosQuaternion.x;
            float qy = rosQuaternion.y;
            float qz = rosQuaternion.z;
            float qw = rosQuaternion.w;

            //From http://www.euclideanspace.com/maths/geometry/rotations/conversions/quaternionToMatrix/index.htm
            return new Matrix3x3(new float[] {
                1 - (2 * qy * qy) - (2 * qz * qz),
                (2 * qx * qy) - (2 * qz * qw),
                (2 * qx * qz) + (2 * qy * qw),

                (2 * qx * qy) + (2 * qz * qw),
                1 - (2 * qx * qx) - (2 * qz * qz),
                (2 * qy * qz) - (2 * qx * qw),

                (2 * qx * qz) - (2 * qy * qw),
                (2 * qy * qz) + (2 * qx * qw),
                1 - (2 * qx * qx) - (2 * qy * qy)});
        }

        private static Link.Inertial.Inertia ToInertia(Matrix3x3 matrix)
        {
            return new Link.Inertial.Inertia(matrix[0][0], matrix[0][1], matrix[0][2],
                matrix[1][1], matrix[1][2],
                matrix[2][2]);
        }

        private static Link.Inertial.Inertia ToRosCoordinates(Link.Inertial.Inertia unityInertia)
        {
            return new Link.Inertial.Inertia(0, 0, 0, 0, 0, 0)
            {
                ixx = unityInertia.izz,
                iyy = unityInertia.ixx,
                izz = unityInertia.iyy,

                ixy = -unityInertia.ixz,
                ixz = unityInertia.iyz,
                iyz = -unityInertia.ixy
            };
        }
#endregion
    }
}

