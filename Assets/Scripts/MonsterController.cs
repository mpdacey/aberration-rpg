using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterController : MonoBehaviour
{
    public CombatantScriptableObject CombatantStats
    {
        get => combatantStats;
        set
        {
            combatantStats = value;
            spriteRenderer.sprite = combatantStats.combatantSprite;
            localHP = combatantStats.combatantMaxHealth;
            localSP = combatantStats.combatantMaxStamina;
        }
    }
    [SerializeField] private CombatantScriptableObject combatantStats;
    private SpriteRenderer spriteRenderer;
    private int localHP = 0;
    private int localSP = 0;

    private void OnEnable()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public AttackObject GetAttack()
    {
        AttackObject StandardAttack(AttackObject attack)
        {
            SpellScriptableObject normalAttack = new()
            {
                spellName = "Attack",
                spellCost = 0,
                spellHitRate = 95,
                spellBaseDamage = 25,
                spellMultitarget = false,
                spellType = CombatantStats.combatantNormalAttackType
            };
            attack.attackSpell = normalAttack;

            return attack;
        }

        AttackObject attack = new();
        attack.attackerStats = CombatantStats.combatantBaseStats;

        if (CombatantStats.combatantSpells.Count > 0)
        {
            int selectedSpellIndex = (CombatantStats.combatantSpells.Count > 1) ? Mathf.FloorToInt(Random.value * CombatantStats.combatantSpells.Count) : 0;
            SpellScriptableObject selectedSpell = CombatantStats.combatantSpells[selectedSpellIndex];

            if (localSP - selectedSpell.spellCost < 0)
                attack = StandardAttack(attack);
            else
                attack.attackSpell = selectedSpell;
        }
        else
            attack = StandardAttack(attack);

        return attack;
    }

    public AttackObject DeliverAttack(AttackObject incomingAttack)
    {
        bool isAbsorbing = false;
        float affinityDamageMultiplier = 1;

        switch (CombatantStats.combatantAttributes[incomingAttack.attackSpell.spellType])
        {
            case CombatantScriptableObject.AttributeAffinity.Resist:
                affinityDamageMultiplier = 0.5f;
                break;
            case CombatantScriptableObject.AttributeAffinity.Weak:
                affinityDamageMultiplier = 1.5f;
                break;
            case CombatantScriptableObject.AttributeAffinity.Null:
                Debug.Log("Attack Nullified");
                return null;
            case CombatantScriptableObject.AttributeAffinity.Absorb:
                Debug.Log("Attack Absorbed");
                isAbsorbing = true;
                break;
            case CombatantScriptableObject.AttributeAffinity.Repel:
                Debug.Log("Attack Reflected");
                // Initialiser prevents endless pingpong between combatants.
                incomingAttack.isInitialiser = !incomingAttack.isInitialiser;
                return incomingAttack.isInitialiser ? null : incomingAttack;
        }

        //Check for evasion
        if(!isAbsorbing)
        {
            var agilityDiff = CombatantStats.combatantBaseStats.agility - incomingAttack.attackerStats.agility;
            var luckDiff = CombatantStats.combatantBaseStats.luck - incomingAttack.attackerStats.luck;
            var hitRate = Mathf.Clamp(incomingAttack.attackSpell.spellHitRate - agilityDiff / 2 - luckDiff / 4, 5, 100);

            if (hitRate < Random.value * 100)
            {
                Debug.Log("Attack Dodged");
                return null;
            }
        }

        bool attackIsPhysical = incomingAttack.attackSpell.spellType == SpellScriptableObject.SpellType.Blunt || incomingAttack.attackSpell.spellType == SpellScriptableObject.SpellType.Sharp;
        int attackStat = attackIsPhysical ? incomingAttack.attackerStats.strength : incomingAttack.attackerStats.magic;

        float grossDamage = Mathf.Sqrt(incomingAttack.attackSpell.spellBaseDamage * Random.Range(0.95f, 1.05f)) * Mathf.Sqrt(attackStat);
        float resultingDamageTaken = grossDamage * affinityDamageMultiplier / Mathf.Sqrt(CombatantStats.combatantBaseStats.endurance * 4.5f);

        localHP = Mathf.RoundToInt(Mathf.Clamp(localHP - resultingDamageTaken * (isAbsorbing ? -1 : 1), 0, CombatantStats.combatantMaxHealth));

        return null;
    }
}
