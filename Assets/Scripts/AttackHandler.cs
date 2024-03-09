using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackHandler
{
    public static AttackObject GenerateNormalAttack(CombatantScriptableObject attackerObject)
    {
        AttackObject attack = new();
        attack.attackerStats = attackerObject.combatantBaseStats;

        SpellScriptableObject normalAttack = new()
        {
            spellName = "Attack",
            spellCost = 0,
            spellHitRate = 95,
            spellBaseDamage = 25,
            spellMultitarget = false,
            spellType = attackerObject.combatantNormalAttackType
        };
        attack.attackSpell = normalAttack;

        return attack;
    }

    public static void CalculateIncomingDamage(AttackObject incomingAttack, CombatantScriptableObject combatantStats, ref int currentHP, out AttackObject reflectedAttack, bool isGuarding = false)
    {
        bool isAbsorbing = false;
        float affinityDamageMultiplier = 1;

        reflectedAttack = null;
        if (isGuarding) Debug.Log("Guarding next attack");

        switch (combatantStats.combatantAttributes[incomingAttack.attackSpell.spellType])
        {
            case CombatantScriptableObject.AttributeAffinity.None:
                affinityDamageMultiplier = isGuarding? 0.5f : 1;
                break;
            case CombatantScriptableObject.AttributeAffinity.Resist:
                Debug.Log("Attack Resisted");
                affinityDamageMultiplier = 0.5f;
                break;
            case CombatantScriptableObject.AttributeAffinity.Weak:
                Debug.Log("Weakness Struct");
                affinityDamageMultiplier = isGuarding ? 0.65f : 1.5f;
                break;
            case CombatantScriptableObject.AttributeAffinity.Null:
                Debug.Log("Attack Nullified");
                return;
            case CombatantScriptableObject.AttributeAffinity.Absorb:
                Debug.Log("Attack Absorbed");
                isAbsorbing = true;
                affinityDamageMultiplier = 0.8f;
                break;
            case CombatantScriptableObject.AttributeAffinity.Repel:
                Debug.Log("Attack Reflected");
                // Initialiser prevents endless pingpong between combatants.
                reflectedAttack = incomingAttack;
                reflectedAttack.isInitialiser = !reflectedAttack.isInitialiser;
                return;
        }

        //Check for evasion
            if (!isAbsorbing)
        {
            var agilityDiff = combatantStats.combatantBaseStats.agility - incomingAttack.attackerStats.agility;
            var luckDiff = combatantStats.combatantBaseStats.luck - incomingAttack.attackerStats.luck;
            var hitRate = Mathf.Clamp(incomingAttack.attackSpell.spellHitRate - agilityDiff / 2 - luckDiff / 4, 5, 100);

            if (hitRate < Random.value * 100)
            {
                Debug.Log("Attack Dodged");
                return;
            }
        }

        bool attackIsPhysical = incomingAttack.attackSpell.spellType == SpellScriptableObject.SpellType.Blunt || incomingAttack.attackSpell.spellType == SpellScriptableObject.SpellType.Sharp;
        int attackStat = attackIsPhysical ? incomingAttack.attackerStats.strength : incomingAttack.attackerStats.magic;

        float grossDamage = Mathf.Sqrt(incomingAttack.attackSpell.spellBaseDamage) * Mathf.Sqrt(attackStat*6) * affinityDamageMultiplier;
        float resultingDamageTaken = Mathf.Clamp(grossDamage* 3 * Random.Range(0.90f, 1.10f) / Mathf.Sqrt(combatantStats.combatantBaseStats.endurance*6), 0, 9999);

        int oldHP = currentHP;
        currentHP = Mathf.RoundToInt(Mathf.Clamp(currentHP - resultingDamageTaken * (isAbsorbing ? -1 : 1), 0, combatantStats.combatantMaxHealth));

        Debug.Log($"{combatantStats.combatantName} takes {oldHP -= currentHP}");
    }
}
