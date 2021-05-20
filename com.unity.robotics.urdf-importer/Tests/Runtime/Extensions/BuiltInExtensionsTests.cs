using System.IO;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Object = UnityEngine.Object;

namespace RosSharp.Tests
{
    public class BuiltInExtensionsTests
    {
        [Test]
        public void DestroyImmediateIfExists_RemoveBoxCollider_Success() 
        {
            var t = new GameObject().transform;
            t.gameObject.AddComponent<BoxCollider>();
            t.DestroyImmediateIfExists<BoxCollider>();

            Assert.IsNull(t.GetComponent<BoxCollider>());
            Assert.IsNotNull(t);

            Object.DestroyImmediate(t.gameObject);
        }

        [Test]
        public void AddComponentIfNotExists_AddBoxCollider_Success()
        {
            var t = new GameObject().transform;
            t.AddComponentIfNotExists<BoxCollider>();

            Assert.IsNotNull(t.GetComponent<BoxCollider>());

            Object.DestroyImmediate(t.gameObject);
        }

        [Test]
        public void SetParentAndAlign_KeepLocalTransform_Success()
        {
            var parent = new GameObject("Parent").transform;
            var child = new GameObject("Child").transform;
            child.localPosition = Vector3.one;
            child.localRotation = Quaternion.Euler(0, 30, 0);
            child.SetParentAndAlign(parent, keepLocalTransform: true);

            Assert.AreEqual(1, parent.childCount);
            Assert.AreEqual(child, parent.GetChild(0));
            Assert.AreEqual(Vector3.one, parent.GetChild(0).localPosition);
            Assert.IsTrue(Quaternion.Euler(0, 30, 0) == parent.GetChild(0).localRotation);

            Object.DestroyImmediate(parent.gameObject);
        }

        [Test]
        public void SetParentAndAlign_IgnoreLocalTransform_Success()
        {
            var parent = new GameObject("Parent").transform;
            var child = new GameObject("Child").transform;
            child.localPosition = Vector3.one;
            child.localRotation = Quaternion.Euler(0, 30, 0);
            child.SetParentAndAlign(parent, keepLocalTransform: false);

            Assert.AreEqual(1, parent.childCount);
            Assert.AreEqual(child, parent.GetChild(0));
            Assert.AreEqual(Vector3.zero, parent.GetChild(0).localPosition);
            Assert.AreEqual(Quaternion.identity, parent.GetChild(0).localRotation);

            Object.DestroyImmediate(parent.gameObject);
        }

        [Test]
        public void HasExactlyOneChild_EmptyTransforms_Success()
        {
            var parent = new GameObject("Parent").transform;
            var child0 = new GameObject("Child0").transform;
            var child1 = new GameObject("Child1").transform;

            Assert.IsFalse(parent.HasExactlyOneChild());

            child0.parent = parent;
            Assert.IsTrue(parent.HasExactlyOneChild());

            child1.parent = parent;
            Assert.IsFalse(parent.HasExactlyOneChild());

            Object.DestroyImmediate(parent.gameObject);
        }

        [Test]
        public void MoveChildTransformToParent_TransformRotation_Success()
        {
            var parent = new GameObject("Parent").transform;
            var child = new GameObject("Child").transform;
            child.localRotation = Quaternion.Euler(0, 30, 0);
            child.localPosition = Vector3.one;
            child.localScale = Vector3.one * 2f;
            child.parent = parent;
            parent.MoveChildTransformToParent(transferRotation: true);

            child = parent.GetChild(0);
            Assert.AreEqual(Vector3.zero, child.localPosition);
            Assert.AreEqual(Vector3.one, parent.localPosition);
            Assert.AreEqual(Vector3.one, child.localScale);
            Assert.AreEqual(Vector3.one * 2f, parent.localScale);
            Assert.IsTrue(Quaternion.identity == child.localRotation);
            Assert.IsTrue(Quaternion.Euler(0, 30, 0) == parent.localRotation);

            Object.DestroyImmediate(parent.gameObject);
        }

        [Test]
        public void ToFromRosTests_NonIdentityValues_Success()
        {
            var testVector = new Vector3(1, 2, 3);
            var testQuaternion = new Quaternion(0.1f, 0.2f, 0.3f, 0.4f);

            Assert.AreEqual(new double[] {-0.05236, 0.017453, -0.034907}, testVector.ToRosRPY());
            Assert.AreEqual(new Vector3(-2, 3, 1), testVector.Ros2Unity());
            Assert.AreEqual(new Vector3(3, -1, 2), testVector.Unity2Ros());
            Assert.AreEqual(new Vector3(2, 3, 1), testVector.Ros2UnityScale());
            Assert.AreEqual(new Vector3(3, 1, 2), testVector.Unity2RosScale());

            Assert.AreEqual(new Quaternion(0.2f, -0.3f, -0.1f, 0.4f), testQuaternion.Ros2Unity());
            Assert.AreEqual(new Quaternion(-0.3f, 0.1f, -0.2f, 0.4f), testQuaternion.Unity2Ros());
        }

