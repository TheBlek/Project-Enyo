using UnityEditor;
using UnityEngine;
using System;

[CustomEditor(typeof(UnitsHub))]
public class UnitsHubEditor : Editor
{

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        UnitsHub hub = (UnitsHub)target;

        if (hub.Units == null || hub.Units.Length != Enum.GetNames(typeof(Enemies)).Length)
        {
            Debug.Log("I had to refresh");
            hub.Units = new Unit[Enum.GetNames(typeof(Enemies)).Length];
        }
        for (int i = 0; i < hub.Units.Length; i++)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(((Enemies)i).ToString());
            hub.Units[i] = (Unit)EditorGUILayout.ObjectField(hub.Units[i], typeof(Unit), true, GUILayout.ExpandWidth(false));
            GUILayout.EndHorizontal();
        }
    }

}
