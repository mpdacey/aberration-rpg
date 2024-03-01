using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Monster", menuName = "Scriptable Objects/Combatant")]
public class CombatantScriptableObject : MonoBehaviour
{
    public struct CombatantStats
    {
        public int strength;
        public int magic;
        public int endurance;
        public int agility;
        public int luck;
    }

    public string combatantName;
    public int combatantMaxHealth;
    public int combatantMaxStamina;
    public CombatantStats combatantBaseStats;
    public List<SpellScriptableObject> combatantSpells = new List<SpellScriptableObject>();
    public DiceScriptableObject combatantDice;
    public Sprite combatantSprite;
}
