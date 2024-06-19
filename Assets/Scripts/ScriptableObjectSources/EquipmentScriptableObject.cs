using UnityEngine;

[CreateAssetMenu(fileName = "New Equipment", menuName = "Scriptable Objects/Equipment")]
public class EquipmentScriptableObject : BaseScriptableObject
{
    public enum EquipmentType
    {
        Weapon,
        Armour,
        Trinket
    }

    public string equipmentName;
    public int equipmentHP;
    public int equipmentSP;
    public EquipmentType equipmentType;
    public CombatantScriptableObject.CombatantStats equipmentStats;
    public CombatantScriptableObject.AttributeAffinityDictionaryItem[] equipmentAffinties;
    public SpellScriptableObject[] equipmentSpells;
}
