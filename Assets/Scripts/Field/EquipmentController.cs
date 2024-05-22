using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentController : MonoBehaviour
{
    public EquipmentUIController uiController;
    RecruitmentController.OfferState offerState = RecruitmentController.OfferState.Decline;

    private void OnEnable()
    {
        TreasureController.TreasureEquipmentGenerated += ContructOffer;
    }

    private void OnDisable()
    {
        TreasureController.TreasureEquipmentGenerated -= ContructOffer;
    }

    private void ContructOffer(EquipmentScriptableObject incomingEquipment)
    {
        uiController.CompareEquipment(PartyController.partyMembers[0].Value, PartyController.protagonistEquipment, incomingEquipment);
    }
}
