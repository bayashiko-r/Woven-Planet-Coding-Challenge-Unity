using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.SceneManagement;
using Cysharp.Threading.Tasks;
using Random = System.Random;

namespace Com.WovenPlanet.CodingTest {
    public class AITest {
        SimController simController;
        GameState gameState;

        [SetUp]
        public void SetUp() {
            SceneManager.LoadScene("MainGame");
        }

        /// <summary>
        /// Test if the AI's choice is included in the expected choices.
        /// "ExpectedResult=null" does not have much meaning. 
        /// It is necessary as a workaround when we use testcases for play mode testing.
        /// </summary>
        /// <param name="chosenIndexByPlayer">The player chooses the line segment with this index.</param>
        /// <param name="possibleReturnIndexesByAI">AI's choice should be included in this array.</param>
        [UnityTest]
        [Timeout(20000)] /* ms */
        [TestCase(2, new int[] { 6, 9, 10, 11, 14 }, ExpectedResult=null)]
        [TestCase(10, new int[] { 0, 2, 5, 6, 8, 9, 11 }, ExpectedResult=null)]
        [TestCase(9, new int[] { 0, 2, 4, 5, 6, 7, 8, 13 }, ExpectedResult=null)]
        public IEnumerator TestAIChoise(int chosenIndexByPlayer, int[] possibleReturnIndexesByAI) {
            simController = GameObject.Find("Hexagon").GetComponent<SimController>();
            gameState = simController.GetGameState;

            PlayerChooseLineSegment(chosenIndexByPlayer);

            yield return new WaitForSeconds(12.0f);

            Assert.That(possibleReturnIndexesByAI,
                Has.Member(gameState.LastLineSegmentChosenByAI.Index));
            
            yield return null;
        }

        [UnityTest]
        [Timeout(20000)] /* ms */
        public IEnumerator TestAIThinkingText() {
            simController = GameObject.Find("Hexagon").GetComponent<SimController>();
            gameState = simController.GetGameState;
            GameObject canvas = GameObject.Find("Canvas");
            GameObject thinkingText = Helpers.FindInChildren(canvas, "Thinking Text");

            Random rnd = new Random();
            PlayerChooseLineSegment(rnd.Next(15));

            yield return new WaitForSeconds(Time.deltaTime);

            Assert.True(thinkingText.activeSelf);

            yield return null;
        }

        /// <summary>
        /// Simulate the operation that the player chooses a certain line segment.
        /// </summary>
        /// <param name="index">The index of the line segment the player chooses</param>
        private void PlayerChooseLineSegment(int index) {
            PolygonSide polygonSide = simController.LineSegments[index]
                    .GetLineRenderer.gameObject
                    .GetComponent<PolygonSide>();

            polygonSide.OnMouseOver();
            Input.GetMouseButtonDown(0);

            gameState.SetTurn(PlayerType.AI);
            _ = polygonSide.GetAIChoice();
        }
    }
}