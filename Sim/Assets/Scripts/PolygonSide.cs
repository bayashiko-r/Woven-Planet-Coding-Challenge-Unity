using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System;

namespace Com.WovenPlanet.CodingTest
{
    public class PolygonSide : MonoBehaviour
    {
        private LineRenderer lineRenderer = null;
        private LineSegment lineSegment = null;
        private GameState gameState = null;
        private Color onHoverColor;
        private Color originalColor;
        [SerializeField]
        private Color playerColor = Color.green;
        [SerializeField]
        private Color AIColor = Color.red;
        private bool isMousedOver = false;
        // Added
#if UNITY_INCLUDE_TESTS
        public bool IsMousedOver {
            set { isMousedOver = value; }
        }
#endif

        public void SetInternalState(LineRenderer inputLineRenderer, LineSegment inputLineSegment, GameState inputGameState, Color inputOnHoverColor)
        {
            lineRenderer = inputLineRenderer;
            lineSegment = inputLineSegment;
            gameState = inputGameState;
            onHoverColor = inputOnHoverColor;
            originalColor = lineRenderer.startColor;
        }
        public void OnMouseOver()
        {
            if (gameState.IsPlayerTurn(PlayerType.Human) && !gameState.LineSegmentAlreadyTaken(lineSegment) && !gameState.HasPlayerLost(PlayerType.Human))
            {
                isMousedOver = true;
                lineRenderer.material.color = onHoverColor;
                lineRenderer.startColor = onHoverColor;
                lineRenderer.endColor = onHoverColor;
            }

        }
        public void OnMouseExit()
        {
            isMousedOver = false;
            if (gameState.IsPlayerTurn(PlayerType.Human) && !gameState.LineSegmentAlreadyTaken(lineSegment) && !gameState.HasPlayerLost(PlayerType.Human))
            {
                lineRenderer.material.color = originalColor;
                lineRenderer.startColor = originalColor;
                lineRenderer.endColor = originalColor;
            }
        }

        void Update()
        {
            if (gameState != null && !gameState.HasPlayerLost(PlayerType.Human))
            {
                if (gameState.IsPlayerTurn(PlayerType.Human) && isMousedOver && Input.GetMouseButtonDown(0))
                {

                    lineRenderer.material.color = playerColor;
                    lineRenderer.startColor = playerColor;
                    lineRenderer.endColor = playerColor;
                    gameState.AddLineSegmentToPlayer(gameState.currentState, PlayerType.Human, lineSegment, gameState.unusedLineSegments);
                    // Added
                    Debug.Log($"<color=green>Human has chosen {lineSegment.Index}</color>");
                    if (gameState.HasPlayerFormedLosingTriangle(PlayerType.Human))
                    {
                        gameState.SetPlayerLost(PlayerType.Human);
                    }
                    else
                    {
                        gameState.SetTurn(PlayerType.AI);
                        _ = GetAIChoice();
                    }
                }
            }
        }

        // Added
        public async UniTaskVoid GetAIChoice() {
            try {
                LineSegment chosenLineSegmentByAI;
                chosenLineSegmentByAI = await UniTask.RunOnThreadPool(() => gameState.TakeAITurn());
                LineRenderer chosenLineRendererByAI = chosenLineSegmentByAI.GetLineRenderer;
                chosenLineRendererByAI.material.color = AIColor;
                chosenLineRendererByAI.startColor = AIColor;
                chosenLineRendererByAI.endColor = AIColor;
                gameState.AddLineSegmentToPlayer(gameState.currentState, PlayerType.AI, chosenLineSegmentByAI, gameState.unusedLineSegments);

                if (gameState.HasPlayerFormedLosingTriangle(PlayerType.AI)) {
                    gameState.SetPlayerLost(PlayerType.AI);
                } else {
                    gameState.SetTurn(PlayerType.Human);
                }
            } catch (Exception ex){
                Debug.LogWarning(ex);
            }
        }
    }
}
