using UnityEditor;
using UnityEngine;

[CustomEditor (typeof(MapManager))]
public class MapManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        var mapMan = (MapManager)target;

        if (GUILayout.Button("Generate Map"))
        {
            mapMan.InitGrid();
        }
    }
}

