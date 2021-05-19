using NUnit.Framework;
using UnityEngine;
using RosSharp.Urdf;

public class UrdfJointContinuousTests
{
    [Test]
    public void CreateTest()
    {
        GameObject linkObject = new GameObject("link");
        UrdfJoint joint = UrdfJointContinuous.Create(linkObject);
        UrdfJoint urdfJoint = linkObject.GetComponent<UrdfJoint>();
        ArticulationBody articulationBody = linkObject.GetComponent<ArticulationBody>();

        Assert.IsTrue(joint is UrdfJointContinuous);
        Assert.AreEqual(joint, urdfJoint);
        Assert.AreEqual(ArticulationJointType.RevoluteJoint, articulationBody.jointType);

        Object.DestroyImmediate(linkObject);
    }

    [Test]
    public void GetPositionTest()
    {
        GameObject baseObject = new GameObject("base");
        GameObject linkObject = new GameObject("link");
        linkObject.transform.parent = baseObject.transform;

        UrdfJoint.Create(baseObject, UrdfJoint.JointTypes.Fixed);
        UrdfJoint joint = UrdfJointContinuous.Create(linkObject);
        ArticulationBody articulationBody = linkObject.GetComponent<ArticulationBody>();
        articulationBody.jointPosition = new ArticulationReducedSpace(1, 2, 3);

        Assert.AreEqual(1, joint.GetPosition());
    }
}
