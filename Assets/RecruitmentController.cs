using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecruitmentController : MonoBehaviour
{
    public RecruitmentUIController uiController;

    private enum RecruitmentState
    {
        Pending,
        OfferA,
        OfferB,
        Decline
    }

    private RecruitmentState state;

    public IEnumerator PitchRecruitment(FormationScriptableObject currentFormation)
    {
        state = RecruitmentState.Pending;
        List<int> availableIndexes = new List<int>();
        for (int i = 0; i < 4; i++)
            if (PartyController.partyMembers[i].HasValue)
                availableIndexes.Add(i);

        CombatantScriptableObject requestingCreature = currentFormation.monsters[Random.Range(0, currentFormation.monsters.Length-1)];

        int minIndex = availableIndexes.Count == 4 ? 1 : 0;
        int leftOfferIndex = availableIndexes[Random.Range(minIndex, availableIndexes.Count - 1)];
        int rightOfferIndex = leftOfferIndex;

        if(availableIndexes.Count > 1)
        {
            do
            {
                rightOfferIndex = availableIndexes[Random.Range(minIndex, availableIndexes.Count - 1)];
            } while (rightOfferIndex == leftOfferIndex);
        }

        uiController.gameObject.SetActive(true);
        uiController.rightOffer.transform.parent.gameObject.SetActive(availableIndexes.Count > 1);

        string leftOffer;
        if (leftOfferIndex == 0) leftOffer = "Sacrifice 40HP";
        else leftOffer = $"Offer {PartyController.partyMembers[leftOfferIndex].Value.partyMemberBaseStats.combatantName}";

        string rightOffer;
        if (rightOfferIndex == 0) rightOffer = "Sacrifice 40HP";
        else rightOffer = $"Offer {PartyController.partyMembers[rightOfferIndex].Value.partyMemberBaseStats.combatantName}";

        uiController.SetOffers(leftOffer, rightOffer);

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
            temp.currentHP -= 40;
            PartyController.partyMembers[0] = temp;

            for(int i = 1; i < 4; i++)
            {
                if (!PartyController.partyMembers[i].HasValue)
                    PartyController.SetPartyMember(newPartyMember, i);
            }
        }
        else
        {
            PartyController.SetPartyMember(newPartyMember, selectedSacrifice);
        }
    }
}
