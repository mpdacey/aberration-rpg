using UnityEngine;

[CreateAssetMenu(fileName = "New Equipment", menuName = "Scriptable Objects/Equipment")]
public class EquipmentScriptableObject : ScriptableObject
{
    public enum EquipmentType
    {
        Weapon,
        Armour,
        Trinket
    }

    public string equipmentName;
    public CombatantScriptableObject.CombatantStats equipmentStats;
    public SpellScriptableObject[] equipmentSpells;
}