        [Test]
        public void ConvertTypeTests_IdentityValues_Success()
        {
            var doubleOne = new double[] {1,1,1};
            Assert.AreEqual(doubleOne, Vector3.one.ToRoundedDoubleArray());
            Assert.AreEqual(Vector3.one, doubleOne.ToVector3());
        }

        [Test]
        public void SetSeparatorChar_AltToDefault_Success()
        {
            var altPath = $"Path{Path.AltDirectorySeparatorChar}To{Path.AltDirectorySeparatorChar}Directory";
            Assert.AreEqual("Path/To/Directory", altPath.SetSeparatorChar());
        }

        static IEnumerable<TestCaseData> DoubleArrayData
        {
            get
            {
                yield return new TestCaseData(new double[] {1,1,1}, new double[] {1,1,1,1}).Returns(false);
                yield return new TestCaseData(new double[] {1,1,1}, new double[] {1,1,1}).Returns(true);
                yield return new TestCaseData(new double[] {0,0,0}, new double[] {1,1,1}).Returns(false);
            }
        }

        [Test, TestCaseSource("DoubleArrayData")]
        public bool DoubleDeltaCompare_DeltaE4_Success(double[] array, double[] array2)
        {
            return array.DoubleDeltaCompare(array2, 1e-4);
        }

        static IEnumerable<TestCaseData> Vector3Data
        {
            get
            {
                yield return new TestCaseData(Vector3.zero, Vector3.one).Returns(false);
                yield return new TestCaseData(Vector3.one, Vector3.one).Returns(true);
            }
        }    

        [Test, TestCaseSource("Vector3Data")]
        public bool VectorEqualDelta_DeltaE4_Success(Vector3 source, Vector3 exported)
        {
            return source.VectorEqualDelta(exported, 1e-4);
        }

        static IEnumerable<TestCaseData> DoubleData
        {
            get
            {
                yield return new TestCaseData(0, 0, 1e-4).Returns(true);
                yield return new TestCaseData(0, 1, 1e-4).Returns(false);
                yield return new TestCaseData(0, 0.001, 1e-4).Returns(false);
                yield return new TestCaseData(0, 0.0001, 1e-2).Returns(true);
            }
        }  

        [Test, TestCaseSource("DoubleData")]
        public bool EqualsDelta_ModifiedDelta_Success(double first, double second, double delta)
        {
            return first.EqualsDelta(second, delta);
        }

        static IEnumerable<TestCaseData> Matrix4x4SubtractData
        {
            get
            {
                yield return new TestCaseData(Matrix4x4.identity, Matrix4x4.identity).Returns(Matrix4x4.zero);
                yield return new TestCaseData(Matrix4x4.zero, Matrix4x4.zero).Returns(Matrix4x4.zero);
            }
        }  

        [Test, TestCaseSource("Matrix4x4SubtractData")]
        public Matrix4x4 Subtract_Matrix4x4s_Success(Matrix4x4 first, Matrix4x4 second)
        {
            return first.Subtract(second);
        }

        [Test]
        public void FloatDivide_Identities_Success()
        {
            Assert.AreEqual(Matrix4x4.identity, Matrix4x4.identity.FloatDivide(1));
            Assert.AreEqual(Matrix4x4.zero, Matrix4x4.zero.FloatDivide(1));
        }

        [Test]
        public void FirstChildByQuery_Components_ReturnFirst()
        {
            var parent = new GameObject("Parent").transform;

            Assert.IsNull(parent.FirstChildByQuery(x => x.name.Contains("Child")));

            var child00 = new GameObject("Child00").transform;
            var child01 = new GameObject("Child01").transform;
            var child10 = new GameObject("Child10").transform;
            child10.gameObject.AddComponent<BoxCollider>();
            child10.parent = child00;
            child01.parent = parent;
            child00.parent = parent;

            Assert.AreEqual(child01, parent.FirstChildByQuery(x => x.name.Contains("Child")));
            Assert.AreEqual(child10, parent.FirstChildByQuery(x => x.GetComponent<BoxCollider>() != null));

            child01.gameObject.AddComponent<BoxCollider>();
            Assert.AreEqual(child01, parent.FirstChildByQuery(x => x.GetComponent<BoxCollider>() != null));

            Object.DestroyImmediate(parent.gameObject);
        }
    }
}
