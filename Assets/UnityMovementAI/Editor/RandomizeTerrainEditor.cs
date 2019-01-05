using UnityEngine;
using UnityEditor;

namespace UnityMovementAI
{
    [CustomEditor(typeof(RandomizeTerrain))]
    public class RandomizeTerrainEditor : Editor
    {

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            RandomizeTerrain myScript = (RandomizeTerrain)target;

            if (CreateCenteredButton("Randomize Terrain"))
            {
                myScript.Randomize();
            }
        }

        public bool CreateCenteredButton(string text)
        {
            GUIContent content = new GUIContent(text);

            Rect rect = GUILayoutUtility.GetRect(content, GUI.skin.button, GUILayout.ExpandWidth(false));

            return GUI.Button(rect, content, GUI.skin.button);
        }
    }
}