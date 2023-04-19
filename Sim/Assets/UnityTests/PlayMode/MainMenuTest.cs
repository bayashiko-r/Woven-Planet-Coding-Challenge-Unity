using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.TestTools;
using UnityEngine.SceneManagement;

namespace Com.WovenPlanet.CodingTest {
    public class MainMenuTest {
        [OneTimeSetUp]
        public void SetUp() {
            SceneManager.LoadScene("MainMenuScene");
        }

        [UnityTest]
        public IEnumerator TestObjectsState() {
            GameObject menuObject = GameObject.Find("MenuCanvas");
            Assert.That(menuObject, Is.Not.Null);

            GameObject playButton = Helpers.FindInChildren(menuObject, "PlayButton");
            Assert.That(playButton, Is.Not.Null);
            Assert.True(playButton.activeSelf);

            GameObject exitButton = Helpers.FindInChildren(menuObject, "ExitButton");
            Assert.That(exitButton, Is.Not.Null);
            Assert.True(exitButton.activeSelf);

            yield return null;
        }

        [UnityTest]
        public IEnumerator TestPlayButton() {
            GameObject menuObject = GameObject.Find("MenuCanvas");
            GameObject playButton = Helpers.FindInChildren(menuObject, "PlayButton");

            playButton.GetComponent<Button>().onClick.Invoke();

            yield return new WaitForSeconds(Time.deltaTime);

            Assert.AreEqual(
                expected: "MainGame",
                actual: SceneManager.GetActiveScene().name
                );
            
            yield return null;
        }

        [UnityTest]
        public IEnumerator TestExitButton() {
            GameObject menuObject = GameObject.Find("MenuCanvas");
            GameObject exitButton = Helpers.FindInChildren(menuObject, "ExitButton");

            exitButton.GetComponent<Button>().onClick.Invoke();
            LogAssert.Expect(LogType.Log, "quit");

            yield return null;
        }
    }
}
