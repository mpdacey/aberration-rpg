using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartyController : MonoBehaviour
{
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
        protagonist.currentHP = protagonist.partyMemberBaseStats.combatantMaxHealth;
        protagonist.currentSP = protagonist.partyMemberBaseStats.combatantMaxStamina;
        partyMembers[0] = protagonist;

        for(int i = 0; i < 3; i++)
        {
            if (partyMonsters.Length <= i) break;
            partyMonsters[i].currentHP = partyMonsters[i].partyMemberBaseStats.combatantMaxHealth;
            partyMonsters[i].currentSP = partyMonsters[i].partyMemberBaseStats.combatantMaxStamina;
            partyMembers[i+1] = partyMonsters[i];
        }
    }
}
