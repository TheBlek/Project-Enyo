using UnityEditor;
using UnityEngine;
using System;

[CustomEditor (typeof(MapManager))]
public class MapManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        var mapMan = (MapManager)target;

        int length = Enum.GetNames(typeof(MapTiles)).Length;
        if (mapMan.tiles_by_name == null || mapMan.tiles_by_name.Length != length)
            mapMan.tiles_by_name = new RuleTile[Enum.GetNames(typeof(MapTiles)).Length];

        mapMan.tiles_by_name = GrabRuleTiles(mapMan.tiles_by_name);

        if (GUILayout.Button("Generate Map"))
        {
            mapMan.InitGrid();
        }
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
}

