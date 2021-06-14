using NUnit.Framework;
using Unity.Robotics.UrdfImporter;
using UnityEngine;
using Joint = Unity.Robotics.UrdfImporter.Joint;


namespace Unity.Robotics.UrdfImporter.Tests
{
    public class TestUrdfJointPrismatic : UrdfJointPrismatic
    {
        public void TestImportJointData(Joint joint)
        {
            unityJoint = gameObject.GetComponent<ArticulationBody>();
            ImportJointData(joint);
        }

        public Joint TestExportSpecificJointData(Joint joint)
        {
            unityJoint = gameObject.GetComponent<ArticulationBody>();
            return ExportSpecificJointData(joint);
        }

        public void SetAxisOfMotion(Vector3 axisofMotion) => this.axisofMotion = axisofMotion;
        public void Dynamics(Joint.Dynamics dynamics) => SetDynamics(dynamics);
    }

    public class UrdfJointPrismaticTests
    {
        [Test]
        public void Create_UrdfJointPrismatic_Succeeds()
        {
            GameObject linkObject = new GameObject("link");
            UrdfJoint joint = UrdfJointPrismatic.Create(linkObject);
            UrdfJoint urdfJoint = linkObject.GetComponent<UrdfJoint>();
            ArticulationBody articulationBody = linkObject.GetComponent<ArticulationBody>();

            Assert.IsTrue(joint is UrdfJointPrismatic);
            Assert.AreEqual(joint, urdfJoint);
            Assert.AreEqual(ArticulationJointType.PrismaticJoint, articulationBody.jointType);

            Object.DestroyImmediate(linkObject);
        }

        [Test]
        public void GetPositionVelocityEffort_Succeeds()
        {
            GameObject baseObject = new GameObject("base");
            GameObject linkObject = new GameObject("link");
            linkObject.transform.parent = baseObject.transform;

            UrdfJoint.Create(baseObject, UrdfJoint.JointTypes.Fixed);
            UrdfJoint joint = UrdfJointPrismatic.Create(linkObject);
            ArticulationBody articulationBody = linkObject.GetComponent<ArticulationBody>();
            articulationBody.jointPosition = new ArticulationReducedSpace(1, 2, 3);
            articulationBody.jointVelocity = new ArticulationReducedSpace(4, 5, 6);
            articulationBody.jointForce = new ArticulationReducedSpace(7, 8, 9);

            Assert.AreEqual(1, joint.GetPosition());
            Assert.AreEqual(4, joint.GetVelocity());
            Assert.AreEqual(7, joint.GetEffort());

            Object.DestroyImmediate(baseObject);
        }

        [Test]
        public void UpdateJointState_Succeeds()
        {
            GameObject baseObject = new GameObject("base");
            GameObject linkObject = new GameObject("link");
            linkObject.transform.parent = baseObject.transform;

            UrdfJoint.Create(baseObject, UrdfJoint.JointTypes.Fixed);
            UrdfJoint joint = UrdfJointPrismatic.Create(linkObject);
            ArticulationBody articulationBody = linkObject.GetComponent<ArticulationBody>();

            Assert.AreEqual(0, articulationBody.xDrive.target);
            joint.UpdateJointState(1);
            Assert.AreEqual(1, articulationBody.xDrive.target);

            Object.DestroyImmediate(baseObject);
        }

