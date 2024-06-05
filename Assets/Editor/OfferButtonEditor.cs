using UnityEditor;
using UnityEditor.UI;

[CustomEditor(typeof(OfferButton))]
public class OfferButtonEditor : ButtonEditor
{
    SerializedProperty m_OnSelectProperty;

    protected override void OnEnable()
    {
        base.OnEnable();
        m_OnSelectProperty = serializedObject.FindProperty("m_OnSelect");
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        EditorGUILayout.Space();

        serializedObject.Update();
        EditorGUILayout.PropertyField(m_OnSelectProperty);
        serializedObject.ApplyModifiedProperties();

    }
}
