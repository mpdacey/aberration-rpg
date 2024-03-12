using UnityEngine;
using System;

public class PartyController : MonoBehaviour
{
    public static event Action PartyIsReady;

    [System.Serializable]
    public struct PartyMember
    {
        public CombatantScriptableObject partyMemberBaseStats;
        public int currentHP;
        public int currentSP;
    }

    public static PartyMember?[] partyMembers = new PartyMember?[4];
    public PartyMember protagonist;
    public PartyMember[] partyMonsters;

    private void OnEnable()
    {
        SceneController.CombatSceneLoaded += SetPartyValues;
        GoalRiftController.GoalRiftEntered += HealParty;
    }

    private void OnDisable()
    {
        SceneController.CombatSceneLoaded -= SetPartyValues;
        GoalRiftController.GoalRiftEntered -= HealParty;
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

        if (PartyIsReady != null)
            PartyIsReady.Invoke();
    }

    private void SetPartyValues()
    {
        protagonist.currentHP = protagonist.partyMemberBaseStats.combatantMaxHealth;
        protagonist.currentSP = protagonist.partyMemberBaseStats.combatantMaxStamina;
        partyMembers[0] = protagonist;

        for (int i = 0; i < 3; i++)
        {
            if (partyMonsters.Length <= i) break;
            partyMonsters[i].currentHP = partyMonsters[i].partyMemberBaseStats.combatantMaxHealth;
            partyMonsters[i].currentSP = partyMonsters[i].partyMemberBaseStats.combatantMaxStamina;
            partyMembers[i + 1] = partyMonsters[i];
        }

        if (PartyIsReady != null)
            PartyIsReady.Invoke();
    }
}
