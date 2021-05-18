using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using RosSharp.Urdf;

public class TestUrdfJoint : UrdfJoint
{
    public override JointTypes JointType => throw new System.NotImplementedException();

    public static Vector3 Axis(RosSharp.Urdf.Joint.Axis axis) => GetAxis(axis);
    public static Vector3 DefaultAxis() => GetDefaultAxis();
    public static RosSharp.Urdf.Joint.Axis AxisData(Vector3 axis) => GetAxisData(axis);
    public void Dynamics(RosSharp.Urdf.Joint.Dynamics dynamics) => SetDynamics(dynamics);
}

public class UrdfJointTests
{
#if UNITY_2020_1_OR_NEWER
    [Test]
    public void Create_FixedArticulationBody()
    {
        GameObject linkObject = new GameObject("link");
        UrdfJoint.Create(linkObject, UrdfJoint.JointTypes.Fixed);

        UrdfJoint urdfJoint = linkObject.GetComponent<UrdfJointFixed>();
        ArticulationBody articulationBody = linkObject.GetComponent<ArticulationBody>();

        Assert.IsNotNull(urdfJoint);
        Assert.IsNotNull(articulationBody);
        Assert.AreEqual(UrdfJoint.JointTypes.Fixed, urdfJoint.JointType);
        Assert.AreEqual(ArticulationJointType.FixedJoint, articulationBody.jointType);

        Object.DestroyImmediate(linkObject);
    }
#endif

#if UNITY_2020_1_OR_NEWER
    [Test]
    public void Create_WithOtherTypeOfJointData_FixedArticulationBody()
    {
        RosSharp.Urdf.Joint joint = new RosSharp.Urdf.Joint(
            name: "reference", type: "prismatic", parent: null, child: null);
        GameObject linkObject = new GameObject("link");
        UrdfJoint.Create(linkObject, UrdfJoint.JointTypes.Fixed, joint);

        UrdfJoint urdfJoint = linkObject.GetComponent<UrdfJointFixed>();
        ArticulationBody articulationBody = linkObject.GetComponent<ArticulationBody>();

        Assert.IsNotNull(urdfJoint);
        Assert.IsNotNull(articulationBody);
        Assert.AreEqual(UrdfJoint.JointTypes.Fixed, urdfJoint.JointType);
        Assert.AreEqual(ArticulationJointType.FixedJoint, articulationBody.jointType);
    }
#endif

#if UNITY_2020_1_OR_NEWER
    // TODO: (pjing) add more unit tests for the create methods with RosSharp.Urdf.Joint when testing specific joint types
    [Test]
    public void Create_ContinuousArticulationBody()
    {
        GameObject linkObject = new GameObject("link");
        UrdfJoint.Create(linkObject, UrdfJoint.JointTypes.Continuous);

        UrdfJoint urdfJoint = linkObject.GetComponent<UrdfJointContinuous>();
        ArticulationBody articulationBody = linkObject.GetComponent<ArticulationBody>();

        Assert.IsNotNull(urdfJoint);
        Assert.IsNotNull(articulationBody);
        Assert.AreEqual(UrdfJoint.JointTypes.Continuous, urdfJoint.JointType);
        Assert.AreEqual(ArticulationJointType.RevoluteJoint, articulationBody.jointType);

        Object.DestroyImmediate(linkObject);
    }
#endif

#if UNITY_2020_1_OR_NEWER
    // TODO: (pjing) add more unit tests for the create methods with RosSharp.Urdf.Joint when testing specific joint types
    [Test]
    public void Create_FloatingArticulationBody()
    {
        GameObject linkObject = new GameObject("link");
        UrdfJoint.Create(linkObject, UrdfJoint.JointTypes.Floating);

        UrdfJoint urdfJoint = linkObject.GetComponent<UrdfJointFloating>();
        ArticulationBody articulationBody = linkObject.GetComponent<ArticulationBody>();

        Assert.IsNotNull(urdfJoint);
        Assert.IsNotNull(articulationBody);
        Assert.AreEqual(UrdfJoint.JointTypes.Floating, urdfJoint.JointType);
        Assert.AreEqual(ArticulationJointType.FixedJoint, articulationBody.jointType);

        Object.DestroyImmediate(linkObject);
    }
#endif

#if UNITY_2020_1_OR_NEWER
    // TODO: (pjing) add more unit tests for the create methods with RosSharp.Urdf.Joint when testing specific joint types
    [Test]
    public void Create_PlanarArticulationBody()
    {
        GameObject linkObject = new GameObject("link");
        UrdfJoint.Create(linkObject, UrdfJoint.JointTypes.Planar);

        UrdfJoint urdfJoint = linkObject.GetComponent<UrdfJointPlanar>();
        ArticulationBody articulationBody = linkObject.GetComponent<ArticulationBody>();

        Assert.IsNotNull(urdfJoint);
        Assert.IsNotNull(articulationBody);
        Assert.AreEqual(UrdfJoint.JointTypes.Planar, urdfJoint.JointType);
        Assert.AreEqual(ArticulationJointType.PrismaticJoint, articulationBody.jointType);

        Object.DestroyImmediate(linkObject);
    }
#endif

#if UNITY_2020_1_OR_NEWER
    // TODO: (pjing) add more unit tests for the create methods with RosSharp.Urdf.Joint when testing specific joint types
    [Test]
    public void Create_RevoluteArticulationBody()
    {
        GameObject linkObject = new GameObject("link");
        UrdfJoint.Create(linkObject, UrdfJoint.JointTypes.Revolute);

        UrdfJoint urdfJoint = linkObject.GetComponent<UrdfJointRevolute>();
        ArticulationBody articulationBody = linkObject.GetComponent<ArticulationBody>();

        Assert.IsNotNull(urdfJoint);
        Assert.IsNotNull(articulationBody);
        Assert.AreEqual(UrdfJoint.JointTypes.Revolute, urdfJoint.JointType);
        Assert.AreEqual(ArticulationJointType.RevoluteJoint, articulationBody.jointType);

        Object.DestroyImmediate(linkObject);
    }
#endif

#if UNITY_2020_1_OR_NEWER
    // TODO: (pjing) add more unit tests for the create methods with RosSharp.Urdf.Joint when testing specific joint types
    [Test]
    public void Create_PrismaticArticulationBody()
    {
        GameObject linkObject = new GameObject("link");
        UrdfJoint.Create(linkObject, UrdfJoint.JointTypes.Prismatic);

        UrdfJoint urdfJoint = linkObject.GetComponent<UrdfJointPrismatic>();
        ArticulationBody articulationBody = linkObject.GetComponent<ArticulationBody>();

        Assert.IsNotNull(urdfJoint);
        Assert.IsNotNull(articulationBody);
        Assert.AreEqual(UrdfJoint.JointTypes.Prismatic, urdfJoint.JointType);
        Assert.AreEqual(ArticulationJointType.PrismaticJoint, articulationBody.jointType);

        Object.DestroyImmediate(linkObject);
    }
#endif

#if UNITY_2020_1_OR_NEWER
    [Test]
    public void ChangeJointType_FromFixedToRevolute()
    {
        GameObject baseLink = new GameObject("base");
        GameObject linkObject = new GameObject("link");
        linkObject.transform.parent = baseLink.transform;

        UrdfJoint.Create(baseLink, UrdfJoint.JointTypes.Fixed);
        UrdfJoint.Create(linkObject, UrdfJoint.JointTypes.Fixed);
        ArticulationBody articulationBody = linkObject.GetComponent<ArticulationBody>();

        Assert.AreEqual(ArticulationJointType.FixedJoint, articulationBody.jointType);
        Assert.AreEqual(0, articulationBody.dofCount);

        UrdfJoint.ChangeJointType(linkObject, UrdfJoint.JointTypes.Revolute);
        articulationBody = linkObject.GetComponent<ArticulationBody>();

        Assert.AreEqual(ArticulationJointType.RevoluteJoint, articulationBody.jointType);
        Assert.AreEqual(1, articulationBody.dofCount);

        Object.DestroyImmediate(linkObject);
    }
#endif

