﻿using UnityEditor;
using UnityEngine;
using System;

[CustomEditor (typeof(MapManager))]
public class MapManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        var mapMan = (MapManager)target;

        mapMan.tiles_by_name = (RuleTile[])CheckList(mapMan.tiles_by_name, typeof(MapTiles));
        mapMan.collidable_tiles = CheckList(mapMan.collidable_tiles, typeof(MapTiles));

        mapMan.tiles_by_name = GrabRuleTiles(mapMan.tiles_by_name);
        mapMan.collidable_tiles = GrabCollidableTiles(mapMan.collidable_tiles);

        if (GUILayout.Button("Generate Map"))
        {
            mapMan.InitGrid();
        }
    }

    private bool[] CheckList(bool[] data, Type enumType)
    {
        int length = Enum.GetNames(enumType).Length;
        if (data == null || data.Length != length)
            data = new bool[length];

        return data;
    }

    private bool[] GrabCollidableTiles(bool[] data)
    {

        int length = Enum.GetNames(typeof(MapTiles)).Length;
        for (int i = 0; i < length; i++)
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label(((MapTiles)i).ToString());
            data[i] = EditorGUILayout.Toggle(data[i]);
            EditorGUILayout.EndHorizontal();
        }
        return data;
    }

    private RuleTile[] GrabRuleTiles(RuleTile[] dict)
    {
        int length = Enum.GetNames(typeof(MapTiles)).Length;
        for (int i = 0; i < length; i++)
        {
            
            GUILayout.Label(((MapTiles)i).ToString());
            dict[i] = EditorGUILayout.ObjectField(dict[i], typeof(RuleTile), true) as RuleTile;
            
        }
        return dict;
    }

    private dynamic[] CheckList(dynamic[] data, Type enumType)
    {
        int length = Enum.GetNames(enumType).Length;
        if (data == null || data.Length != length)
            data = new object[length];

        return data;
    } 
}