        [Test]
        public void ImportJointData_SpecificAixs_Succeeds()
        {
            var joint = new Joint(
                name: "custom_joint", type: "prismatic", parent: "base", child: "link",
                axis: new Joint.Axis(new double[] { 1, 2, 3 }),
                limit: new Joint.Limit(4, 5, 6, 7),
                dynamics: new Joint.Dynamics(8, 9));

            GameObject baseObject = new GameObject("base");
            GameObject linkObject = new GameObject("link");
            linkObject.transform.parent = baseObject.transform;

            UrdfJoint.Create(baseObject, UrdfJoint.JointTypes.Fixed);
            TestUrdfJointPrismatic urdfJoint = linkObject.AddComponent<TestUrdfJointPrismatic>();
            ArticulationBody articulationBody = linkObject.GetComponent<ArticulationBody>();
            urdfJoint.TestImportJointData(joint);

            Assert.AreEqual(ArticulationDofLock.LimitedMotion, articulationBody.linearLockX);
            Assert.AreEqual(ArticulationDofLock.LockedMotion, articulationBody.linearLockY);
            Assert.AreEqual(ArticulationDofLock.LockedMotion, articulationBody.linearLockZ);

            Quaternion expectedAnchorRotation = new Quaternion();
            expectedAnchorRotation.SetFromToRotation(new Vector3(1, 0, 0), new Vector3(-2, 3, 1));
            Assert.AreEqual(expectedAnchorRotation, articulationBody.anchorRotation);

            Assert.AreEqual(4, articulationBody.xDrive.lowerLimit);
            Assert.AreEqual(5, articulationBody.xDrive.upperLimit);
            Assert.AreEqual(6, articulationBody.xDrive.forceLimit);
            Assert.AreEqual(7, articulationBody.maxLinearVelocity);
            Assert.AreEqual(8, articulationBody.linearDamping);
            Assert.AreEqual(8, articulationBody.angularDamping);
            Assert.AreEqual(9, articulationBody.jointFriction);

            Object.DestroyImmediate(baseObject);
        }

        [Test]
        public void ExportSpecificJointData_Succeeds()
        {
            GameObject linkObject = new GameObject("link");
            TestUrdfJointPrismatic urdfJoint = linkObject.AddComponent<TestUrdfJointPrismatic>();
            ArticulationBody articulationBody = linkObject.GetComponent<ArticulationBody>();
            urdfJoint.SetAxisOfMotion(new Vector3(1.2345678f, 2.3456789f, 3.4567891f));
            urdfJoint.Dynamics(new Joint.Dynamics(4, 5));

            var joint = new Joint(
                name: "custom_joint", type: "continuous", parent: "base", child: "link");
            urdfJoint.TestExportSpecificJointData(joint);

            UnityEngine.Assertions.Assert.AreApproximatelyEqual(1.234568f, (float)joint.axis.xyz[0]);
            UnityEngine.Assertions.Assert.AreApproximatelyEqual(2.345679f, (float)joint.axis.xyz[1]);
            UnityEngine.Assertions.Assert.AreApproximatelyEqual(3.456789f, (float)joint.axis.xyz[2]);
            Assert.AreEqual(4, joint.dynamics.damping);
            Assert.AreEqual(5, joint.dynamics.friction);
            Assert.AreEqual(articulationBody.xDrive.lowerLimit, joint.limit.lower);
            Assert.AreEqual(articulationBody.xDrive.upperLimit, joint.limit.upper);
            Assert.AreEqual(articulationBody.xDrive.forceLimit, joint.limit.effort);
            Assert.AreEqual(articulationBody.maxLinearVelocity, joint.limit.velocity);

            Object.DestroyImmediate(linkObject);
        }

        [Test]
        public void AreLimitsCorrect_Succeeds()
        {
            var joint = new Joint(
                name: "custom_joint", type: "planar", parent: "base", child: "link",
                limit: new Joint.Limit(4, 5, 6, 7),
                dynamics: new Joint.Dynamics(8, 9));
            GameObject linkObject = new GameObject("link");
            UrdfJoint urdfJoint = UrdfJoint.Create(linkObject, UrdfJoint.JointTypes.Prismatic, joint);

            Assert.IsTrue(urdfJoint.AreLimitsCorrect());

            Object.DestroyImmediate(linkObject);
        }

        [Test]
        public void AreLimitsCorrect_Fails()
        {
            var joint = new Joint(
                name: "custom_joint", type: "planar", parent: "base", child: "link",
                limit: new Joint.Limit(5, 4, 6, 7),
                dynamics: new Joint.Dynamics(8, 9));
            GameObject linkObject = new GameObject("link");
            UrdfJoint urdfJoint = UrdfJoint.Create(linkObject, UrdfJoint.JointTypes.Prismatic, joint);

            Assert.IsFalse(urdfJoint.AreLimitsCorrect());

            Object.DestroyImmediate(linkObject);
        }
    }
}
