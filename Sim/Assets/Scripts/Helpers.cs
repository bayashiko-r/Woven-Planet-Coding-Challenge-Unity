using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Com.WovenPlanet.CodingTest {
    public static class InumberableExtensions {
        public static void ForEach<T>(this IEnumerable<T> enumeration, Action<T> action) {
            foreach (T item in enumeration) {
                action(item);
            }
        }
    }

    // Added
    public static class Calculations {
        public static Dictionary<int, int> noMapping = new Dictionary<int, int>() {
                { 0, 0}, { 1, 1}, { 2, 2}, { 3, 3}, { 4, 4}, { 5, 5},
                { 6, 6}, { 7, 7}, { 8, 8}, { 9, 9}, { 10, 10}, { 11, 11}, 
                { 12, 12}, { 13, 13}, { 14, 14}
                };

        public static Dictionary<int, int> rotateMapping = new Dictionary<int, int>() {
                { 9, 5}, { 5, 0}, { 0, 4}, { 4, 14}, { 14, 12}, { 12, 9},
                { 8, 3}, { 3, 13}, { 13, 10}, { 10, 6}, { 6, 1}, { 1, 8},
                { 2, 11}, { 11, 7}, { 7, 2}
                };

        public static Dictionary<int, int> mirrorMappingA = new Dictionary<int, int>() {
                { 0, 4}, { 4, 0}, { 5, 14}, { 14, 5}, { 9, 12}, { 12, 9},
                { 8, 8}, { 10, 10}, { 2, 2},
                { 1, 3}, { 3, 1}, { 6, 13}, { 13, 6}, { 7, 11}, { 11, 7}
                };

        public static Dictionary<int, int> mirrorMappingB = new Dictionary<int, int>() {
                { 0, 5}, { 5, 0}, { 12, 14}, { 14, 12}, { 9, 4}, { 4, 9},
                { 1, 1}, { 7, 7}, { 13, 13},
                { 10, 3}, { 3, 10}, { 6, 8}, { 8, 6}, { 2, 11}, { 11, 2}
                };

        public static Dictionary<int, int> mirrorMappingC = new Dictionary<int, int>() {
                { 14, 4}, { 4, 14}, { 5, 9}, { 9, 5}, { 0, 12}, { 12, 0},
                { 6, 6}, { 11, 11}, { 3, 3},
                { 1, 10}, { 10, 1}, { 8, 13}, { 13, 8}, { 7, 2}, { 2, 7}
                };

        /// <summary>
        /// Get the state ID using ternary system. State value takes either -1, 0, 1.
        /// -1: Selected by Human
        /// 0: Not selected yet
        /// +1: Selected by AI
        /// </summary>
        /// <param name="state">Current state</param>
        /// <returns>State ID</returns>
        public static int GetStateID(int[] state) {
            int stateID = 0;
            for (int i = 0; i < state.Length; i++) {
                stateID += state[i] * Convert.ToInt32(Math.Pow(3, i));
            }
            return stateID;
        }

        /// <summary>
        /// Map the state using the given mapping dictionary
        /// </summary>
        /// <param name="currentState">Current state</param>
        /// <param name="mapping">which mapping to use</param>
        /// <returns>New state changed by the mapping</returns>
        public static int[] Map(int[] currentState, Dictionary<int, int> mapping) {
            int[] newState = new int[currentState.Length];
            foreach (KeyValuePair<int, int> entry in mapping) {
                newState[entry.Value] = currentState[entry.Key];
            }
            return newState;
        }

        /// <summary>
        /// Map the line using the given mapping dictionary.
        /// </summary>
        /// <param name="lineSegments">all the line segments</param>
        /// <param name="lineSegment">one line segment</param>
        /// <param name="mapping">which mapping to use</param>
        /// <returns>New line changed by the mapping</returns>
        public static LineSegment MapLineSegment(LineSegment[] lineSegments, LineSegment lineSegment, Dictionary<int, int> mapping) {
            return lineSegments[mapping[lineSegment.Index]];
        }
    }

    public static class Helpers {
        /// <summary>
        /// Find GameObject in the children of a gameobject using recurtion.
        /// </summary>
        /// <param name="gameObject">the parent GameObject</param>
        /// <param name="name">the name of the object to find</param>
        /// <returns>returns the gameobject to find if found, returns null if not found</returns>
        public static GameObject FindInChildren(GameObject gameObject, string name) {
            if (gameObject.name == name) {
                return gameObject;
            }
            foreach (Transform child in gameObject.transform) {
                GameObject result = FindInChildren(child.gameObject, name);
                if (result != null) {
                    return result;
                }
            }
            return null;
        }
    }
}
