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
        public int currentMP;
    }

    public static PartyMember?[] partyMembers = new PartyMember?[4];
    public PartyMember protagonist;

    private void OnEnable()
    {
        partyMembers[0] = protagonist;
    }
}
