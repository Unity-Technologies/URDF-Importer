/*
© Siemens AG, 2018-2019
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

namespace Unity.Robotics.UrdfImporter
{
    public class UrdfJointRevolute : UrdfJoint
    {
        public override JointTypes JointType => JointTypes.Revolute;

        public static UrdfJoint Create(GameObject linkObject)
        {
            UrdfJointRevolute urdfJoint = linkObject.AddComponent<UrdfJointRevolute>();
            urdfJoint.unityJoint = linkObject.GetComponent<ArticulationBody>();
            urdfJoint.unityJoint.jointType = ArticulationJointType.RevoluteJoint;

            return urdfJoint;
        }

        #region Runtime

        /// <summary>
        /// Returns the current position of the joint in radians
        /// </summary>
        /// <returns>floating point number for joint position in radians</returns>
        public override float GetPosition()
        {
            return ((ArticulationBody)unityJoint).jointPosition[xAxis];
        }

        /// <summary>
        /// Returns the current velocity of joint in radians per second
        /// </summary>
        /// <returns>floating point for joint velocity in radians per second</returns>
        public override float GetVelocity()
        {
            return ((ArticulationBody)unityJoint).jointVelocity[xAxis];
        }

        /// <summary>
        /// Returns current joint torque in Nm
        /// </summary>
        /// <returns>floating point in Nm</returns>
        public override float GetEffort()
        {
            return unityJoint.jointForce[xAxis];
        }


        /// <summary>
        /// Rotates the joint by deltaState radians 
        /// </summary>
        /// <param name="deltaState">amount in radians by which joint needs to be rotated</param>
        protected override void OnUpdateJointState(float deltaState)
        {
            ArticulationDrive drive = unityJoint.xDrive;
            drive.target += deltaState * Mathf.Rad2Deg;
            unityJoint.xDrive = drive;
        }

        #endregion

        protected override void ImportJointData(Joint joint)
        {
            var axis = (joint.axis != null && joint.axis.xyz != null) ? joint.axis.xyz.ToVector3() : new Vector3(1, 0, 0);
            SetAxisData(axis);
            SetLimits(joint);
            SetDynamics(joint.dynamics);
        }

        protected override Joint ExportSpecificJointData(Joint joint)
        {
            joint.axis = GetAxisData();
            joint.dynamics = new Joint.Dynamics(unityJoint.angularDamping, unityJoint.jointFriction);
            joint.limit = ExportLimitData();

            return joint;
        }

        public override bool AreLimitsCorrect()
        {
            ArticulationBody drive = GetComponent<ArticulationBody>();
            return drive.linearLockX == ArticulationDofLock.LimitedMotion && drive.xDrive.lowerLimit < drive.xDrive.upperLimit;
        }

        protected override Joint.Limit ExportLimitData()
        {
            ArticulationDrive drive = unityJoint.xDrive;
            return new Joint.Limit(drive.lowerLimit * Mathf.Deg2Rad, drive.upperLimit * Mathf.Deg2Rad, drive.forceLimit, unityJoint.maxAngularVelocity);
        }
        
        public override Joint.Axis GetAxisData()
        {
            var res = (unityJoint.anchorRotation * Vector3.left).Unity2Ros();
            double[] rosAxis = res.ToRoundedDoubleArray();
            return new Joint.Axis(rosAxis);
        }

        /// <summary>
        /// Reads axis joint information and rotation to the articulation body to produce the required motion
        /// </summary>
        /// <param name="joint">Structure containing joint information</param>
        protected override void SetAxisData(Vector3 axis)
        {
            axisofMotion = axis;
            Vector3 axisofMotionUnity = axisofMotion.Ros2Unity();
            Quaternion Motion = new Quaternion();
            Motion.SetFromToRotation(new Vector3(1, 0, 0), -1 * axisofMotionUnity);
            unityJoint.anchorRotation = Motion;
        }

        protected override void SetLimits(Joint joint)
        {
            unityJoint.linearLockX = ArticulationDofLock.LimitedMotion;
            unityJoint.linearLockY = ArticulationDofLock.LockedMotion;
            unityJoint.linearLockZ = ArticulationDofLock.LockedMotion;
            unityJoint.twistLock = ArticulationDofLock.LimitedMotion;

            if (joint.limit != null)
            {
                ArticulationDrive drive = unityJoint.xDrive;
                drive.upperLimit = (float)(joint.limit.upper * Mathf.Rad2Deg);
                drive.lowerLimit = (float)(joint.limit.lower * Mathf.Rad2Deg);
                drive.forceLimit = (float)(joint.limit.effort);
                unityJoint.maxAngularVelocity = (float)(joint.limit.velocity);
                drive.damping = unityJoint.xDrive.damping;
                drive.stiffness = unityJoint.xDrive.stiffness;
                unityJoint.xDrive = drive;
            }
        }
    }
}

