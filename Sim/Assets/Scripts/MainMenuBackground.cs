using System.Collections;

using UnityEngine;

namespace Com.WovenPlanet.CodingTest
{
    public class MainMenuBackground : MonoBehaviour
    {
        [SerializeField]
        private GameObject canvas = null;
        [Range(0f, 50f)]
        public float moveZAxisFactor = 0f; // previously 5.0f;
        void Start()
        {
            float canvasWidth = canvas.GetComponent<RectTransform>().rect.width;
            float canvasHeight = canvas.GetComponent<RectTransform>().rect.height;
            
            GetComponent<RectTransform>().sizeDelta = new Vector2(canvasWidth, canvasHeight);
        }

        void Update()
        {
            if (moveZAxisFactor > 0f)
            {
                float zIncrement = moveZAxisFactor * Mathf.Pow(10, -5);
                float x = transform.position.x;
                float y = transform.position.y;
                float z = transform.position.z + zIncrement;
                transform.position = new Vector3(x, y, z);
            }
        }
    }
}
