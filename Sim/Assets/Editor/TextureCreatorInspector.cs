using UnityEngine;
using UnityEditor;

namespace Com.WovenPlanet.CodingTest
{
    [CustomEditor(typeof(MainMenuTextureCreator))]
    public class TextureCreatorInspector : Editor
    {
        private MainMenuTextureCreator creator;

        private void OnEnable()
        {
            creator = target as MainMenuTextureCreator;
            Undo.undoRedoPerformed += RefreshCreator;
        }

        private void OnDisable()
        {
            Undo.undoRedoPerformed -= RefreshCreator;
        }

        private void RefreshCreator()
        {
            if (Application.isPlaying)
            {
                creator.FillTexture();
            }
        }
        public override void OnInspectorGUI()
        {
            EditorGUI.BeginChangeCheck();
            DrawDefaultInspector();
            if (EditorGUI.EndChangeCheck())
            {
                RefreshCreator();
            }
        }
    }
}
