/*
© Siemens AG, 2017-2019
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

using System;
using UnityEngine;

namespace Unity.Robotics.UrdfImporter
{
    [RequireComponent(typeof(ArticulationBody))]
    public abstract class UrdfJoint : MonoBehaviour
    {
        public enum JointTypes
        {
            Fixed,
            Continuous,
            Revolute,
            Floating,
            Prismatic,
            Planar
        }

        public int xAxis = 0;

        protected ArticulationBody unityJoint;
        protected Vector3 axisofMotion;

        public string jointName;

        public abstract JointTypes JointType { get; } // Clear out syntax
        public bool IsRevoluteOrContinuous => JointType == JointTypes.Revolute || JointType == JointTypes.Revolute;
        public double EffortLimit = 1e3;
        public double VelocityLimit = 1e3;

        protected const int RoundDigits = 6;
        protected const float Tolerance = 0.0000001f;

        protected int defaultDamping = 0;
        protected int defaultFriction = 0;

        public static UrdfJoint Create(GameObject linkObject, JointTypes jointType, Joint joint = null)
        {
            UrdfJoint urdfJoint = AddCorrectJointType(linkObject, jointType);

            if (joint != null)
            {
                urdfJoint.jointName = joint.name;
                urdfJoint.ImportJointData(joint);
            }
            return urdfJoint;
        }

        private static UrdfJoint AddCorrectJointType(GameObject linkObject, JointTypes jointType)
        {
            UrdfJoint urdfJoint = null;

            switch (jointType)
            {
                case JointTypes.Fixed:
                    urdfJoint = UrdfJointFixed.Create(linkObject);
                    break;
                case JointTypes.Continuous:
                    urdfJoint = UrdfJointContinuous.Create(linkObject);
                    break;
                case JointTypes.Revolute:
                    urdfJoint = UrdfJointRevolute.Create(linkObject);
                    break;
                case JointTypes.Floating:
                    urdfJoint = UrdfJointFloating.Create(linkObject);
                    break;
                case JointTypes.Prismatic:
                    urdfJoint = UrdfJointPrismatic.Create(linkObject);
                    break;
                case JointTypes.Planar:
                    urdfJoint = UrdfJointPlanar.Create(linkObject);
                    break;
            }

            return urdfJoint;
        }

        /// <summary>
        /// Changes the type of the joint
        /// </summary>
        /// <param name="linkObject">Joint whose type is to be changed</param>
        /// <param name="newJointType">Type of the new joint</param>
        public static void ChangeJointType(GameObject linkObject, JointTypes newJointType)
        {
            linkObject.transform.DestroyImmediateIfExists<UrdfJoint>();
            linkObject.transform.DestroyImmediateIfExists<UnityEngine.ArticulationBody>();

            AddCorrectJointType(linkObject, newJointType);
        }

        public abstract Joint.Axis GetAxisData();

        #region Runtime

        public void Start()
        {
            unityJoint = GetComponent<ArticulationBody>();
        }

        public virtual float GetPosition()
        {
            return 0;
        }

        public virtual float GetVelocity()
        {
            return 0;
        }

        public virtual float GetEffort()
        {
            return 0;
        }

        public void UpdateJointState(float deltaState)
        {
            OnUpdateJointState(deltaState);
        }
        protected virtual void OnUpdateJointState(float deltaState) { }

        #endregion

        #region Import Helpers

        public static JointTypes GetJointType(string jointType)
        {
            switch (jointType)
            {
                case "fixed":
                    return JointTypes.Fixed;
                case "continuous":
                    return JointTypes.Continuous;
                case "revolute":
                    return JointTypes.Revolute;
                case "floating":
                    return JointTypes.Floating;
                case "prismatic":
                    return JointTypes.Prismatic;
                case "planar":
                    return JointTypes.Planar;
                default:
                    return JointTypes.Fixed;
            }
        }

        protected virtual void ImportJointData(Joint joint) { }

        protected static Vector3 GetAxis(Joint.Axis axis)
        {
            return axis.xyz.ToVector3().Ros2Unity();
        }

        protected static Vector3 GetDefaultAxis()
        {
            return new Vector3(-1, 0, 0);
        }

        protected virtual void SetAxisData(Vector3 axisofMotion) { }
        protected  virtual void SetLimits(Joint joint){}

        protected void SetDynamics(Joint.Dynamics dynamics)
        {
            if (unityJoint == null)
            {
                unityJoint = GetComponent<ArticulationBody>();
            }

            if (dynamics != null)
            {
                float damping = (double.IsNaN(dynamics.damping)) ? defaultDamping : (float)dynamics.damping;
                unityJoint.linearDamping = damping;
                unityJoint.angularDamping = damping;
                unityJoint.jointFriction = (double.IsNaN(dynamics.friction)) ? defaultFriction : (float)dynamics.friction;
            }
            else
            {
                unityJoint.linearDamping = defaultDamping;
                unityJoint.angularDamping = defaultDamping;
                unityJoint.jointFriction = defaultFriction;
            }
        }

        #endregion

        #region Export

        public Joint ExportJointData()
        {
            unityJoint = GetComponent<UnityEngine.ArticulationBody>();
            CheckForUrdfCompatibility();

            //Data common to all joints
            Joint joint = new Joint(
                jointName,
                JointType.ToString().ToLower(),
                gameObject.transform.parent.name,
                gameObject.name,
                UrdfOrigin.ExportOriginData(transform));

            joint.limit = ExportLimitData();
            return ExportSpecificJointData(joint);
        }

        public static Joint ExportDefaultJoint(Transform transform)
        {
            return new Joint(
                transform.parent.name + "_" + transform.name + "_joint",
                JointTypes.Fixed.ToString().ToLower(),
                transform.parent.name,
                transform.name,
                UrdfOrigin.ExportOriginData(transform));
        }

        #region ExportHelpers

        protected virtual Joint ExportSpecificJointData(Joint joint)
        {
            return joint;
        }

        protected virtual Joint.Limit ExportLimitData()
        {
            return null; // limits aren't used
        }

        public virtual bool AreLimitsCorrect()
        {
            return true; // limits aren't needed
        }

        protected virtual bool IsJointAxisDefined()
        {
            return true;
        }

        public void GenerateUniqueJointName()
        {
            jointName = transform.parent.name + "_" + transform.name + "_joint";
        }

        private bool IsAnchorTransformed() // TODO : Check for tolerances before implementation
        {

            UnityEngine.Joint joint = GetComponent<UnityEngine.Joint>();

            return Math.Abs(joint.anchor.x) > Tolerance ||
                Math.Abs(joint.anchor.x) > Tolerance ||
                Math.Abs(joint.anchor.x) > Tolerance;
        }

        private void CheckForUrdfCompatibility()
        {
            if (!AreLimitsCorrect())
                Debug.LogWarning("Limits are not defined correctly for Joint " + jointName + " in Link " + name +
                                 ". This may cause problems when visualizing the robot in RVIZ or Gazebo.",
                                 gameObject);
            if (!IsJointAxisDefined())
                Debug.LogWarning("Axis for joint " + jointName + " is undefined. Axis will not be written to URDF, " +
                                 "and the default axis will be used instead.",
                                 gameObject);
        }

        #endregion

        #endregion
    }
}

