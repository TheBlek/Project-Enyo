using UnityEditor;
using UnityEngine;
using System;

[CustomEditor (typeof(MapManager))]
public class MapManagerEditor : Editor
{
    private SerializedProperty _tiles;
    private SerializedProperty _collidability;

    private void OnEnable()
    {
        _tiles = serializedObject.FindProperty("Tiles");
        _collidability = serializedObject.FindProperty("Collidability");
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        serializedObject.Update();

        int length = Enum.GetNames(typeof(MapTiles)).Length;

        if (_tiles.arraySize != length)
            _tiles.arraySize = length;

        if (_collidability.arraySize != length)
            _collidability.arraySize = length;

        for (int i = 0; i < length; i++)
            EditorGUILayout.ObjectField(_tiles.GetArrayElementAtIndex(i), new GUIContent(((MapTiles)i).ToString()));
        
        for (int i = 0; i < length; i++)
        {
            SerializedProperty element = _collidability.GetArrayElementAtIndex(i);
            element.boolValue = EditorGUILayout.Toggle(new GUIContent(((MapTiles)i).ToString()), element.boolValue);
        }

        serializedObject.ApplyModifiedProperties();
    }
}