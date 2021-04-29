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

        var mapGen = target as MapGenerator;

        if(mapGen._thresholds == null || mapGen._thresholds.Length != length)
        {
            Debug.Log("Had to reset threshold array");
            mapGen._thresholds = new float[length];
        }

        GUILayout.Label("Regions Thresholds");

        for (int i = 0; i < length; i++)
        {
            string label = ((MapTiles)i).ToString();
            mapGen._thresholds[i] = EditorGUILayout.Slider(label, mapGen._thresholds[i], 0f, 1f);
        }
    }
}