using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GoalRiftController))]
public class GoalRiftControllerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("Skip current floor"))
            ((GoalRiftController)target).StartCoroutine(((GoalRiftController)target).EnterRift());
    }
}
