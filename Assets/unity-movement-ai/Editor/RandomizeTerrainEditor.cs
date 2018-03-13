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

            if (createCenteredButton("Randomize Terrain"))
            {
                myScript.randomize();
            }
        }

        public bool createCenteredButton(string text)
        {
            GUIContent content = new GUIContent(text);

            Rect rect = GUILayoutUtility.GetRect(content, GUI.skin.button, GUILayout.ExpandWidth(false));
            rect.position = new Vector2((Screen.width - rect.size.x) / 2, rect.position.y);

            return GUI.Button(rect, content, GUI.skin.button);
        }
    }
}