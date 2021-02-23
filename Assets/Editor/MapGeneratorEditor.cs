using UnityEditor;
using UnityEngine;
using System;

[CustomEditor(typeof(MapGenerator))]
public class MapGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        int length = Enum.GetNames(typeof(MapTiles)).Length;

        GUILayout.Label("Regions Thresholds");

        for (int i = 0; i < length; i++)
        {
            string label = ((MapTiles)i).ToString();
            (target as MapGenerator)._thresholds[i] = EditorGUILayout.Slider(label, (target as MapGenerator)._thresholds[i], 0f, 1f);
        }
    }
}