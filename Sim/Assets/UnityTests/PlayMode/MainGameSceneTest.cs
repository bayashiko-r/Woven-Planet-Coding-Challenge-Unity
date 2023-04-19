using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.SceneManagement;

namespace Com.WovenPlanet.CodingTest {
    public class MainGameSceneTest {

        [OneTimeSetUp]
        public void SetUp() {
            SceneManager.LoadScene("MainGame");
        }

        [UnityTest]
        public IEnumerator TestObjectsState() {
            GameObject hexagon = GameObject.Find("Hexagon");
            Assert.That(hexagon, Is.Not.Null);

            GameObject canvas = GameObject.Find("Canvas");
            Assert.That(canvas, Is.Not.Null);

            GameObject loseText = Helpers.FindInChildren(canvas, "Lose Text");
            Assert.That(loseText, Is.Not.Null);
            Assert.False(loseText.activeSelf);

            GameObject winText = Helpers.FindInChildren(canvas, "Win Text");
            Assert.That(winText, Is.Not.Null);
            Assert.False(winText.activeSelf);

            GameObject thinkingText = Helpers.FindInChildren(canvas, "Thinking Text");
            Assert.That(winText, Is.Not.Null);
            Assert.False(winText.activeSelf);

            yield return null;
        }

        [UnityTest]
        public IEnumerator TestSimControllerAttributes() {
            GameObject hexagon = GameObject.Find("Hexagon");
            SimController simController = hexagon.GetComponent<SimController>();
            Assert.That(simController.LineSegments, Is.Not.Null);
            Assert.AreEqual(simController.LineSegments.Length, 15);

            yield return null;
        }
    }
}
