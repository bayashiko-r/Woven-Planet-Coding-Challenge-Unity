using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

namespace Com.WovenPlanet.CodingTest
{
    public class SimController : MonoBehaviour {
        [Range(20f, 200f)]
        public float hexagonRadius = 200;
        private GameState gameState = new GameState();
        private GameObject[] hexagonLines = null;
        private LineSegment[] lineSegments = null;
        // Added
        public LineSegment[] LineSegments {
            get => lineSegments;
        }

        [SerializeField]
        private GameObject loseText = null;
        [SerializeField]
        private GameObject winText = null;
        // Added
        [SerializeField]
        private GameObject thinkingText = null;

        [SerializeField]
        [Range(0.001f, 20f)]
        private float lineWidth = 10f;
        [SerializeField]
        private Color outerEdgeColor = Color.black;
        [SerializeField]
        private Color innerEdgeColor = Color.grey;
        [SerializeField]
        private Color onHoverColor = Color.green;
        [SerializeField]
        private GameObject canvas = null;

        //Added
#if UNITY_INCLUDE_TESTS
        // Accessors for testing purpose
        public GameObject LoseText {
            get => loseText;
        }
        public GameObject WinText {
            get => winText;
        }
        public GameObject Canvas {
            get => canvas;
        }
        public GameState GetGameState {
            get => gameState;
        }
#endif
        private float canvasWidth;
        private float canvasHeight;
        void Awake()
        {
            loseText.SetActive(false);
            winText.SetActive(false);
            // Added
            thinkingText.SetActive(false);

            canvasWidth = canvas.GetComponent<RectTransform>().rect.width;
            canvasHeight = canvas.GetComponent<RectTransform>().rect.height;

            transform.localScale = new Vector3(canvasWidth, canvasHeight, 1);

            gameState.SetTurn(PlayerType.Human);
            DrawHexagon(hexagonRadius);
        }
        void DrawHexagon(float radius)
        {
            lineSegments = Hexagon.GenerateLineSegments(radius);
            float zOffset = -10;
            hexagonLines = lineSegments.Select((lineSegment) => {
                GameObject hexagonLine = new GameObject("HexagonLine");
                hexagonLine.transform.position = transform.position;
                LineRenderer lineRenderer = hexagonLine.AddComponent<LineRenderer>();
                lineRenderer.positionCount = 2;
                lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
                if (lineSegment.IsOuterEdge)
                {
                    lineRenderer.material.color = outerEdgeColor;
                    lineRenderer.startColor = outerEdgeColor;
                    lineRenderer.endColor = outerEdgeColor;
                }
                else
                {
                    lineRenderer.material.color = innerEdgeColor;
                    lineRenderer.startColor = innerEdgeColor;
                    lineRenderer.endColor = innerEdgeColor;
                }
                lineRenderer.sortingLayerName = "OnTop";
                lineRenderer.sortingOrder = 5;
                Vector2[] lineSegmentVerticies = lineSegment.GetLineSegment();
                Vector2 startPosition = lineSegmentVerticies[0].y > lineSegmentVerticies[1].y ? lineSegmentVerticies[1] : lineSegmentVerticies[0];
                Vector2 endPosition = lineSegmentVerticies[0].y > lineSegmentVerticies[1].y ? lineSegmentVerticies[0] : lineSegmentVerticies[1];
                lineRenderer.SetPosition(0, new Vector3(startPosition.x, startPosition.y, zOffset));
                lineRenderer.SetPosition(1, new Vector3(endPosition.x, endPosition.y, zOffset));
                lineRenderer.widthMultiplier = lineWidth;
                // Added
                lineSegment.SetLineRenderer(lineRenderer);

                BoxCollider boxCollider = hexagonLine.AddComponent<BoxCollider>();
                boxCollider.transform.parent = lineRenderer.transform;
                boxCollider.center = new Vector3(0, 0, zOffset);
                float lineLength = Vector2.Distance(startPosition, endPosition);
                boxCollider.size = new Vector3(lineLength, lineWidth, 1f);
                Vector3 midPoint = (Vector3)((startPosition + endPosition) / 2);
                boxCollider.transform.position = midPoint;
                float angle = (Mathf.Abs(startPosition.y - endPosition.y) / Mathf.Abs(startPosition.x - endPosition.x));
                if ((startPosition.y < endPosition.y && startPosition.x > endPosition.x) || (endPosition.y < startPosition.y && endPosition.x > startPosition.y))
                {
                    angle *= -1;
                }
                angle = Mathf.Rad2Deg * Mathf.Atan(angle);
                boxCollider.transform.Rotate(0, 0, angle);
                hexagonLine.AddComponent<PolygonSide>();
                PolygonSide polygonSide = hexagonLine.GetComponent<PolygonSide>();
                polygonSide.SetInternalState(lineRenderer, lineSegment, gameState, onHoverColor);
                return hexagonLine;
            }).ToArray();
            // Added
            gameState.SetLineSegments(lineSegments);
        }

        // Update is called once per frame
        void Update()
        {
            thinkingText.SetActive(gameState.GetPlayerState[(int)PlayerType.AI].IsTurn);
            if (gameState.HasPlayerLost(PlayerType.Human) && !loseText.activeSelf)
            {
                loseText.SetActive(true);
            }
            if (gameState.HasPlayerLost(PlayerType.AI) && !winText.activeSelf)
            {
                winText.SetActive(true);
            }
        }
    }
}
