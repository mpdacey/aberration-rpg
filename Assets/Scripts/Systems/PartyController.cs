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
        GoalRiftController.GoalRiftEntered += HealParty;
        GameController.ResetGameEvent += ResetParty;
    }

    private void OnDisable()
    {
        EquipmentController.EquipmentUpdated -= SetPartyValues;
        GoalRiftController.GoalRiftEntered -= HealParty;
        GameController.ResetGameEvent += ResetParty;
    }

    private void HealParty()
    {
        for(int i = 0; i < partyMembers.Length; i++)
        {
            if (!partyMembers[i].HasValue) continue;

            var currentMember = partyMembers[i].Value;
            currentMember.currentHP = currentMember.partyMemberBaseStats.combatantMaxHealth;
            partyMembers[i] = currentMember;
        }

        var temp = partyMembers[0].Value;
        temp.currentSP = Mathf.Min(temp.currentSP + temp.partyMemberBaseStats.combatantMaxStamina / 3, temp.partyMemberBaseStats.combatantMaxStamina);
        partyMembers[0] = temp;

        if (PartyIsReady != null)
            PartyIsReady.Invoke();
    }

    private void SetPartyValues()
    {
        var tempProtag = SetProtagonistStats();
        tempProtag.currentHP = tempProtag.partyMemberBaseStats.combatantMaxHealth;
        tempProtag.currentSP = tempProtag.partyMemberBaseStats.combatantMaxStamina;
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
        protagonist.currentHP = Mathf.Min(this.protagonist.currentHP, protagonistStats.combatantMaxHealth);
        protagonist.currentSP = Mathf.Min(this.protagonist.currentSP, protagonistStats.combatantMaxStamina);

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
