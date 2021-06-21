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

namespace Unity.Robotics.UrdfImporter
{
    public enum GeometryTypes { Box, Cylinder, Sphere, Mesh }

    public class UrdfRobot : MonoBehaviour
    {
        public string FilePath;
        public ImportSettings.axisType chosenAxis ;
        [SerializeField]
        private ImportSettings.axisType currentOrientation = ImportSettings.axisType.yAxis;
        public List<CollisionIgnore> collisionExceptions;

        //Current Settings
        public static bool collidersConvex = true;
        public static bool useUrdfInertiaData = false;
        public static bool useGravity = true;
        public static bool addController = true;
        public static bool addFkRobot = true;
        public static bool changetoCorrectedSpace = false;

        #region Configure Robot

        public void SetCollidersConvex()
        {
            foreach (MeshCollider meshCollider in GetComponentsInChildren<MeshCollider>())
                meshCollider.convex = !collidersConvex;
            collidersConvex = !collidersConvex;
        }


        public void SetUseUrdfInertiaData()
        {
            foreach (UrdfInertial urdfInertial in GetComponentsInChildren<UrdfInertial>())
                urdfInertial.useUrdfData = !useUrdfInertiaData;
            useUrdfInertiaData = !useUrdfInertiaData;
        }

        public void SetRigidbodiesUseGravity()
        {
            foreach (ArticulationBody ar in GetComponentsInChildren<ArticulationBody>())
                ar.useGravity = !useGravity;
            useGravity = !useGravity;

        }

        public void GenerateUniqueJointNames()
        {
            foreach (UrdfJoint urdfJoint in GetComponentsInChildren<UrdfJoint>())
                urdfJoint.GenerateUniqueJointName();
        }

        // Add a rotation in the model which gives the correct correspondence between UnitySpace and RosSpace
        public void ChangeToCorrectedSpace()
        {
            this.transform.Rotate(0, 180, 0);
            changetoCorrectedSpace = !changetoCorrectedSpace;
        }

        public bool CheckOrientation()
        {
            return currentOrientation == chosenAxis;
        }

        public void SetOrientation()
        {
            currentOrientation = chosenAxis;
        }

        public void AddController()
        {
            if (!addController && this.gameObject.GetComponent< Unity.Robotics.UrdfImporter.Control.Controller>() == null)
            {
                this.gameObject.AddComponent<Unity.Robotics.UrdfImporter.Control.Controller>();
            }
            else
            {
                DestroyImmediate(this.gameObject.GetComponent<Unity.Robotics.UrdfImporter.Control.Controller>());
                DestroyImmediate(this.gameObject.GetComponent<Unity.Robotics.UrdfImporter.Control.FKRobot>());
                JointControl[] scriptList = GetComponentsInChildren<JointControl>();
                foreach (JointControl script in scriptList)
                    DestroyImmediate(script);
            }
            addController = !addController;
        }

        public void AddFkRobot()
        {
            if (!addFkRobot && this.gameObject.GetComponent<Unity.Robotics.UrdfImporter.Control.FKRobot>() == null)
            {
                this.gameObject.AddComponent<Unity.Robotics.UrdfImporter.Control.FKRobot>();
            }
            else
            {
                DestroyImmediate(this.gameObject.GetComponent<Unity.Robotics.UrdfImporter.Control.FKRobot>());
            }
            addFkRobot = !addFkRobot;
        }

        public void SetAxis(ImportSettings.axisType setAxis)
        {
            this.chosenAxis = setAxis;
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
