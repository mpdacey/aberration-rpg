using UnityEngine;
using System;

public class PartyController : MonoBehaviour
{
    public static event Action<PartyMember?, int> PartyLineUpChanged;
    public static event Action PartyIsReady;

    [Serializable]
    public struct PartyMember
    {
        public CombatantScriptableObject partyMemberBaseStats;
        public int currentHP;
        public int currentSP;
    }

    [Serializable]
    public class ProtagonistEquipment
    {
        public EquipmentScriptableObject weapon;
        public EquipmentScriptableObject defense;
        public EquipmentScriptableObject[] trinkets;
    }

    public static PartyMember?[] partyMembers = new PartyMember?[4];
    public static ProtagonistEquipment protagonistEquipment;
    public PartyMember protagonist;
    [SerializeField] private ProtagonistEquipment initialEquipment;
    public PartyMember[] partyMonsters;
    [SerializeField] private ProtagonistEquipment protagonistEquipmentNonstatic;

    private void OnEnable()
    {
        EquipmentController.EquipmentUpdated += SetPartyValues;
        GoalRiftController.GoalRiftEntered += GoalRiftHealParty;
        GameController.ResetGameEvent += ResetParty;
        StaminaRiftController.RecoverStamina += RestorePartyStamina;
        HealthRiftController.RecoverHealth += HealParty;
    }

    private void OnDisable()
    {
        EquipmentController.EquipmentUpdated -= SetPartyValues;
        GoalRiftController.GoalRiftEntered -= GoalRiftHealParty;
        GameController.ResetGameEvent -= ResetParty;
        StaminaRiftController.RecoverStamina -= RestorePartyStamina;
        HealthRiftController.RecoverHealth -= HealParty;
    }

    private void GoalRiftHealParty()
    {
        partyMembers[0] = RestoreMemberStamina(partyMembers[0].Value, 0.3f);
        HealParty(1);
    }

    private void HealParty(float percentageHeal)
    {
        for(int i = 0; i < partyMembers.Length; i++)
        {
            if (!partyMembers[i].HasValue) continue;

            partyMembers[i] = HealPartyMember(partyMembers[i].Value, percentageHeal);
        }

        if (PartyIsReady != null)
            PartyIsReady.Invoke();
    }

    private PartyMember HealPartyMember(PartyMember member, float percentageHeal)
    {
        member.currentHP += Mathf.CeilToInt(member.partyMemberBaseStats.combatantMaxHealth * percentageHeal);
        member.currentHP = Mathf.Min(member.currentHP, member.partyMemberBaseStats.combatantMaxHealth);
        return member;
    }

    private void RestorePartyStamina(float percentageRestore)
    {
        for (int i = 0; i < partyMembers.Length; i++)
        {
            if (!partyMembers[i].HasValue) continue;

            partyMembers[i] = RestoreMemberStamina(partyMembers[i].Value, percentageRestore);
        }

        if (PartyIsReady != null)
            PartyIsReady.Invoke();
    }

    private PartyMember RestoreMemberStamina(PartyMember member, float percentageRestore)
    {
        member.currentSP += Mathf.CeilToInt(member.partyMemberBaseStats.combatantMaxStamina * percentageRestore);
        member.currentSP = Mathf.Min(member.currentSP, member.partyMemberBaseStats.combatantMaxStamina);
        return member;
    }

    private void SetPartyValues()
    {
        var tempProtag = SetProtagonistStats();
        partyMembers[0] = tempProtag;

        for (int i = 0; i < 3; i++)
        {
            if (partyMonsters.Length <= i) break;
            SetPartyMember(partyMonsters[i], i + 1);
        }

        if (PartyIsReady != null)
            PartyIsReady.Invoke();
    }

    private PartyMember SetProtagonistStats()
    {
        var protagonist = new PartyMember();
        var protagonistStats = Instantiate(this.protagonist.partyMemberBaseStats);

        ApplyEquipmentStats(ref protagonistStats, protagonistEquipment.weapon);
        ApplyEquipmentStats(ref protagonistStats, protagonistEquipment.defense);
        foreach(var tricket in protagonistEquipment.trinkets)
            ApplyEquipmentStats(ref protagonistStats, tricket);

        protagonist.partyMemberBaseStats = protagonistStats;
        if (partyMembers[0].HasValue)
        {
            protagonist.currentHP = Mathf.Min(partyMembers[0].Value.currentHP, protagonistStats.combatantMaxHealth);
            protagonist.currentSP = Mathf.Min(partyMembers[0].Value.currentSP, protagonistStats.combatantMaxStamina);
        }
        else
        {
            protagonist.currentHP = protagonistStats.combatantMaxHealth;
            protagonist.currentSP = protagonistStats.combatantMaxStamina;
        }

        return protagonist;
    }

    private void ApplyEquipmentStats(ref CombatantScriptableObject stats, EquipmentScriptableObject equipment)
    {
        if (equipment == null) return;

        // Set stats
        stats.combatantBaseStats.strength += equipment.equipmentStats.strength;
        stats.combatantBaseStats.magic += equipment.equipmentStats.magic;
        stats.combatantBaseStats.endurance += equipment.equipmentStats.endurance;
        stats.combatantBaseStats.agility += equipment.equipmentStats.agility;
        stats.combatantBaseStats.luck += equipment.equipmentStats.luck;
        stats.combatantMaxHealth += equipment.equipmentHP;
        stats.combatantMaxStamina += equipment.equipmentSP;

        // Set affinities
        foreach(var affinity in equipment.equipmentAffinties)
        {
            if (!stats.combatantAttributes.ContainsKey(affinity.key))
                stats.combatantAttributes.Add(affinity.key, affinity.value);
            else if(stats.combatantAttributes[affinity.key] < affinity.value)
                stats.combatantAttributes[affinity.key] = affinity.value;
        }

        // Set spells
        foreach(var spell in equipment.equipmentSpells)
            if(!stats.combatantSpells.Contains(spell))
                stats.combatantSpells.Add(spell);
    }

    public static void SetPartyMember(PartyMember? creature, int index)
    {
        if (creature.HasValue)
        {
            var temp = creature.Value;
            temp.currentHP = temp.partyMemberBaseStats.combatantMaxHealth;
            temp.currentSP = temp.partyMemberBaseStats.combatantMaxStamina;
            partyMembers[index] = temp;

            if (PartyLineUpChanged != null)
                PartyLineUpChanged.Invoke(temp, index);
        }
        else
        {
            partyMembers[index] = null;
            if (PartyLineUpChanged != null)
                PartyLineUpChanged.Invoke(null, index);
        }
    }

    private void ResetParty()
    {
        Array.Clear(partyMembers, 0, partyMembers.Length);
        Array.Clear(partyMonsters, 0, partyMonsters.Length);
        ResetProtagonistEquipment();
        SetPartyValues();
    }

    private void ResetProtagonistEquipment()
    {
        if (protagonistEquipment == null)
        {
            protagonistEquipment = new ProtagonistEquipment();
            protagonistEquipment.trinkets = new EquipmentScriptableObject[initialEquipment.trinkets.Length];
        }

        protagonistEquipment.weapon = initialEquipment.weapon;
        protagonistEquipment.defense = initialEquipment.defense;
        protagonistEquipment.trinkets[0] = initialEquipment.trinkets[0];
        protagonistEquipment.trinkets[1] = initialEquipment.trinkets[1];

        // Assigns protagonistEquipment memory address to protagonistEquipmentNonstatic
        // Therefore, we don't need to update it when ever protagonistEquipment is updated
        protagonistEquipmentNonstatic = protagonistEquipment;
    }
}
