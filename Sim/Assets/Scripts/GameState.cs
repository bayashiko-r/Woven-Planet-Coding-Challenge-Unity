#nullable enable
using System;
using System.Linq;
using System.Collections.Generic;
using Com.WovenPlanet.CodingTest;
using UnityEngine;
using System.Diagnostics;
using UnityEngine.SceneManagement;
using Debug = UnityEngine.Debug;

namespace Com.WovenPlanet.CodingTest {
    public enum BoardShape {
        Hexagon = 6
    };
    public class LineSegment
    {
        private Vector2? startingPoint = null;
        private Vector2? endPoint = null;
        private Vector2 zeroVector = new Vector2(0, 0);
        private bool isOuterEdge;
        // Added
        private LineRenderer? lineRenderer;
        public LineRenderer? GetLineRenderer {
            get => lineRenderer;
        }
        public void SetLineRenderer(LineRenderer generatedLineRenderer) {
            lineRenderer = generatedLineRenderer;
        }
        public int Index { get; set; } = 0;

        public LineSegment(Vector2 inputStartingPoint, Vector2 inputEndingPoint, bool isOuterEdge)
        {
            SetLineSegment(inputStartingPoint, inputEndingPoint, isOuterEdge);
        }
        public void SetLineSegment(Vector2 inputStartingPoint, Vector2 inputEndPoint, bool inputIsOuterEdge)
        {
            if (startingPoint == null && endPoint == null)
            {
                startingPoint = inputStartingPoint;
                endPoint = inputEndPoint;
                isOuterEdge = inputIsOuterEdge;
            }
        }
        public Vector2[] GetLineSegment()
        {
            return new Vector2[]{startingPoint ?? zeroVector, endPoint ?? zeroVector};
        }
        public Vector2? DoesLineSegmentShareVertex(LineSegment otherLineSegment)
        {
            Vector2[] verticies = otherLineSegment.GetLineSegment();
            if (verticies != null)
            {
                if (this.startingPoint == verticies[0] || this.startingPoint == verticies[1])
                {
                    return this.startingPoint;
                }
                if (this.endPoint == verticies[0] || this.endPoint == verticies[1])
                {
                    return this.endPoint;
                }
            }
            return null;
        }
        public Vector2? GetOtherVertex(Vector2 vertex)
        {
            if (this.startingPoint == vertex)
            {
                return this.endPoint;
            }
            if (this.endPoint == vertex)
            {
                return this.startingPoint;
            }
            return null;
        }
        public bool IsNull()
        {
            if (startingPoint == null || endPoint == null)
            {
                return true;
            }
            return false;
        }
        public bool IsOuterEdge
        {
            get { return isOuterEdge; }
        }
        public override bool Equals(System.Object obj)
        {
            if ((obj == null) || !this.GetType().Equals(obj.GetType()))
            {
                return false;
            }
            #pragma warning disable CS8600
            #pragma warning disable CS8602
            LineSegment otherLineSegment = obj as LineSegment;
            if (startingPoint == null || endPoint == null) {
                return false;
            }
            if (otherLineSegment.IsNull()) {
                return false;
            }
            #pragma warning restore CS8600
            #pragma warning restore CS8602
            Vector2[] vectorsFromLineSegment = otherLineSegment.GetLineSegment();
            Vector2 otherStartingPoint = vectorsFromLineSegment[0];
            Vector2 otherEndPoint = vectorsFromLineSegment[1];
            if (startingPoint == otherStartingPoint && endPoint == otherEndPoint)
            {
                return true;
            }
            if (startingPoint == otherEndPoint && endPoint == otherStartingPoint)
            {
                return true;
            }
            return false;
        }
        public override int GetHashCode()
        {
            Vector2 actualStartingPoint = startingPoint ?? zeroVector;
            Vector2 actualEndPoint = endPoint ?? zeroVector;
            if (actualStartingPoint == zeroVector || actualEndPoint == zeroVector)
            {
                return 0;
            }
            if (actualStartingPoint.x > actualEndPoint.x)
            {
                return (actualStartingPoint.ToString() + actualEndPoint.ToString()).GetHashCode();
            }
            return (actualEndPoint.ToString() + actualStartingPoint.ToString()).GetHashCode();
        }
        public override string ToString()
        {
            return "start point: " + startingPoint.ToString() + "end point: " + endPoint.ToString();
        }
    }
    public class Hexagon
    {
        const int sides = 6;
        const float degreesToRadians = 180f / Mathf.PI;
        static private float sin60 = Mathf.Sin(60f * degreesToRadians);
        const float cos60 = 0.5f;
        const float normalizedRadius = 1f;
        static float normalizedSin60 = normalizedRadius * sin60;
        static float normalizedCos60 = normalizedRadius * cos60;
        static Vector2[] normalVerticies = new Vector2[sides]
        {
            new Vector2( normalizedRadius, 0.0f),
            new Vector2( normalizedCos60, normalizedSin60),
            new Vector2(-normalizedCos60, normalizedSin60),
            new Vector2(-normalizedRadius, 0.0f),
            new Vector2(-normalizedCos60,-normalizedSin60),
            new Vector2( normalizedCos60,-normalizedSin60)
        };
        static Vector2[] CreateVerticiesWithRadius(float radius)
        {
            return normalVerticies.Select(normalVertex => {
                return new Vector2(radius * normalVertex[0], radius * normalVertex[1]);
            }).ToArray();
        }
        static public LineSegment[] GenerateLineSegments(float radius)
        {
            Vector2[] verticies = CreateVerticiesWithRadius(radius);
            // Added
            int segmentIdx = 0;
            return Enumerable.Range(0, sides - 1).Aggregate(new LineSegment[] {},
                (accumulator, firstVertexIndex) =>
                {
                    bool isFirst = true;
                    LineSegment[] newLineSegments = Enumerable.Range(firstVertexIndex + 1, sides - firstVertexIndex - 1 ).Select(secondVertexIndex =>
                    {
                        // Modified the logic: Found a bug here.
                        // Explanation: The lineSegment with the index 4 was not marked as outer edge.
                        // Added: (segmentIdx == sides - 2)
                        LineSegment newLineSegment = new LineSegment(verticies[firstVertexIndex], verticies[secondVertexIndex], isFirst || (segmentIdx == sides - 2));
                        if (isFirst)
                        {
                            isFirst = false;
                        }
                        // Added
                        newLineSegment.Index = segmentIdx++;
                        return newLineSegment;
                    }).ToArray();
                    accumulator = accumulator.Concat(newLineSegments).ToArray();
                    return accumulator;
                }
            );
        }
    }
    public enum PlayerType {
        Human,
        AI
    }
    public class PlayerState {
        public PlayerState(PlayerType inputPlayerType)
        {
            playerType = inputPlayerType;
        }
        private PlayerType playerType;
        public PlayerType PlayerType {
            get => playerType;
        }
        bool isTurn = false;
        bool hasLost = false;
        public bool IsTurn {
            get => isTurn;
            set => isTurn = value;
        }
        public bool HasLost {
            get => hasLost;
            set => hasLost = value;
        }
        private LineSegment[] lineSegments = new LineSegment[]{};
        public void AddLineSegment(LineSegment newLineSegment)
        {
            lineSegments = lineSegments.Append(newLineSegment).ToArray();
        }
        // Added
        public void RemoveLineSegment(LineSegment removingLineSegment) {
            lineSegments = lineSegments.Where(element => element != removingLineSegment).ToArray();
        }
        public bool DoesOwnLineSegment(LineSegment lineSegmentToCheck)
        {
            return lineSegments.Any(lineSegment => lineSegment == lineSegmentToCheck);
        }
        public bool DoLineSegmentsFormTriangle()
        {
            if (lineSegments.Length < 3)
            {
                return false;
            }
            LineSegment[] remainingLineSegments = new LineSegment[lineSegments.Length];
            lineSegments.CopyTo(remainingLineSegments, 0);
            return lineSegments.Any(lineSegment => 
            {
                if (remainingLineSegments.Length < 2)
                {
                    return false;
                }
                remainingLineSegments = remainingLineSegments.Skip(1).ToArray();
                LineSegment[] sharedLineSegments = remainingLineSegments.Aggregate(new LineSegment[0] {}, (accumulator, lineSegmentToCompare) => {
                    if(lineSegment.DoesLineSegmentShareVertex(lineSegmentToCompare) != null)
                    {
                        accumulator = accumulator.Concat(new LineSegment[1] { lineSegmentToCompare }).ToArray();
                    }
                    return accumulator;
                });
                if (sharedLineSegments.Length < 2)
                {
                    return false;
                }
                return sharedLineSegments.Any(sharedLineSegment =>
                {
                    Vector2 impossiblePoint = new Vector2(0, 0);
                    Vector2 sharedVertex = lineSegment.DoesLineSegmentShareVertex(sharedLineSegment) ?? impossiblePoint;
                    Vector2 connectingVertex0 = lineSegment.GetOtherVertex(sharedVertex) ?? impossiblePoint;
                    Vector2 connectingVertex1 = sharedLineSegment.GetOtherVertex(sharedVertex) ?? impossiblePoint;
                    LineSegment possibleConnectingLineSegment = new LineSegment(connectingVertex0, connectingVertex1, false);
                    return sharedLineSegments.Any(lineSegmentToCheck => lineSegmentToCheck.Equals(possibleConnectingLineSegment));
                });
            });
        }
    }
    public class GameState {
        const int numberOfPlayers = 2;
        // Added
        const int numberOfLines = (int)BoardShape.Hexagon * ((int)BoardShape.Hexagon - 1) / 2;