    [Test]
    public void GetJointType_AllJointTypes()
    {
        Assert.AreEqual(UrdfJoint.JointTypes.Fixed, UrdfJoint.GetJointType("fixed"));
        Assert.AreEqual(UrdfJoint.JointTypes.Continuous, UrdfJoint.GetJointType("continuous"));
        Assert.AreEqual(UrdfJoint.JointTypes.Revolute, UrdfJoint.GetJointType("revolute"));
        Assert.AreEqual(UrdfJoint.JointTypes.Floating, UrdfJoint.GetJointType("floating"));
        Assert.AreEqual(UrdfJoint.JointTypes.Prismatic, UrdfJoint.GetJointType("prismatic"));
        Assert.AreEqual(UrdfJoint.JointTypes.Planar, UrdfJoint.GetJointType("planar"));
        Assert.AreEqual(UrdfJoint.JointTypes.Fixed, UrdfJoint.GetJointType("unknown"));
    }

    [Test]
    public void GetAxis_JointAxis()
    {
        var axis = new RosSharp.Urdf.Joint.Axis(new double[] { 1, 2, 3 });
        Assert.AreEqual(new Vector3(-2, 3, 1), TestUrdfJoint.Axis(axis));
    }

    [Test]
    public void GetDefaultAxis_JointAxis()
    {
        Assert.AreEqual(new Vector3(-1, 0, 0), TestUrdfJoint.DefaultAxis());
    }

