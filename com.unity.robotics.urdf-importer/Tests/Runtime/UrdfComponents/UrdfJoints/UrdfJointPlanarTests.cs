using System.Collections.Generic;
using NUnit.Framework;
using RosSharp.Urdf;
using UnityEngine;
using Joint = RosSharp.Urdf.Joint;

public class TestUrdfJointPlanar : UrdfJointPlanar
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

    public Joint.Limit TestExportLimitData => ExportLimitData();
    public bool TestIsJointAxisDefined => IsJointAxisDefined();

    public void SetAxisOfMotion(Vector3 axisofMotion) => this.axisofMotion = axisofMotion;
    public void Dynamics(Joint.Dynamics dynamics) => SetDynamics(dynamics);
}

public class UrdfJointPlanarTests
{
    [Test]
    public void Create_UrdfJointPlanar_Succeeds()
    {
        GameObject linkObject = new GameObject("link");
        UrdfJoint joint = UrdfJointPlanar.Create(linkObject);
        UrdfJoint urdfJoint = linkObject.GetComponent<UrdfJoint>();
        ArticulationBody articulationBody = linkObject.GetComponent<ArticulationBody>();

        Assert.IsTrue(joint is UrdfJointPlanar);
        Assert.AreEqual(joint, urdfJoint);
        Assert.AreEqual(ArticulationJointType.PrismaticJoint, articulationBody.jointType);

        Object.DestroyImmediate(linkObject);
    }

    [Test]
    public void GetPosition_Succeeds()
    {
        GameObject baseObject = new GameObject("base");
        GameObject linkObject = new GameObject("link");
        linkObject.transform.parent = baseObject.transform;
        linkObject.transform.localPosition = new Vector3(1, 2, 3);

        UrdfJoint.Create(baseObject, UrdfJoint.JointTypes.Fixed);
        UrdfJoint joint = UrdfJointPlanar.Create(linkObject);

        Assert.AreEqual(linkObject.transform.localPosition.magnitude, joint.GetPosition());

        Object.DestroyImmediate(baseObject);
    }

    [Test, TestCaseSource("AxisType")]
    public void ImportJointData_DefaultAxis_Succeeds(Joint.Axis axis, Quaternion expectedAnchorRotation)
    {
        var joint = new Joint(
            name: "custom_joint", type: "continuous", parent: "base", child: "link",
            axis: axis);

        GameObject baseObject = new GameObject("base");
        GameObject linkObject = new GameObject("link");
        linkObject.transform.parent = baseObject.transform;

        UrdfJoint.Create(baseObject, UrdfJoint.JointTypes.Fixed);
        TestUrdfJointPlanar urdfJoint = linkObject.AddComponent<TestUrdfJointPlanar>();
        ArticulationBody articulationBody = linkObject.GetComponent<ArticulationBody>();
        urdfJoint.TestImportJointData(joint);

        UnityEngine.Assertions.Assert.AreApproximatelyEqual(expectedAnchorRotation.w, articulationBody.anchorRotation.w);
        UnityEngine.Assertions.Assert.AreApproximatelyEqual(expectedAnchorRotation.x, articulationBody.anchorRotation.x);
        UnityEngine.Assertions.Assert.AreApproximatelyEqual(expectedAnchorRotation.y, articulationBody.anchorRotation.y);
        UnityEngine.Assertions.Assert.AreApproximatelyEqual(expectedAnchorRotation.z, articulationBody.anchorRotation.z);

        Object.DestroyImmediate(baseObject);
    }

    private static IEnumerable<TestCaseData> AxisType
    {
        get
        {
            yield return new TestCaseData(null, Quaternion.Euler(new Vector3(0, -90, 0)));
            yield return new TestCaseData(new Joint.Axis(new double[] { 1, 0, 0 }), Quaternion.Euler(new Vector3(0, -90, 0)));
            yield return new TestCaseData(new Joint.Axis(new double[] { 0, 1, 0 }), Quaternion.Euler(new Vector3(0, 0, 0)));
            yield return new TestCaseData(new Joint.Axis(new double[] { 0, 0, 1 }), Quaternion.Euler(new Vector3(0, 0, 90)));
        }
    }

