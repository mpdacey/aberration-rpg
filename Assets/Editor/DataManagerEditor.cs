using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(DataManager))]
public class DataManagerEditor : Editor
{
    DataManager component;
    private void OnEnable()
    {
        component = (DataManager)target;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        EditorGUILayout.Space();

        if (GUILayout.Button("Save Progress"))
            component.SaveProgress();

        if (GUILayout.Button("Load Progress"))
            ;//component.LoadProgress();
    }
}
