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

namespace Unity.Robotics.UrdfImporter
{
    public class UrdfJointPlanar : UrdfJoint
    {
        public override JointTypes JointType => JointTypes.Planar;

        public static UrdfJoint Create(GameObject linkObject)
        {
            UrdfJointPlanar urdfJoint = linkObject.AddComponent<UrdfJointPlanar>();
            urdfJoint.unityJoint = linkObject.GetComponent<ArticulationBody>();
            urdfJoint.unityJoint.jointType = ArticulationJointType.PrismaticJoint;

            return urdfJoint;
        }

        public override float GetPosition()
        {
            Vector3 distanceFromAnchor = unityJoint.transform.localPosition;
            Debug.Log("'ArticulationBody' does not contain a definition for 'connectedAnchor' and no accessible extension method 'connectedAnchor'");
            return distanceFromAnchor.magnitude;
        }

        protected override void ImportJointData(Joint joint)
        {
            AdjustMovement(joint);
            SetDynamics(joint.dynamics);
        }

        private static JointDrive GetJointDrive(Joint.Dynamics dynamics)
        {
            return new JointDrive
            {
                maximumForce = float.MaxValue,
                positionDamper = (float)dynamics.damping,
                positionSpring = (float)dynamics.friction
            };
        }

        private static JointSpring GetJointSpring(Joint.Dynamics dynamics)
        {
            return new JointSpring
            {
                damper = (float)dynamics.damping,
                spring = (float)dynamics.friction,
                targetPosition = 0
            };
        }

        private static SoftJointLimit GetLinearLimit(Joint.Limit limit)
        {
            return new SoftJointLimit { limit = (float)limit.upper };
        }

        #region Export

        protected override Joint ExportSpecificJointData(Joint joint)
        {
            joint.axis = GetAxisData(axisofMotion);
            joint.dynamics = new Joint.Dynamics(unityJoint.linearDamping, unityJoint.jointFriction);
            joint.limit = ExportLimitData();
            return joint;
        }

        protected override Joint.Limit ExportLimitData()
        {
            ArticulationDrive drive = GetComponent<ArticulationBody>().yDrive;
            return new Joint.Limit(drive.lowerLimit, drive.upperLimit, EffortLimit, VelocityLimit);
        }

        public override bool AreLimitsCorrect()
        {
            ArticulationBody joint = GetComponent<ArticulationBody>();
            return joint.linearLockY == ArticulationDofLock.LimitedMotion &&
                joint.linearLockZ == ArticulationDofLock.LimitedMotion &&
                joint.yDrive.lowerLimit < joint.yDrive.upperLimit &&
                joint.zDrive.lowerLimit < joint.zDrive.upperLimit;
        }

        protected override bool IsJointAxisDefined()
        {
            // Vidur - what is this??
            Debug.Log("Cannot convert type 'UnityEngine.ArticulationBody' to 'UnityEngine.ConfigurableJoint'");
            return false;
        }

        protected override void AdjustMovement(Joint joint)
        {
            if (joint.axis == null || joint.axis.xyz == null)
            {
                joint.axis = new Joint.Axis(new double[] { 1, 0, 0 });
            }
            axisofMotion = new Vector3((float)joint.axis.xyz[0], (float)joint.axis.xyz[1], (float)joint.axis.xyz[2]);
            int motionAxis = joint.axis.AxisofMotion();
            Quaternion motion = unityJoint.anchorRotation;

            unityJoint.linearLockX = ArticulationDofLock.LockedMotion;
            if (joint.limit != null)
            {
                unityJoint.linearLockY = ArticulationDofLock.LimitedMotion;
                unityJoint.linearLockZ = ArticulationDofLock.LimitedMotion;
                var drive = new ArticulationDrive()
                {
                    stiffness = unityJoint.xDrive.stiffness,
                    damping = unityJoint.xDrive.damping,
                    forceLimit = (float)joint.limit.effort,
                    lowerLimit = (float)joint.limit.lower,
                    upperLimit = (float)joint.limit.upper,
                };
                unityJoint.xDrive = drive;
                unityJoint.zDrive = drive;
                unityJoint.yDrive = drive;
                unityJoint.maxLinearVelocity = (float)joint.limit.velocity;
            }
            else
            {
                unityJoint.linearLockZ = ArticulationDofLock.FreeMotion;
                unityJoint.linearLockY = ArticulationDofLock.FreeMotion;
            }

            switch (motionAxis)
            {
                case 0:
                    motion.eulerAngles = new Vector3(0, -90, 0);
                    break;
                case 1:
                    motion.eulerAngles = new Vector3(0, 0, 0);
                    break;
                case 2:
                    motion.eulerAngles = new Vector3(0, 0, 90);
                    break;
            }
            unityJoint.anchorRotation = motion;
        }

        #endregion
    }
}
