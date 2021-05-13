using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using RosSharp;

public class Matrix3x3Tests
{
    [Test]
    public void Matrix3x3_Instantiations_All()
    {
        var zeroMatrix = new Matrix3x3(0f);

        Assert.AreEqual(zeroMatrix.elements, new Matrix3x3(new float[] {0, 0, 0}).elements);
        Assert.AreEqual(zeroMatrix.elements, new Matrix3x3(new float[] {0, 0, 0, 0, 0, 0}).elements);
        Assert.AreEqual(zeroMatrix.elements, new Matrix3x3(new float[] {0, 0, 0, 0, 0, 0, 0, 0, 0}).elements);
        Assert.AreEqual(zeroMatrix.elements, new Matrix3x3(new float[][] { new float[]{0, 0, 0}, new float[]{0, 0, 0}, new float[]{0, 0,00 }}).elements);
        Assert.AreEqual(zeroMatrix.elements, new Matrix3x3(new Vector3[] {Vector3.zero, Vector3.zero, Vector3.zero, Vector3.zero, Vector3.zero, Vector3.zero, Vector3.zero, Vector3.zero, Vector3.zero}).elements);
    }

    [Test]
    public void Member_Indexing()
    {
        var m = new Matrix3x3();
        Assert.IsNotNull(m[0]);
        Assert.IsNotNull(m[0] = new float[] {1, 1, 1});
    }

    [Test]
    public void OperatorOverrides_Arithmetic()
    {
        var zeroMatrix = new Matrix3x3(0f);
        var oneMatrix = new Matrix3x3(1f);
        var negOneMatrix = new Matrix3x3(-1f);

        // +
        Assert.AreEqual(zeroMatrix.elements, (oneMatrix + negOneMatrix).elements);
        Assert.AreEqual(zeroMatrix.elements, (negOneMatrix + 1f).elements);

        // -
        Assert.AreEqual(zeroMatrix.elements, (oneMatrix - oneMatrix).elements);
        Assert.AreEqual(zeroMatrix.elements, (oneMatrix - 1f).elements);

        // *        
        Assert.AreEqual(zeroMatrix.elements, (oneMatrix * zeroMatrix).elements);
        Assert.AreEqual(zeroMatrix.elements, (oneMatrix * 0f).elements);
        Assert.AreEqual(Vector3.one * 6f, (oneMatrix * new Vector3(1, 2, 3)));
    }

    [Test]
    public void MatrixOperations()
    {
        var identity = new Matrix3x3(new float[] {1f, 1f, 1f});
        var nonDiag = new Matrix3x3(new float[] {0f, 1f, 2f, 3f, 4f, 5f, 6f, 7f, 8f});
        var nonDiagT = new Matrix3x3(new float[] {0f, 3f, 6f, 1f, 4f, 7f, 2f, 5f, 8f});
        var eigenTest = new Matrix3x3(1f);
        var eigenValues = new Vector3(3f, 0f, 0f);
        var eigenVectors = new Vector3[] {Vector3.one * 0.6f, new Vector3(-0.4f, -0.4f, 0.8f), new Vector3(0.7f, -0.7f, 0)};

        // Determinant
        Assert.AreEqual(1f, identity.Determinant());

        // Trace
        Assert.AreEqual(3f, identity.Trace());

        // IsDiagonal
        Assert.IsTrue(identity.IsDiagonal());
        Assert.IsFalse(nonDiag.IsDiagonal());

        // Transpose
        Assert.AreEqual(identity.elements, identity.Transpose().elements);
        Assert.AreEqual(nonDiagT.elements, nonDiag.Transpose().elements);

        // DiagonalizeRealSymmetric
        // Identity
        Vector3 values;
        Vector3[] vectors;
        identity.DiagonalizeRealSymmetric(out values, out vectors);
        Assert.AreEqual(Vector3.one, values);
        Assert.AreEqual(new Vector3[] {new Vector3(1, 0, 0), new Vector3(0, 1, 0), new Vector3(0, 0, 1)}, vectors);

        // Non-identity
        Vector3 nonDiagValues;
        Vector3[] nonDiagVectors;
        eigenTest.DiagonalizeRealSymmetric(out nonDiagValues, out nonDiagVectors);
        Assert.IsTrue(Vector3.Equals(eigenValues, nonDiagValues));
        Assert.AreEqual(eigenVectors.Length, nonDiagVectors.Length);
        for (int i = 0; i < eigenVectors.Length; i++)
        {
            Assert.IsTrue(Vector3.Equals(eigenVectors[i], nonDiagVectors[i]));
        }
    }
}
