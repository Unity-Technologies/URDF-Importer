using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using RosSharp.Urdf;
using Joint = RosSharp.Urdf.Joint;

public class TestUrdfJoint : UrdfJoint
{
    public override JointTypes JointType => throw new System.NotImplementedException();

    public static Vector3 Axis(Joint.Axis axis) => GetAxis(axis);
    public static Vector3 DefaultAxis() => GetDefaultAxis();
    public static Joint.Axis AxisData(Vector3 axis) => GetAxisData(axis);
    public void Dynamics(Joint.Dynamics dynamics) => SetDynamics(dynamics);
}

public class UrdfJointTests
{

#if UNITY_2020_1_OR_NEWER
    [Test, TestCaseSource("JointTypes")]
    public void Create_UrdfJoint_Succeeds(UrdfJoint.JointTypes urdfJointType, ArticulationJointType articulationJointType)
    {
        GameObject linkObject = new GameObject("link");
        UrdfJoint urdfJoint = UrdfJoint.Create(linkObject, urdfJointType);
        ArticulationBody articulationBody = linkObject.GetComponent<ArticulationBody>();

        Assert.IsNotNull(urdfJoint);
        Assert.IsNotNull(articulationBody);
        Assert.AreEqual(urdfJointType, urdfJoint.JointType);
        Assert.AreEqual(articulationJointType, articulationBody.jointType);

        Object.DestroyImmediate(linkObject);
    }

    private static IEnumerable<TestCaseData> JointTypes
    {
        get
        {
            yield return new TestCaseData(UrdfJoint.JointTypes.Fixed, ArticulationJointType.FixedJoint);
            yield return new TestCaseData(UrdfJoint.JointTypes.Continuous, ArticulationJointType.RevoluteJoint);
            yield return new TestCaseData(UrdfJoint.JointTypes.Floating, ArticulationJointType.FixedJoint);
            yield return new TestCaseData(UrdfJoint.JointTypes.Planar, ArticulationJointType.PrismaticJoint);
            yield return new TestCaseData(UrdfJoint.JointTypes.Prismatic, ArticulationJointType.PrismaticJoint);
            yield return new TestCaseData(UrdfJoint.JointTypes.Revolute, ArticulationJointType.RevoluteJoint);
        }
    }

    [Test]
    public void Create_WithOtherTypeOfJointData_FixedArticulationBody()
    {
        Joint joint = new Joint(
            name: "reference", type: "prismatic", parent: null, child: null);
        GameObject linkObject = new GameObject("link");
        UrdfJoint urdfJoint = UrdfJoint.Create(linkObject, UrdfJoint.JointTypes.Fixed, joint);
        ArticulationBody articulationBody = linkObject.GetComponent<ArticulationBody>();

        Assert.IsNotNull(urdfJoint);
        Assert.IsNotNull(articulationBody);
        Assert.AreEqual(UrdfJoint.JointTypes.Fixed, urdfJoint.JointType);
        Assert.AreEqual(ArticulationJointType.FixedJoint, articulationBody.jointType);
    }

    [Test]
    public void Create_WithJointData_Succeeds()
    {
        GameObject baseObject = new GameObject("base");
        GameObject linkObject = new GameObject("link");
        linkObject.transform.parent = baseObject.transform;

        var joint = new Joint("custom_name", "revolute", "base", "link");
        UrdfJoint urdfJoint = UrdfJoint.Create(linkObject, UrdfJoint.JointTypes.Prismatic, joint);

        Assert.AreEqual("custom_name", urdfJoint.jointName);

        Object.DestroyImmediate(baseObject);
        Object.DestroyImmediate(linkObject);
    }

    [Test]
    public void ChangeJointType_FromFixedToRevolute_Succeeds()
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
    public void GetJointType_AllJointTypes_Succeeds()
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
    public void GetAxis_JointAxis_Succeeds()
    {
        var axis = new Joint.Axis(new double[] { 1, 2, 3 });
        Assert.AreEqual(new Vector3(-2, 3, 1), TestUrdfJoint.Axis(axis));
    }

    [Test]
    public void GetDefaultAxis_JointAxis_Succeeds()
    {
        Assert.AreEqual(new Vector3(-1, 0, 0), TestUrdfJoint.DefaultAxis());
    }

