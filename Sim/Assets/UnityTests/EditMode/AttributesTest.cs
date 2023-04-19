using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Com.WovenPlanet.CodingTest {

    public class AttributesTest {

        [Test]
        public void TestPlayerType() {
            Assert.AreEqual(expected: 0, actual: (int)PlayerType.Human);
            Assert.AreEqual(expected: 1, actual: (int)PlayerType.AI);
        }

        [Test]
        public void TestPlayerTypeSize() {
            Assert.AreEqual(expected: 2, actual: Enum.GetNames(typeof(PlayerType)).Length);
        }

        [Test]
        public void TestBoardShapeHasHexagon() {
            Assert.IsTrue(Enum.IsDefined(typeof(BoardShape), "Hexagon"));
        }

        [Test]
        public void TestGameStateAttributes() {
            GameState gameState = new GameState();

            Assert.AreEqual(
                expected: new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                actual: gameState.currentState);

            Assert.AreEqual(
                expected: new HashSet<LineSegment>(),
                actual: gameState.unusedLineSegments
                );
        }
    }
}