using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Random = UnityEngine.Random;

public class RecruitmentController : MonoBehaviour
{
    public static event Action<PartyController.PartyMember, int> UpdatePlayerHP;

    public RecruitmentUIController uiController;
    [SerializeField] private int recruitmentCost = 40;

    public enum RecruitmentState
    {
        Pending,
        OfferA,
        OfferB,
        Decline
    }

    private RecruitmentState state;

    public IEnumerator PitchRecruitment(FormationScriptableObject currentFormation)
    {
        List<int> availableIndexes = new List<int>();
        for (int i = 0; i < 4; i++)
            if (PartyController.partyMembers[i].HasValue)
                availableIndexes.Add(i);

        CombatantScriptableObject requestingCreature = currentFormation.monsters[Random.Range(0, currentFormation.monsters.Length-1)];

        int leftOfferIndex;
        int rightOfferIndex;

        if (availableIndexes.Count == 4)
        {
            leftOfferIndex = availableIndexes[DateTime.Now.Millisecond % (availableIndexes.Count-1) + 1];
            rightOfferIndex = availableIndexes[(DateTime.Now.Millisecond + 1) % (availableIndexes.Count-1) + 1];
        }
        else
        {
            leftOfferIndex = availableIndexes[DateTime.Now.Millisecond % availableIndexes.Count];
            rightOfferIndex = availableIndexes[(DateTime.Now.Millisecond + 1) % availableIndexes.Count];
        }

        uiController.gameObject.SetActive(true);
        uiController.rightOffer.transform.parent.gameObject.SetActive(availableIndexes.Count > 1);

        string leftOffer;
        if (leftOfferIndex == 0) leftOffer = $"Sacrifice {recruitmentCost}HP";
        else leftOffer = $"Offer {PartyController.partyMembers[leftOfferIndex].Value.partyMemberBaseStats.combatantName} ({leftOfferIndex})";

        string rightOffer;
        if (rightOfferIndex == 0) rightOffer = $"Sacrifice {recruitmentCost}HP";
        else rightOffer = $"Offer {PartyController.partyMembers[rightOfferIndex].Value.partyMemberBaseStats.combatantName} ({rightOfferIndex})";

        uiController.SetOffers(leftOffer, rightOffer);

        state = RecruitmentState.Pending;
        while (state == RecruitmentState.Pending)
        {
            yield return null;
        }

        switch (state)
        {
            case RecruitmentState.OfferA:
                CarryOutOffer(leftOfferIndex, requestingCreature);
                break;
            case RecruitmentState.OfferB:
                CarryOutOffer(rightOfferIndex, requestingCreature);
                break;
        }

        uiController.gameObject.SetActive(false);
    }

    public void SetRecruitmentState(int value) =>
        state = (RecruitmentState)value;

    private void CarryOutOffer(int selectedSacrifice, CombatantScriptableObject recruit)
    {
        PartyController.PartyMember newPartyMember = new PartyController.PartyMember();
        newPartyMember.partyMemberBaseStats = recruit;

        if (selectedSacrifice == 0)
        {
            var temp = PartyController.partyMembers[0].Value;
            temp.currentHP -= recruitmentCost;
            PartyController.partyMembers[0] = temp;
            if (UpdatePlayerHP != null)
                UpdatePlayerHP.Invoke(temp, 0);

            for(int i = 1; i < 4; i++)
            {
                if (!PartyController.partyMembers[i].HasValue)
                {
                    PartyController.SetPartyMember(newPartyMember, i);
                    break;
                }
            }
        }
        else
        {
            PartyController.SetPartyMember(newPartyMember, selectedSacrifice);
        }
    }
}