    [Test]
    public void SetDynamics_Success()
    {
        var dynamics = new RosSharp.Urdf.Joint.Dynamics(1, 2);
        GameObject linkObject = new GameObject("link");
        var joint = linkObject.AddComponent<TestUrdfJoint>();
        joint.Dynamics(dynamics);

        var articulationBody = linkObject.GetComponent<ArticulationBody>();
        Assert.AreEqual(1, articulationBody.linearDamping);
        Assert.AreEqual(1, articulationBody.angularDamping);
        Assert.AreEqual(2, articulationBody.jointFriction);

        Object.DestroyImmediate(linkObject);
    }

    [Test]
    public void SetDynamics_DefaultDynamics()
    {
        GameObject linkObject = new GameObject("link");
        var joint = linkObject.AddComponent<TestUrdfJoint>();
        joint.Dynamics(null);

        var articulationBody = linkObject.GetComponent<ArticulationBody>();
        Assert.AreEqual(0, articulationBody.linearDamping);
        Assert.AreEqual(0, articulationBody.angularDamping);
        Assert.AreEqual(0, articulationBody.jointFriction);

        Object.DestroyImmediate(linkObject);
    }

    [Test]
    public void ExportJointData_Success()
    {
        GameObject baseObject = new GameObject("base");
        GameObject linkObject = new GameObject("link");
        linkObject.transform.parent = baseObject.transform;
        linkObject.transform.position = new Vector3(1, 2, 3);
        linkObject.transform.rotation = Quaternion.Euler(4, 5, 6);

        UrdfJoint.Create(baseObject, UrdfJoint.JointTypes.Fixed);
        UrdfJoint.Create(linkObject, UrdfJoint.JointTypes.Revolute);
        var joint = linkObject.GetComponent<UrdfJoint>().ExportJointData();

        Assert.AreEqual(null, joint.name);
        Assert.AreEqual("revolute", joint.type);
        Assert.AreEqual("base", joint.parent);
        Assert.AreEqual("link", joint.child);
        Assert.AreEqual(new double[] { 3, -1, 2 }, joint.origin.Xyz);
        UnityEngine.Assertions.Assert.AreApproximatelyEqual(-6 * Mathf.Deg2Rad, (float)joint.origin.Rpy[0]);
        UnityEngine.Assertions.Assert.AreApproximatelyEqual(4 * Mathf.Deg2Rad, (float)joint.origin.Rpy[1]);
        UnityEngine.Assertions.Assert.AreApproximatelyEqual(-5 * Mathf.Deg2Rad, (float)joint.origin.Rpy[2]);

        Object.DestroyImmediate(baseObject);
        Object.DestroyImmediate(linkObject);
    }

    [Test]
    public void GenerateUniqueJointName_Name()
    {
        GameObject baseObject = new GameObject("base");
        GameObject linkObject = new GameObject("link");
        linkObject.transform.parent = baseObject.transform;

        UrdfJoint.Create(linkObject, UrdfJoint.JointTypes.Revolute);
        var joint = linkObject.GetComponent<UrdfJoint>();

        Assert.AreEqual(null, joint.jointName);
        joint.GenerateUniqueJointName();
        Assert.AreEqual("base_link_joint", joint.jointName);

        Object.DestroyImmediate(baseObject);
        Object.DestroyImmediate(linkObject);
    }

    [Test]
    public void GetAxisData_Success()
    {
        Assert.AreEqual(new double[] { 1.234568, 2.345679, 3.456789 },
            TestUrdfJoint.AxisData(new Vector3(1.2345678f, 2.3456789f, 3.4567891f)).xyz);
    }
}
