using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using RosSharp.Urdf;

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
}
