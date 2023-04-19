using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.UI;

namespace Com.WovenPlanet.CodingTest
{
    public class MainMenuTextureCreator : MonoBehaviour
    {
        public int resolution = 256;
        public float frequency = 1f;
        [Range(1, 3)]
        public int dimensions = 3;
        [Range(1, 8)]
        public int octaves = 1;
        [Range(1f, 4f)]
        public float lacunarity = 2f;
        [Range(0f, 1f)]
        public float persistence = 0.5f;
        public Gradient coloring;
        private Texture2D texture;

        public NoiseMethodType type;

        public void FillTexture()
        {
            float stepSize = 1f / resolution;
            if (texture.width != resolution)
            {
                texture.Resize(resolution, resolution);
            }
            Vector3 point00 = transform.TransformPoint(new Vector3(-0.5f, -0.5f));
            Vector3 point10 = transform.TransformPoint(new Vector3( 0.5f, -0.5f));
            Vector3 point01 = transform.TransformPoint(new Vector3(-0.5f,  0.5f));
            Vector3 point11 = transform.TransformPoint(new Vector3(0.5f ,  0.5f));

            NoiseMethod method = Noise.noiseMethods[(int) type][dimensions - 1];
            Enumerable.Range(0, resolution).ForEach(y => 
            {
                Vector3 point0 = Vector3.Lerp(point00, point01, (y + 0.5f) * stepSize);
                Vector3 point1 = Vector3.Lerp(point10, point11, (y + 0.5f) * stepSize);
                Enumerable.Range(0, resolution).ForEach(x => 
                {
                    Vector3 point = Vector3.Lerp(point0, point1, (x + 0.5f) * stepSize);
                    float sample = Noise.Sum(method, point, frequency, octaves, lacunarity, persistence);
                    if (type != NoiseMethodType.Value)
                    {
                        sample = sample * 0.5f + 0.5f;
                    }
                    texture.SetPixel(x, y, coloring.Evaluate(sample));
                });
            });
            texture.Apply();
        }
        void OnEnable()
        {
            if (texture == null)
            {
                texture = new Texture2D(resolution, resolution, TextureFormat.RGB24, true);
                texture.name = "ProceduralTexture";
                texture.wrapMode = TextureWrapMode.Clamp;
                texture.filterMode = FilterMode.Trilinear;
                texture.anisoLevel = 9;
                GetComponent<Image>().material.mainTexture = texture;
            }
            FillTexture();
        }

        void Update()
        {
            if (transform.hasChanged) {
                transform.hasChanged = false;
                FillTexture();
            }
        }
    }
}
