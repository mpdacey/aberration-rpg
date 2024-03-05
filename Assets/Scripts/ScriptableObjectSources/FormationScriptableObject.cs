using UnityEngine;

[CreateAssetMenu(fileName = "Formation", menuName = "Scriptable Objects/Formation")]
public class FormationScriptableObject : ScriptableObject
{
    public CombatantScriptableObject[] monsters;
}
