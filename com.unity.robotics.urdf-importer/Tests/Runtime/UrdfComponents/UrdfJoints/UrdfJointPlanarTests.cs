using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using RosSharp.Urdf;

public class TestUrdfJointPlanar : UrdfJointPlanar
{
    public void TestImportJointData(RosSharp.Urdf.Joint joint)
    {
        unityJoint = gameObject.GetComponent<ArticulationBody>();
        ImportJointData(joint);
    }
}

public class UrdfJointPlanarTests
{
    [Test]
    public void Create_Succeeds()
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
        Object.DestroyImmediate(linkObject);
    }

    [Test]
    public void ImportJointData_DefaultAxis_Succeeds()
    {
        var joint = new RosSharp.Urdf.Joint(
            name: "custom_joint", type: "continuous", parent: "base", child: "link",
            axis: null);

        GameObject baseObject = new GameObject("base");
        GameObject linkObject = new GameObject("link");
        linkObject.transform.parent = baseObject.transform;

        UrdfJoint.Create(baseObject, UrdfJoint.JointTypes.Fixed);
        TestUrdfJointPlanar urdfJoint = linkObject.AddComponent<TestUrdfJointPlanar>();
        ArticulationBody articulationBody = linkObject.GetComponent<ArticulationBody>();
        urdfJoint.TestImportJointData(joint);

        Assert.AreEqual(Quaternion.Euler(new Vector3(0, -90, 0)), articulationBody.anchorRotation);

        Object.DestroyImmediate(linkObject);
        Object.DestroyImmediate(baseObject);
    }

    [Test]
    public void ImportJointData_SpecificLimit_Succeeds()
    {
        var joint = new RosSharp.Urdf.Joint(
            name: "custom_joint", type: "continuous", parent: "base", child: "link",
            axis: new RosSharp.Urdf.Joint.Axis(new double[] { 0, 1, 0 }),
            limit: new RosSharp.Urdf.Joint.Limit(4, 5, 6, 7),
            dynamics: new RosSharp.Urdf.Joint.Dynamics(8, 9));

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
        Assert.AreEqual(Quaternion.Euler(Vector3.zero), articulationBody.anchorRotation);

        Object.DestroyImmediate(linkObject);
        Object.DestroyImmediate(baseObject);
    }
}
