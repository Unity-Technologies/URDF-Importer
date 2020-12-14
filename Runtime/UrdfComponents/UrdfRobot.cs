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

using UnityEngine;
using System;
using System.Collections.Generic;

namespace RosSharp.Urdf
{
    public enum GeometryTypes { Box, Cylinder, Sphere, Mesh }

    public class UrdfRobot : MonoBehaviour
    {
        public string FilePath;
        public ImportSettings.axisType choosenAxis ;
        private ImportSettings.axisType currentOrientation = ImportSettings.axisType.yAxis;
        public List<CollisionIgnore> collisionExceptions;
        #region Configure Robot

        public void SetCollidersConvex(bool convex)
        {
            foreach (MeshCollider meshCollider in GetComponentsInChildren<MeshCollider>())
                meshCollider.convex = convex;
        }


        public void SetUseUrdfInertiaData(bool useUrdfData)
        {
            foreach (UrdfInertial urdfInertial in GetComponentsInChildren<UrdfInertial>())
                urdfInertial.useUrdfData = useUrdfData;
        }

        public void SetRigidbodiesUseGravity(bool useGravity)
        {
#if UNITY_2020_1_OR_NEWER
            foreach (ArticulationBody ar in GetComponentsInChildren<ArticulationBody>())
                ar.useGravity = useGravity;
#else
            foreach (Rigidbody rb in GetComponentsInChildren<Rigidbody>())
                rb.useGravity = useGravity;
#endif
        }

        public void GenerateUniqueJointNames()
        {
            foreach (UrdfJoint urdfJoint in GetComponentsInChildren<UrdfJoint>())
                urdfJoint.GenerateUniqueJointName();
        }

        // Add a rotation in the model which gives the correct correspondence between UnitySpace and RosSpace
        public void ChangeToCorrectedSpace(bool rosSpace)
        {
            this.transform.Rotate(0, 180, 0);
        }

        public bool CheckOrientation()
        {
            return currentOrientation == choosenAxis;
        }

        public void SetOrientation()
        {
            currentOrientation = choosenAxis;
        }

        public void AddController(bool controller)
        {
            if (controller && this.gameObject.GetComponent< RosSharp.Control.Controller>() == null)
            {
                this.gameObject.AddComponent<RosSharp.Control.Controller>();
            }
            else
            {
                DestroyImmediate(this.gameObject.GetComponent<RosSharp.Control.Controller>());
                DestroyImmediate(this.gameObject.GetComponent<RosSharp.Control.FKRobot>());
                JointControl[] scriptList = GetComponentsInChildren<JointControl>();
                foreach (JointControl script in scriptList)
                    DestroyImmediate(script);
            }
        }

        public void AddFkRobot(bool fkRobot)
        {
            if (fkRobot && this.gameObject.GetComponent<RosSharp.Control.FKRobot>() == null)
            {
                this.gameObject.AddComponent<RosSharp.Control.FKRobot>();
            }
            else
            {
                DestroyImmediate(this.gameObject.GetComponent<RosSharp.Control.FKRobot>());
            }
        }

        public void SetAxis(ImportSettings.axisType setAxis)
        {
            this.choosenAxis = setAxis;
        }

        void Start()
        {
            CreateCollisionExceptions();
        }

        public void CreateCollisionExceptions()
        {
            if (collisionExceptions != null)
            {
                foreach (CollisionIgnore ignoreCollision in collisionExceptions)
                {
                    Collider[] collidersObject1 = ignoreCollision.Link1.GetComponentsInChildren<Collider>();
                    Collider[] collidersObject2 = ignoreCollision.Link2.GetComponentsInChildren<Collider>();
                    foreach (Collider colliderMesh1 in collidersObject1)
                    {
                        foreach (Collider colliderMesh2 in collidersObject2)
                        {
                            Physics.IgnoreCollision(colliderMesh1, colliderMesh2);
                        }
                    }
                }
            }
        }
        #endregion
    }
}
