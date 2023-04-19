using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Com.WovenPlanet.CodingTest {
    public class MappingTest {
        int[] testArray1 = new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        int[] testArray2 = new int[] { -1, -1, -1, 0, 1, 0, 1, -1, 1, 1, 0, 1, -1, 0, -1 };

        [Test]
        public void TestRotateMapping() {
            CollectionAssert.AreEqual(
                expected: new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                actual: Calculations.Map(testArray1, Calculations.rotateMapping)
                );

            CollectionAssert.AreEqual(
                expected: new int[] { 0, 1, -1, 1, -1, 1, 0, 1, -1, -1, 0, -1, -1, 0, 1 },
                actual: Calculations.Map(testArray2, Calculations.rotateMapping)
                );
        }

        [Test]
        public void TestNoMapping() {
            CollectionAssert.AreEqual(
                expected: new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                actual: Calculations.Map(testArray1, Calculations.noMapping)
                );

            CollectionAssert.AreEqual(
                expected: new int[] { -1, -1, -1, 0, 1, 0, 1, -1, 1, 1, 0, 1, -1, 0, -1 },
                actual: Calculations.Map(testArray2, Calculations.noMapping)
                );
        }

        [Test]
        public void TestMirrorMappingA() {
            CollectionAssert.AreEqual(
                expected: new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                actual: Calculations.Map(testArray1, Calculations.mirrorMappingA)
                );

            CollectionAssert.AreEqual(
                expected: new int[] { 1, 0, -1, -1, -1, -1, 0, 1, 1, -1, 0, -1, 1, 1, 0 },
                actual: Calculations.Map(testArray2, Calculations.mirrorMappingA)
                );
        }

        [Test]
        public void TestMirrorMappingB() {
            CollectionAssert.AreEqual(
                expected: new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                actual: Calculations.Map(testArray1, Calculations.mirrorMappingB)
                );

            CollectionAssert.AreEqual(
                expected: new int[] { 0, -1, 1, 0, 1, -1, 1, -1, 1, 1, 0, -1, -1, 0, -1 },
                actual: Calculations.Map(testArray2, Calculations.mirrorMappingB)
                );
        }

        [Test]
        public void TestMirrorMappingC() {
            CollectionAssert.AreEqual(
                expected: new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                actual: Calculations.Map(testArray1, Calculations.mirrorMappingC)
                );

            CollectionAssert.AreEqual(
                expected: new int[] { -1, 0, -1, 0, -1, 1, 1, -1, 0, 0, -1, 1, -1, 1, 1 },
                actual: Calculations.Map(testArray2, Calculations.mirrorMappingC)
                );
        }
    }
}