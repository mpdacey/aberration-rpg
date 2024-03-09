using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Random = UnityEngine.Random;
using System.Linq;

public class MonsterController : MonoBehaviour
{
    public static event Action<int> MonsterDefeated;
    public static event Action<int> MonsterStunned;

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
            currentDiceValues = new int[combatantStats.combatantDiceSet.Length];
        }
    }
    public bool isDefeated = false;
    [SerializeField] private CombatantScriptableObject combatantStats;
    [SerializeReference] private DiceUIController[] diceControllers;
    private SpriteRenderer spriteRenderer;
    private int localHP = 0;
    private int localSP = 0;
    private int[] currentDiceValues;

    private void OnEnable()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public IEnumerator GenerateDice()
    {
        //diceControllers[0].gameObject.SetActive(true);
        for(int i = 0; i < currentDiceValues.Length; i++)
        {
            diceControllers[i].gameObject.SetActive(currentDiceValues.Length >= i);
            currentDiceValues[i] = combatantStats.combatantDiceSet[i].dieFaces[Mathf.FloorToInt(Random.Range(0, currentDiceValues.Length - 0.01f))];
            diceControllers[i].CastFace(currentDiceValues[i], combatantStats.combatantDiceSet[i].dieFaces);

            yield return new WaitForSeconds(diceControllers[i].revealTime/2);
        }
    }

    public void HideDice()
    {
        diceControllers[0].gameObject.SetActive(false);
        diceControllers[1].gameObject.SetActive(false);
    }

    public AttackObject GetAttack()
    {
        AttackObject attack = new();
        attack.attackerStats = CombatantStats.combatantBaseStats;

        if (CombatantStats.combatantSpells.Count > 0)
        {
            int selectedSpellIndex = (CombatantStats.combatantSpells.Count > 1) ? Mathf.FloorToInt(UnityEngine.Random.value * CombatantStats.combatantSpells.Count) : 0;
            SpellScriptableObject selectedSpell = CombatantStats.combatantSpells[selectedSpellIndex];

            if (localSP - selectedSpell.spellCost < 0)
                attack = AttackHandler.GenerateNormalAttack(CombatantStats);
            else
                attack.attackSpell = selectedSpell;
        }
        else
            attack = AttackHandler.GenerateNormalAttack(CombatantStats);

        return attack;
    }

    public void RecieveAttack(AttackObject incomingAttack, bool isStunned = false)
    {
        AttackObject reflectedAttack = null;

        var affinity = AttackHandler.CalculateIncomingDamage(incomingAttack, combatantStats, ref localHP, out reflectedAttack);
        int lastDiceIndex = Array.FindLastIndex(currentDiceValues, x => x > 0);

        if (currentDiceValues[0] > 0)
        {
            switch (affinity)
            {
                case CombatantScriptableObject.AttributeAffinity.None:
                case CombatantScriptableObject.AttributeAffinity.Resist:
                    diceControllers[lastDiceIndex].UpdateFace(--currentDiceValues[lastDiceIndex]);
                break;
                case CombatantScriptableObject.AttributeAffinity.Weak:
                    diceControllers[lastDiceIndex].UpdateFace(currentDiceValues[lastDiceIndex] -= 3);
                    break;
                case CombatantScriptableObject.AttributeAffinity.Repel:
                case CombatantScriptableObject.AttributeAffinity.Absorb:
                    diceControllers[lastDiceIndex].UpdateFace(Mathf.Clamp(++currentDiceValues[lastDiceIndex], 1, 9));
                    break;
            }
        }

        //TODO: Input reflect handling;

        if (localHP <= 0 && MonsterDefeated != null)
        {
            spriteRenderer.enabled = false;
            if(MonsterDefeated != null)
                MonsterDefeated.Invoke(transform.GetSiblingIndex());
        }
        else if(currentDiceValues[0] <= 0)
        {
            if (MonsterStunned != null)
                MonsterStunned.Invoke(transform.GetSiblingIndex());
        }
    }
}