    [Test]
    public void ImportJointData_SpecificLimit_Succeeds()
    {
        var joint = new Joint(
            name: "custom_joint", type: "planar", parent: "base", child: "link",
            limit: new Joint.Limit(4, 5, 6, 7),
            dynamics: new Joint.Dynamics(8, 9));

        GameObject baseObject = new GameObject("base");
        GameObject linkObject = new GameObject("link");
        linkObject.transform.parent = baseObject.transform;

        UrdfJoint.Create(baseObject, UrdfJoint.JointTypes.Fixed);
        TestUrdfJointPlanar urdfJoint = linkObject.AddComponent<TestUrdfJointPlanar>();
        ArticulationBody articulationBody = linkObject.GetComponent<ArticulationBody>();
        urdfJoint.TestImportJointData(joint);

        Assert.AreEqual(ArticulationDofLock.LockedMotion, articulationBody.linearLockX);
        Assert.AreEqual(ArticulationDofLock.LimitedMotion, articulationBody.linearLockY);
        Assert.AreEqual(ArticulationDofLock.LimitedMotion, articulationBody.linearLockZ);

        Assert.AreEqual(4, articulationBody.xDrive.lowerLimit);
        Assert.AreEqual(4, articulationBody.yDrive.lowerLimit);
        Assert.AreEqual(4, articulationBody.zDrive.lowerLimit);
        Assert.AreEqual(5, articulationBody.xDrive.upperLimit);
        Assert.AreEqual(5, articulationBody.yDrive.upperLimit);
        Assert.AreEqual(5, articulationBody.zDrive.upperLimit);
        Assert.AreEqual(6, articulationBody.xDrive.forceLimit);
        Assert.AreEqual(6, articulationBody.yDrive.forceLimit);
        Assert.AreEqual(6, articulationBody.zDrive.forceLimit);
        Assert.AreEqual(7, articulationBody.maxLinearVelocity);

        Object.DestroyImmediate(baseObject);
    }

    [Test]
    public void ExportSpecificJointData_Succeeds()
    {
        GameObject linkObject = new GameObject("link");
        TestUrdfJointPlanar urdfJoint = linkObject.AddComponent<TestUrdfJointPlanar>();
        urdfJoint.SetAxisOfMotion(new Vector3(1.2345678f, 2.3456789f, 3.4567891f));
        urdfJoint.Dynamics(new Joint.Dynamics(4, 5));

        var joint = new Joint(
            name: "custom_joint", type: "planar", parent: "base", child: "link");
        urdfJoint.TestExportSpecificJointData(joint);

        UnityEngine.Assertions.Assert.AreApproximatelyEqual(1.234568f, (float)joint.axis.xyz[0]);
        UnityEngine.Assertions.Assert.AreApproximatelyEqual(2.345679f, (float)joint.axis.xyz[1]);
        UnityEngine.Assertions.Assert.AreApproximatelyEqual(3.456789f, (float)joint.axis.xyz[2]);
        Assert.AreEqual(4, joint.dynamics.damping);
        Assert.AreEqual(5, joint.dynamics.friction);

        Object.DestroyImmediate(linkObject);
    }

    [Test]
    public void ExportLimitData_Succeeds()
    {
        GameObject linkObject = new GameObject("link");
        TestUrdfJointPlanar joint = linkObject.AddComponent<TestUrdfJointPlanar>();
        ArticulationBody articulationBody = linkObject.GetComponent<ArticulationBody>();
        articulationBody.yDrive = new ArticulationDrive()
        {
            lowerLimit = 1,
            upperLimit = 2,
        };
        Joint.Limit limit = joint.TestExportLimitData;

        Assert.AreEqual(1, limit.lower);
        Assert.AreEqual(2, limit.upper);
        Assert.AreEqual(joint.EffortLimit, limit.effort);
        Assert.AreEqual(joint.VelocityLimit, limit.velocity);

        Object.DestroyImmediate(linkObject);
    }

    [Test]
    public void Create_AreLimitCorrect_Succeeds()
    {
        var joint = new Joint(
            name: "custom_joint", type: "planar", parent: "base", child: "link",
            limit: new Joint.Limit(4, 5, 6, 7),
            dynamics: new Joint.Dynamics(8, 9));
        GameObject linkObject = new GameObject("link");
        UrdfJoint urdfJoint = UrdfJoint.Create(linkObject, UrdfJoint.JointTypes.Planar, joint);

        Assert.IsTrue(urdfJoint.AreLimitsCorrect());

        Object.DestroyImmediate(linkObject);
    }

    [Test]
    public void IsJointAxisDefined_IsFalse()
    {
        GameObject linkObject = new GameObject("link");
        TestUrdfJointPlanar joint = linkObject.AddComponent<TestUrdfJointPlanar>();

        Assert.IsFalse(joint.TestIsJointAxisDefined);

        Object.DestroyImmediate(linkObject);
    }
}
