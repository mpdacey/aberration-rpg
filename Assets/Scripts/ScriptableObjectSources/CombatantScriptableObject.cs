using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Monster", menuName = "Scriptable Objects/Combatant")]
public class CombatantScriptableObject : ScriptableObject
{
    public struct CombatantStats
    {
        public int strength;
        public int magic;
        public int endurance;
        public int agility;
        public int luck;
    }

    public struct CombatantAttributes
    {
        public AttributeAffinity physical;
        public AttributeAffinity fire;
        public AttributeAffinity ice;
        public AttributeAffinity thunder;
        public AttributeAffinity wind;
    }

    public enum AttributeAffinity
    {
        None,
        Weak,
        Resist,
        Null,
        Repel,
        Absorb
    }

    public string combatantName;
    public int combatantMaxHealth;
    public int combatantMaxStamina;
    public CombatantStats combatantBaseStats;
    public CombatantAttributes combatantAttributes;
    public List<SpellScriptableObject> combatantSpells = new List<SpellScriptableObject>();
    public DiceScriptableObject combatantDice;
    public Sprite combatantSprite;
}
