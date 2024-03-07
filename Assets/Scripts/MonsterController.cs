using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class MonsterController : MonoBehaviour
{
    public static event Action<int> MonsterDefeated;

    public CombatantScriptableObject CombatantStats
    {
        get => combatantStats;
        set
        {
            combatantStats = value;
            if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
            spriteRenderer.sprite = combatantStats.combatantSprite;
            localHP = combatantStats.combatantMaxHealth;
            localSP = combatantStats.combatantMaxStamina;
        }
    }
    public bool isDefeated = false;
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
            int selectedSpellIndex = (CombatantStats.combatantSpells.Count > 1) ? Mathf.FloorToInt(UnityEngine.Random.value * CombatantStats.combatantSpells.Count) : 0;
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

    private void RecieveAttack(AttackObject incomingAttack)
    {
        AttackObject reflectedAttack = null;

        AttackHandler.CalculateIncomingDamage(incomingAttack, combatantStats, ref localHP, out reflectedAttack);

        //TODO: Input reflect handling;

        if (localHP <= 0 && MonsterDefeated != null)
            MonsterDefeated.Invoke(transform.GetSiblingIndex());
    }
}
