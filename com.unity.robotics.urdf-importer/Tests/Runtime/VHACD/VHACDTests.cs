using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using RosSharp.Urdf;
using MeshProcess;

public class VHACDTests
{
    [Test]
    public void GenerateConvexMeshes_Cylinder_NullInput()
    {
        // Create primitive cylinder with VHACD
        GameObject geometryGameObject = new GameObject("Cylinder");
        MeshFilter meshFilter = geometryGameObject.AddComponent<MeshFilter>();
        Link.Geometry.Cylinder cylinder = new Link.Geometry.Cylinder(0.5, 2); //Default unity cylinder sizes
        meshFilter.sharedMesh = UrdfGeometry.CreateCylinderMesh(cylinder);

        GameObject go = meshFilter.gameObject;
        VHACD decomposer = go.AddComponent<VHACD>();
        List<Mesh> colliderMeshes = decomposer.GenerateConvexMeshes(null);

        Assert.IsNotNull(meshFilter.sharedMesh);
        Assert.IsTrue(colliderMeshes.Count > 0);

        Component.DestroyImmediate(go.GetComponent<VHACD>());
        Object.DestroyImmediate(go.GetComponent<MeshRenderer>());
        Object.DestroyImmediate(meshFilter);
    }

    [Test]
    public void GenerateConvexMeshes_Cylinder_MeshInput()
    {
        // Create primitive cylinder with VHACD
        GameObject geometryGameObject = new GameObject("Cylinder");
        MeshFilter meshFilter = geometryGameObject.AddComponent<MeshFilter>();
        Link.Geometry.Cylinder cylinder = new Link.Geometry.Cylinder(0.5, 2); //Default unity cylinder sizes
        meshFilter.sharedMesh = UrdfGeometry.CreateCylinderMesh(cylinder);

        GameObject go = meshFilter.gameObject;
        VHACD decomposer = go.AddComponent<VHACD>();
        List<Mesh> colliderMeshes = decomposer.GenerateConvexMeshes(meshFilter.sharedMesh);

        Assert.IsNotNull(cylinder);
        Assert.IsNotNull(meshFilter.sharedMesh);
        Assert.IsTrue(colliderMeshes.Count > 0);

        Component.DestroyImmediate(go.GetComponent<VHACD>());
        Object.DestroyImmediate(go.GetComponent<MeshRenderer>());
        Object.DestroyImmediate(meshFilter);
    }
}