    [Test]
    public void SetDynamics_ArbitraryDynamics_Succeeds()
    {
        var dynamics = new Joint.Dynamics(1, 2);
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
    public void SetDynamics_DefaultDynamics_Succeeds()
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
    public void ExportJointData_ArbitraryJointData_Succeeds()
    {
        Vector3 position = new Vector3(1, 2, 3);
        Quaternion rotation = Quaternion.Euler(4, 5, 6);

        GameObject baseObject = new GameObject("base");
        GameObject linkObject = new GameObject("link");
        linkObject.transform.parent = baseObject.transform;
        linkObject.transform.position = position;
        linkObject.transform.rotation = rotation;

        UrdfJoint.Create(baseObject, UrdfJoint.JointTypes.Fixed);
        UrdfJoint.Create(linkObject, UrdfJoint.JointTypes.Revolute);
        var joint = linkObject.GetComponent<UrdfJoint>().ExportJointData();

        Assert.IsNull(joint.name);
        Assert.AreEqual("revolute", joint.type);
        Assert.AreEqual(baseObject.name, joint.parent);
        Assert.AreEqual(linkObject.name, joint.child);
        Assert.AreEqual(new double[] { position[2], -position[0], position[1] }, joint.origin.Xyz);
        UnityEngine.Assertions.Assert.AreApproximatelyEqual(-rotation.eulerAngles[2] * Mathf.Deg2Rad, (float)joint.origin.Rpy[0]);
        UnityEngine.Assertions.Assert.AreApproximatelyEqual(rotation.eulerAngles[0] * Mathf.Deg2Rad, (float)joint.origin.Rpy[1]);
        UnityEngine.Assertions.Assert.AreApproximatelyEqual(-rotation.eulerAngles[1] * Mathf.Deg2Rad, (float)joint.origin.Rpy[2]);

        Object.DestroyImmediate(baseObject);
        Object.DestroyImmediate(linkObject);
    }

    [Test]
    public void ExportDefaultJointData_DefaultJoint_Succeeds()
    {
        Vector3 position = new Vector3(1, 2, 3);
        Quaternion rotation = Quaternion.Euler(4, 5, 6);

        GameObject baseObject = new GameObject("base");
        GameObject linkObject = new GameObject("link");
        linkObject.transform.parent = baseObject.transform;
        linkObject.transform.position = position;
        linkObject.transform.rotation = rotation;

        Joint joint = UrdfJoint.ExportDefaultJoint(linkObject.transform);

        Assert.AreEqual("base_link_joint", joint.name);
        Assert.AreEqual("fixed", joint.type);
        Assert.AreEqual(baseObject.name, joint.parent);
        Assert.AreEqual(linkObject.name, joint.child);

        Assert.AreEqual(new double[] { position[2], -position[0], position[1] }, joint.origin.Xyz);
        UnityEngine.Assertions.Assert.AreApproximatelyEqual(-rotation.eulerAngles[2] * Mathf.Deg2Rad, (float)joint.origin.Rpy[0]);
        UnityEngine.Assertions.Assert.AreApproximatelyEqual(rotation.eulerAngles[0] * Mathf.Deg2Rad, (float)joint.origin.Rpy[1]);
        UnityEngine.Assertions.Assert.AreApproximatelyEqual(-rotation.eulerAngles[1] * Mathf.Deg2Rad, (float)joint.origin.Rpy[2]);

        Object.DestroyImmediate(baseObject);
        Object.DestroyImmediate(linkObject);
    }

    [Test]
    public void GenerateUniqueJointName_UniqueName_Succeeds()
    {
        GameObject baseObject = new GameObject("base");
        GameObject linkObject = new GameObject("link");
        linkObject.transform.parent = baseObject.transform;

        var joint = UrdfJoint.Create(linkObject, UrdfJoint.JointTypes.Revolute);

        Assert.IsNull(joint.jointName);
        joint.GenerateUniqueJointName();
        Assert.NotNull(joint.jointName);

        Object.DestroyImmediate(baseObject);
        Object.DestroyImmediate(linkObject);
    }

    [Test]
    public void GetAxisData_ArbitraryData_Succeeds()
    {
        Assert.AreEqual(new double[] { 1.234568, 2.345679, 3.456789 },
            TestUrdfJoint.AxisData(new Vector3(1.2345678f, 2.3456789f, 3.4567891f)).xyz);
    }
}
