using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Com.WovenPlanet.CodingTest {
    public class HelperFuncTest {
        [Test]
        public void TestCalcStateID() {
            Assert.AreEqual(
                expected: 0,
                actual: Calculations.GetStateID(
                    new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }
                    ));
            Assert.AreEqual(
                expected: -4580968,
                actual: Calculations.GetStateID(
                    new int[] { -1, -1, -1, 0, 1, 0, 1, -1, 1, 1, 0, 1, 0, 0, -1 }
                    ));
            Assert.AreEqual(
                expected: 7174453,
                actual: Calculations.GetStateID(
                    new int[] { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 }
                    )); ;
            Assert.AreEqual(
                expected: -7174453,
                actual: Calculations.GetStateID(
                    new int[] { -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 }
                    )); ;
        }

        [Test]
        public void TestFindInChildren() {
            GameObject testObject = Object.Instantiate(Resources.Load("Prefabs/Test/TestObject") as GameObject);

            Assert.That(Helpers.FindInChildren(testObject, "111"), Is.Not.Null);
            Assert.That(Helpers.FindInChildren(testObject, "XXX"), Is.Not.Null);
            Assert.That(Helpers.FindInChildren(testObject, "333"), Is.Null);
        }
    }
}