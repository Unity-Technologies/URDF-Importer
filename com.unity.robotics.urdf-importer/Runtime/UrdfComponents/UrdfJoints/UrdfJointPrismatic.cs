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
    public class UrdfJointPrismatic : UrdfJoint
    {
        private ArticulationDrive drive;

        public override JointTypes JointType => JointTypes.Prismatic;

        public static UrdfJoint Create(GameObject linkObject)
        {
            UrdfJointPrismatic urdfJoint = linkObject.AddComponent<UrdfJointPrismatic>();
            urdfJoint.unityJoint = linkObject.GetComponent<ArticulationBody>();
            urdfJoint.unityJoint.jointType = ArticulationJointType.PrismaticJoint;
            return urdfJoint;
        }

        #region Runtime

        /// <summary>
        /// Returns the current position of the joint in meters
        /// </summary>
        /// <returns>floating point number for joint position in meters</returns>
        public override float GetPosition()
        {
            return unityJoint.jointPosition[xAxis];
        }

        /// <summary>
        /// Returns the current velocity of joint in meters per second
        /// </summary>
        /// <returns>floating point for joint velocity in meters per second</returns>
        public override float GetVelocity()
        {
            return unityJoint.jointVelocity[xAxis];
        }

        /// <summary>
        /// Returns current joint torque in N
        /// </summary>
        /// <returns>floating point in N</returns>
        public override float GetEffort()
        {
            return unityJoint.jointForce[xAxis];
        }

        /// <summary>
        /// Rotates the joint by deltaState m
        /// </summary>
        /// <param name="deltaState">amount in m by which joint needs to be rotated</param>
        protected override void OnUpdateJointState(float deltaState)
        {
            ArticulationDrive drive = unityJoint.xDrive;
            drive.target += deltaState;
            unityJoint.xDrive = drive;
        }

        #endregion

        #region Import

        protected override void ImportJointData(Joint joint)
        {
            var axis = (joint.axis != null && joint.axis.xyz != null) ? joint.axis.xyz.ToVector3() : new Vector3(1, 0, 0);
            SetAxisData(axis);
            SetLimits(joint);
            SetDynamics(joint.dynamics);
        }

        /// <summary>
        /// Reads axis joint information and rotation to the articulation body to produce the required motion
        /// </summary>
        /// <param name="joint">Structure containing joint information</param>
        protected override void SetAxisData(Vector3 axis) // Test this function
        {
            axisofMotion = axis;
            Vector3 axisofMotionUnity = axisofMotion.Ros2Unity();
            Quaternion Motion = new Quaternion();
            Motion.SetFromToRotation(new Vector3(1, 0, 0), axisofMotionUnity);
            unityJoint.anchorRotation = Motion;
        }

        protected override void SetLimits(Joint joint)
        {
            unityJoint.linearLockX = (joint.limit != null) ? ArticulationDofLock.LimitedMotion : ArticulationDofLock.FreeMotion;
            unityJoint.linearLockY = ArticulationDofLock.LockedMotion;
            unityJoint.linearLockZ = ArticulationDofLock.LockedMotion;
            
            if (joint.limit != null)
            {
                ArticulationDrive drive = unityJoint.xDrive;
                drive.upperLimit = (float)joint.limit.upper;
                drive.lowerLimit = (float)joint.limit.lower;
                drive.forceLimit = (float)joint.limit.effort;

                unityJoint.maxLinearVelocity = (float)joint.limit.velocity;
                unityJoint.xDrive = drive;
            }
        }
        
        public override Joint.Axis GetAxisData()
        {
            var res = (unityJoint.anchorRotation * Vector3.right).Unity2Ros();
            double[] rosAxis = res.ToRoundedDoubleArray();
            return new Joint.Axis(rosAxis);
        }

        #endregion


        #region Export

        protected override Joint ExportSpecificJointData(Joint joint)
        {
            joint.axis = GetAxisData();
            joint.dynamics = new Joint.Dynamics(unityJoint.linearDamping, unityJoint.jointFriction);
            joint.limit = ExportLimitData();
            return joint;
        }

        public override bool AreLimitsCorrect()
        {
            ArticulationBody joint = GetComponent<ArticulationBody>();
            return joint.linearLockX == ArticulationDofLock.LimitedMotion && joint.xDrive.lowerLimit < joint.xDrive.upperLimit;
        }

        protected override Joint.Limit ExportLimitData()
        {
            ArticulationDrive drive = GetComponent<ArticulationBody>().xDrive;
            return new Joint.Limit(drive.lowerLimit, drive.upperLimit, drive.forceLimit, unityJoint.maxLinearVelocity);
        }

        #endregion
    }
}
