using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Com.WovenPlanet.CodingTest {
    public class MainGameTextureCreator : MainMenuTextureCreator {
        private double time;

        private void FixedUpdate() {
            time += Time.deltaTime;
            // Change parameters over time
            lacunarity = (float)(1.5f * Math.Sin(time / 3.0f) + 2.5f);
            int dimensionVal = (int)Math.Ceiling(time / 3.0f) % 4;
            dimensions = Math.Min(dimensionVal, 4 - dimensionVal) + 1;
        }
    }

}

