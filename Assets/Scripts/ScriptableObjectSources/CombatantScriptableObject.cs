using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Monster", menuName = "Scriptable Objects/Combatant")]
public class CombatantScriptableObject : ScriptableObject
{
    [System.Serializable]
    public struct CombatantStats
    {
        [Range(1,99)] public int strength;
        [Range(1,99)] public int magic;
        [Range(1,99)] public int endurance;
        [Range(1,99)] public int agility;
        [Range(1,99)] public int luck;
    }

    public enum AttributeAffinity
    {
        None,
        Weak,
        Resist,
        Null,
        Repel,
        Absorb,
        Evade
    }

    public string combatantName;
    public int combatantMaxHealth;
    public int combatantMaxStamina;
    public CombatantStats combatantBaseStats;
    public Dictionary<SpellScriptableObject.SpellType, AttributeAffinity> combatantAttributes = new Dictionary<SpellScriptableObject.SpellType, AttributeAffinity>();
    public List<SpellScriptableObject> combatantSpells = new List<SpellScriptableObject>();
    public SpellScriptableObject.SpellType combatantNormalAttackType;
    public Sprite combatantSprite;
    public Dice[] combatantDiceSet;

    [System.Serializable]
    public struct Dice
    {
        public int[] dieFaces;
    }

    [System.Serializable]
    public struct AttributeAffinityDictionaryItem
    {
        public SpellScriptableObject.SpellType key;
        public AttributeAffinity value;
    }

    [SerializeField]
    private AttributeAffinityDictionaryItem[] combatantAttributesArray;

    private void OnValidate()
    {
        SetSpells();
    }

    private void OnEnable()
    {
        SetSpells();
    }

    private void SetSpells()
    {
        if (combatantAttributesArray.Length != Enum.GetNames(typeof(SpellScriptableObject.SpellType)).Length)
            combatantAttributesArray = new AttributeAffinityDictionaryItem[Enum.GetValues(typeof(SpellScriptableObject.SpellType)).Length];

        int i = 0;
        foreach (SpellScriptableObject.SpellType key in Enum.GetValues(typeof(SpellScriptableObject.SpellType)))
        {
            combatantAttributesArray[i].key = key;
            combatantAttributes[key] = combatantAttributesArray[i].value;
            i++;
        }
    }
}
