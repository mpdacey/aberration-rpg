using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(EquipmentController))]
[CanEditMultipleObjects]
public class EquipmentControllerEditor : Editor
{
    private EquipmentController targetComponent;

    private void OnEnable()
    {
        targetComponent = (EquipmentController)target;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button(new GUIContent("Display Equipment")) && targetComponent != null && targetComponent.testEquipment != null)
            targetComponent.ContructOffer(targetComponent.testEquipment);
    }
}
