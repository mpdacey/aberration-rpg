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
    public static event Action<DamageTextProducer, int> DisplayRecievedMonsterDamage;
    public static event Action<DamageTextProducer, CombatantScriptableObject.AttributeAffinity> DisplayMonsterAffinityEvent;

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
    [SerializeField] private DiceUIController[] diceControllers;
    [SerializeField] private DamageVFXController damageVFXController;
    private SpriteRenderer spriteRenderer;
    private Animator animator;
    private int localHP = 0;
    private int localSP = 0;
    private int[] currentDiceValues;

    private void OnEnable()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }

    public void PlayEntranceAnimation()
    {
        animator.Play("MonsterEntrance");
    }

    public IEnumerator GenerateDice()
    {
        for(int i = 0; i < currentDiceValues.Length; i++)
        {
            diceControllers[i].gameObject.SetActive(currentDiceValues.Length >= i);
            float weightedRandomIndex = Random.value * GetLuckAdvantage(PartyController.partyMembers[0].Value.partyMemberBaseStats.combatantBaseStats.luck, combatantStats.combatantBaseStats.luck);
            int validatedDiceIndex = Mathf.Min(Mathf.RoundToInt(weightedRandomIndex * (combatantStats.combatantDiceSet[i].dieFaces.Length - 1)), combatantStats.combatantDiceSet[i].dieFaces.Length - 1);
            currentDiceValues[i] = combatantStats.combatantDiceSet[i].dieFaces[validatedDiceIndex];
            diceControllers[i].CastFace(currentDiceValues[i], combatantStats.combatantDiceSet[i].dieFaces);

            yield return new WaitForSeconds(diceControllers[i].revealTime/2);
        }
    }

    private float GetLuckAdvantage(int protagonistLuck, int monsterLuck)
    {
        float advantage = protagonistLuck * 0.575f - monsterLuck;
        float variance = Random.Range(-0.15f, 0.15f); 
        float boundAdvantage = 1 + Mathf.Clamp((advantage + variance) * 0.085f, -0.4f, 0.4f);
        return boundAdvantage;
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
            {
                attack.attackSpell = selectedSpell;
                localSP -= selectedSpell.spellCost;
            }
        }
        else
            attack = AttackHandler.GenerateNormalAttack(CombatantStats);

        return attack;
    }

    public void RecieveAttack(AttackObject incomingAttack, bool isStunned = false)
    {
        AttackObject reflectedAttack = null;

        int oldHP = localHP;
        var affinity = AttackHandler.CalculateIncomingDamage(incomingAttack, combatantStats, ref localHP, out reflectedAttack);
        int lastDiceIndex = Array.FindLastIndex(currentDiceValues, x => x > 0);

        damageVFXController.PlayDamageVFX(incomingAttack.attackSpell.spellType);
        if (DisplayRecievedMonsterDamage != null && oldHP != localHP)
            DisplayRecievedMonsterDamage.Invoke(GetComponent<DamageTextProducer>(), localHP-oldHP);

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

        if (DisplayMonsterAffinityEvent != null && affinity != CombatantScriptableObject.AttributeAffinity.None)
            DisplayMonsterAffinityEvent.Invoke(GetComponent<DamageTextProducer>(), affinity);

        //TODO: Input reflect handling;

        if(affinity != CombatantScriptableObject.AttributeAffinity.Evade)
            SeenMonsterAffinities.UpdateAffinityWitness(combatantStats, incomingAttack.attackSpell.spellType);

        if (localHP <= 0 && MonsterDefeated != null)
        {
            animator.Play("MonsterDefeated");
            foreach (var die in diceControllers)
                die.UpdateFace(0);
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
