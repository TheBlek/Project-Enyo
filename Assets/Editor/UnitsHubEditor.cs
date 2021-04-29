using UnityEditor;
using UnityEngine;
using System;

[CustomEditor(typeof(UnitsHub))]
public class UnitsHubEditor : Editor
{
    private SerializedProperty _units;

    private void OnEnable()
    {
        _units = serializedObject.FindProperty("Units");
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        serializedObject.Update();

        if (_units.arraySize != Enum.GetNames(typeof(Enemies)).Length)
            _units.arraySize = Enum.GetNames(typeof(Enemies)).Length;

        for (int i = 0; i < _units.arraySize; i++)
        {
            SerializedProperty element = _units.GetArrayElementAtIndex(i);
            EditorGUILayout.ObjectField(element, new GUIContent(((Enemies)i).ToString()));
        }

        serializedObject.ApplyModifiedProperties();
    }

}