        private PlayerState[] players = createEmptyPlayers();
        private static PlayerState[] createEmptyPlayers() {
            return new PlayerState[numberOfPlayers]
            {
                new PlayerState(PlayerType.Human),
                new PlayerState(PlayerType.AI)
            };
        }
        // Added
        public PlayerState[] GetPlayerState {
            get => players;
        }
        private LineSegment[] lineSegments = new LineSegment[] { };

        public void ResetGameState() {
            players = createEmptyPlayers();
            // Added
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
        private LineSegment? lastLineSegmentChosenByAI = null;
#if UNITY_INCLUDE_TESTS
        public LineSegment? LastLineSegmentChosenByAI {
            get => lastLineSegmentChosenByAI;
        }
#endif

        public HashSet<LineSegment> unusedLineSegments = new HashSet<LineSegment>();
        private Dictionary<int, Tuple<int, LineSegment>> statesMemo = new Dictionary<int, Tuple<int, LineSegment>>();
        public int[] currentState = new int[numberOfLines];

        public void SetTurn(PlayerType playerType) {
            PlayerType otherPlayer;
            if (playerType == PlayerType.AI) {
                otherPlayer = PlayerType.Human;
            } else {
                otherPlayer = PlayerType.AI;
            }
            if (players[(int)playerType].IsTurn) {
                return;
            } else {
                if (players[(int)otherPlayer].IsTurn) {
                    players[(int)otherPlayer].IsTurn = false;
                }
                players[(int)playerType].IsTurn = true;
            }
        }
        public bool IsPlayerTurn(PlayerType playerType) {
            return players[(int)playerType].IsTurn;
        }
        public void AddLineSegmentToPlayer(int[] currentState, PlayerType playerType, LineSegment lineSegment, HashSet<LineSegment> unusedLineSegments) {
            players[(int)playerType].AddLineSegment(lineSegment);
            // Added
            unusedLineSegments.Remove(lineSegment);
            currentState[lineSegment.Index] = (playerType == PlayerType.AI) ? 1 : -1;
        }
        // Added
        public void RemoveLineSegmentFromPlayer(int[] currentState, PlayerType playerType, LineSegment lineSegment, HashSet<LineSegment> unusedLineSegments) {
            players[(int)playerType].RemoveLineSegment(lineSegment);
            unusedLineSegments.Add(lineSegment);
            currentState[lineSegment.Index] = 0;
        }
        public bool HasPlayerFormedLosingTriangle(PlayerType playerType) {
            return players[(int)playerType].DoLineSegmentsFormTriangle();
        }
        public void SetPlayerLost(PlayerType playerType) {
            players[(int)playerType].HasLost = true;
        }
        public bool HasPlayerLost(PlayerType playerType) {
            return players[(int)playerType].HasLost;
        }
        public bool LineSegmentAlreadyTaken(LineSegment lineSegment) {
            if (players[(int)PlayerType.Human].DoesOwnLineSegment(lineSegment) || players[(int)PlayerType.AI].DoesOwnLineSegment(lineSegment)) {
                return true;
            }
            return false;
        }
        
        public LineSegment TakeAITurn() {
            // #TODO
            // Added
            {
                LineSegment[] remainingLineSegments = GetRemainingLineSegments(unusedLineSegments);
                Debug.Log($"Remaining segments indexes are: [{string.Join(", ", remainingLineSegments.Select(segment => { return segment.Index; }).ToArray())}]");
            }

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            Tuple<int, LineSegment> result = MinMax(currentState, PlayerType.AI, int.MinValue, int.MaxValue, unusedLineSegments);
            stopwatch.Stop();
            if (stopwatch.ElapsedMilliseconds > 10) {
                Debug.Log($"Elapsed time: {stopwatch.ElapsedMilliseconds} ms");
            }

            {
                Debug.Log($"Current AI score is {result.Item1}, AI's best move is {result.Item2.Index}");
                Debug.Log($"<color=red>AI has chosen {result.Item2.Index}</color>");
            }

            lastLineSegmentChosenByAI = result.Item2;
            return lastLineSegmentChosenByAI;
        }

        /// <summary>
        /// Return tuple of (value of the current state, best next move) using Min-Max algorithm.
        /// High score means AI has the advantage. (AI tries to maximize the score.)
        /// Low score means Human has the advantage. (Human tries to minimize the score.)
        /// Final score of the state can be either -1, 0 or 1.
        /// </summary>
        /// <param name="playerType"></param>
        /// <returns>The tuple of (value of the current state, best next move)</returns>
        private Tuple<int, LineSegment> MinMax(int[] currentState, PlayerType playerType, int alpha, int beta, HashSet<LineSegment> unusedLineSegments) {
            // Base case
            if (HasPlayerFormedLosingTriangle(PlayerType.Human)) {
                return new Tuple<int, LineSegment>(1, default!);
            } else if (HasPlayerFormedLosingTriangle(PlayerType.AI)) {
                return new Tuple<int, LineSegment>(-1, default!);
            }

            // Check for previous visit
            int currentStateID = Calculations.GetStateID(currentState);
            if (statesMemo.ContainsKey(currentStateID)) {
                return statesMemo[currentStateID];
            }

            LineSegment chosenLineSegment = default!;
            int finalScore;
            if (playerType == PlayerType.AI) {
                // Try to maximize score
                finalScore = int.MinValue;
                foreach (LineSegment lineSegment in GetRemainingLineSegments(unusedLineSegments)) {
                    AddLineSegmentToPlayer(currentState, PlayerType.AI, lineSegment, unusedLineSegments);
                    int score = MinMax(currentState, PlayerType.Human, alpha, beta, unusedLineSegments).Item1;
                    RemoveLineSegmentFromPlayer(currentState, PlayerType.AI, lineSegment, unusedLineSegments);

                    if (score > finalScore) {
                        chosenLineSegment = lineSegment;
                        finalScore = score;
                    }
                    alpha = Math.Max(alpha, score);
                    if (beta <= alpha || score == 1) {
                        RegisterInMemo(currentState, score, lineSegment);
                        return new Tuple<int, LineSegment>(score, lineSegment);
                    }
                }
            } else {
                // Try to minimize score
                finalScore = int.MaxValue;
                foreach (LineSegment lineSegment in GetRemainingLineSegments(unusedLineSegments)) {
                    AddLineSegmentToPlayer(currentState, PlayerType.Human, lineSegment, unusedLineSegments);
                    int score = MinMax(currentState, PlayerType.AI, alpha, beta, unusedLineSegments).Item1;
                    RemoveLineSegmentFromPlayer(currentState, PlayerType.Human, lineSegment, unusedLineSegments);

                    if (score < finalScore) {
                        chosenLineSegment = lineSegment;
                        finalScore = score;
                    }
                    beta = Math.Min(beta, score);
                    if (beta <= alpha || score == -1) {
                        RegisterInMemo(currentState, score, lineSegment);
                        return new Tuple<int, LineSegment>(score, lineSegment);
                    }
                }
            }
            RegisterInMemo(currentState, finalScore, chosenLineSegment);
            return new Tuple<int, LineSegment>(finalScore, chosenLineSegment);
        }

        public void SetLineSegments(LineSegment[] generatedLineSegments) {
            lineSegments = generatedLineSegments;
            unusedLineSegments = new HashSet<LineSegment>(lineSegments);
        }

        /// <summary>
        /// Get all the remaining available segments. This returns all unused segments after the second turn of AI,
        /// but returns only limited choices in the first turn of AI to reduce calculation.
        /// </summary>
        /// <param name="unusedLineSegments">Unused segments</param>
        /// <returns>Remaining available segments</returns>
        private LineSegment[] GetRemainingLineSegments(HashSet<LineSegment> unusedLineSegments) {
            // Just returns all remaining available segments after the second turn of AI
            if (unusedLineSegments.Count() < numberOfLines - 1) {
                return unusedLineSegments.ToArray();
            }

            // For pruning, we reduce the number of exploration for the first AI turn
            int currentStateValue = Calculations.GetStateID(currentState);
            int[] shortLengthScores = new int[] {
                Calculations.GetStateID(new int[15] {0, 0, 0, 0, 0, 0, 0, 0, 0, -1, 0, 0, 0, 0, 0 }),
                Calculations.GetStateID(new int[15] {0, 0, 0, 0, 0, -1, 0, 0, 0, 0, 0, 0, 0, 0, 0 }),
                Calculations.GetStateID(new int[15] {-1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }),
                Calculations.GetStateID(new int[15] {0, 0, 0, 0, -1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }),
                Calculations.GetStateID(new int[15] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, -1 }),
                Calculations.GetStateID(new int[15] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, -1, 0, 0 })
            };

