using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterController : MonoBehaviour
{
    [SerializeField]
    public CombatantScriptableObject combatantStats
    {
        private get => combatantStats;
        set
        {
            combatantStats = value;
            spriteRenderer.sprite = combatantStats.combatantSprite;
            localHP = combatantStats.combatantMaxHealth;
            localSP = combatantStats.combatantMaxStamina;
        }
    }
    private SpriteRenderer spriteRenderer;
    private int localHP = 0;
    private int localSP = 0;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public AttackObject GetAttack()
    {
        AttackObject StandardAttack()
        {
            AttackObject attack = new();

            attack.attackName = "Attack";
            attack.attackType = combatantStats.combatantNormalAttackType;
            attack.attackMultitarget = false;

            bool attackIsPhysical = combatantStats.combatantNormalAttackType == SpellScriptableObject.SpellType.Blunt || combatantStats.combatantNormalAttackType == SpellScriptableObject.SpellType.Sharp;
            int attackStat = attackIsPhysical ? combatantStats.combatantBaseStats.strength : combatantStats.combatantBaseStats.magic;

            attack.attackDamage = Mathf.Sqrt(25 * Random.Range(0.95f, 1.05f)) * Mathf.Sqrt(attackStat);

            return attack;
        }

        AttackObject attack;

        if (combatantStats.combatantSpells.Count > 0)
        {
            int selectedSpellIndex = (combatantStats.combatantSpells.Count > 1) ? Mathf.FloorToInt(Random.value * combatantStats.combatantSpells.Count) : 0;
            SpellScriptableObject selectedSpell = combatantStats.combatantSpells[selectedSpellIndex];

            if (localSP - selectedSpell.spellCost < 0)
                attack = StandardAttack();
            else
            {
                attack = new();

                attack.attackName = selectedSpell.spellName;
                attack.attackType = selectedSpell.spellType;
                attack.attackMultitarget = selectedSpell.spellMultitarget;

                bool attackIsPhysical = selectedSpell.spellType == SpellScriptableObject.SpellType.Blunt || selectedSpell.spellType == SpellScriptableObject.SpellType.Sharp;
                int attackStat = attackIsPhysical ? combatantStats.combatantBaseStats.strength : combatantStats.combatantBaseStats.magic;

                attack.attackDamage = Mathf.Sqrt(selectedSpell.spellBaseDamage * Random.Range(0.95f,1.05f)) * Mathf.Sqrt(attackStat);
            }
        }
        else
            attack = StandardAttack();

        return attack;
    }
}
