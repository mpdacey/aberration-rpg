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
        AttackObject StandardAttack(AttackObject attack)
        {
            SpellScriptableObject normalAttack = new()
            {
                spellName = "Attack",
                spellCost = 0,
                spellHitRate = 95,
                spellBaseDamage = 25,
                spellMultitarget = false,
                spellType = combatantStats.combatantNormalAttackType
            };
            attack.attackSpell = normalAttack;

            return attack;
        }

        AttackObject attack = new();
        attack.attackerStats = combatantStats.combatantBaseStats;

        if (combatantStats.combatantSpells.Count > 0)
        {
            int selectedSpellIndex = (combatantStats.combatantSpells.Count > 1) ? Mathf.FloorToInt(Random.value * combatantStats.combatantSpells.Count) : 0;
            SpellScriptableObject selectedSpell = combatantStats.combatantSpells[selectedSpellIndex];

            if (localSP - selectedSpell.spellCost < 0)
                attack = StandardAttack(attack);
            else
                attack.attackSpell = selectedSpell;
        }
        else
            attack = StandardAttack(attack);

        return attack;
    }


            }
        }

    }
}