            int[] middleLengthScores = new int[] {
                Calculations.GetStateID(new int[15] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, -1, 0, 0, 0, 0 }),
                Calculations.GetStateID(new int[15] {0, 0, 0, 0, 0, 0, -1, 0, 0, 0, 0, 0, 0, 0, 0 }),
                Calculations.GetStateID(new int[15] {0, -1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }),
                Calculations.GetStateID(new int[15] {0, 0, 0, 0, 0, 0, 0, 0, -1, 0, 0, 0, 0, 0, 0 }),
                Calculations.GetStateID(new int[15] {0, 0, 0, -1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }),
                Calculations.GetStateID(new int[15] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, -1, 0 })
            };

            int[] longLengthScores = new int[] {
                Calculations.GetStateID(new int[15] {0, 0, -1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }),
                Calculations.GetStateID(new int[15] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, -1, 0, 0, 0 }),
                Calculations.GetStateID(new int[15] {0, 0, 0, 0, 0, 0, 0, -1, 0, 0, 0, 0, 0, 0, 0 })
            };

            int[] idxes = default!;
            if (Array.Exists(shortLengthScores, element => element == currentStateValue)) {
                idxes = new int[] { 1, 0, 1, 0, 1, 1, 1, 1, 1, 0, 0, 0, 0, 1, 0 };
                for (int i = 0; i < Array.IndexOf(shortLengthScores, currentStateValue); i++) {
                    idxes = Calculations.Map(idxes, Calculations.rotateMapping);
                }
            } else if (Array.Exists(middleLengthScores, element => element == currentStateValue)) {
                idxes = new int[] { 1, 0, 1, 0, 0, 1, 1, 0, 1, 1, 0, 1, 0, 0, 0 };
                for (int i = 0; i < Array.IndexOf(middleLengthScores, currentStateValue); i++) {
                    idxes = Calculations.Map(idxes, Calculations.rotateMapping);
                }
            } else {
                idxes = new int[] { 0, 0, 0, 0, 0, 0, 1, 0, 0, 1, 1, 1, 0, 0, 1 };
                for (int i = 0; i < Array.IndexOf(longLengthScores, currentStateValue); i++) {
                    idxes = Calculations.Map(idxes, Calculations.rotateMapping);
                }
            }

            List<LineSegment> remainingSegments = new List<LineSegment>();
            for (int i = 0; i < idxes.Count(); i++) {
                if (idxes[i] == 1) {
                    remainingSegments.Add(lineSegments[i]);
                }
            }
            return remainingSegments.ToArray();
        }

        /// <summary>
        /// Register the score and the next best action to the memo once Min-Max finds them.
        /// </summary>
        /// <param name="currentState">Current State</param>
        /// <param name="score">Score</param>
        /// <param name="lineSegment">The next best choice to make</param>
        private void RegisterInMemo(int[] currentState, int score, LineSegment lineSegment) {
            foreach (Dictionary<int, int> mirrorMapping in new Dictionary<int, int>[4] {
                Calculations.noMapping,
                Calculations.mirrorMappingA,
                Calculations.mirrorMappingB,
                Calculations.mirrorMappingC
            }) {
                // Mirror the state
                if (mirrorMapping != Calculations.noMapping) {
                    currentState = Calculations.Map(currentState, mirrorMapping);
                    lineSegment = Calculations.MapLineSegment(lineSegments, lineSegment, mirrorMapping);
                }

                for (int i = 0; i < (int)BoardShape.Hexagon; i++) {
                    int currentStateID = Calculations.GetStateID(currentState);
                    if (statesMemo.ContainsKey(currentStateID)) {
                        break;
                    }
                    statesMemo[currentStateID] = new Tuple<int, LineSegment>(score, lineSegment);
                    // Rotate the state and the line segment by 60 degrees clockwise
                    currentState = Calculations.Map(currentState, Calculations.rotateMapping);
                    lineSegment = Calculations.MapLineSegment(lineSegments, lineSegment, Calculations.rotateMapping);
                }

                // "Un"mirror the state
                if (mirrorMapping == Calculations.mirrorMappingA || mirrorMapping == Calculations.mirrorMappingB) {
                    currentState = Calculations.Map(currentState, mirrorMapping);
                    lineSegment = Calculations.MapLineSegment(lineSegments, lineSegment, mirrorMapping);
                }
            }
        }
    }
}