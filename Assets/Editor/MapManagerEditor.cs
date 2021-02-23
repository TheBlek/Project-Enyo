using UnityEditor;
using UnityEngine;
using System;
using System.Collections.Generic;

[CustomEditor (typeof(MapManager))]
public class MapManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        var mapMan = (MapManager)target;
        if (mapMan.tiles_by_name == null)
            mapMan.tiles_by_name = new Dictionary<MapTiles, RuleTile>();
        mapMan.tiles_by_name = GrabRuleTiles(mapMan.tiles_by_name);

        if (GUILayout.Button("Generate Map"))
        {
            mapMan.InitGrid();
        }
    }

    private Dictionary<MapTiles, RuleTile> GrabRuleTiles(Dictionary<MapTiles, RuleTile> dict)
    {
        int length = Enum.GetNames(typeof(MapTiles)).Length;
        for (int i = 0; i < length; i++)
        {
            GUILayout.Label(((MapTiles)i).ToString());
            _ = dict.TryGetValue((MapTiles)i, out var tmp);
            dict[(MapTiles)i] = EditorGUILayout.ObjectField(tmp, typeof(RuleTile), true) as RuleTile;
        }
        return dict;
    }
}

