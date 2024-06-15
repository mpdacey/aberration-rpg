using System;
using System.Collections;
using UnityEngine;
using OfferState = RecruitmentController.RecruitmentState;
using EquipmentType = EquipmentScriptableObject.EquipmentType;

public class EquipmentController : MonoBehaviour
{
    public static event Action EquipmentUpdated;

    public EquipmentUIController uiController;
    private OfferState state = OfferState.Decline;
    private EquipmentScriptableObject incomingEquipment;
    private Animator treasureRiftAnimator = null;


    public EquipmentScriptableObject testEquipment;

    private void OnEnable()
    {
        TreasureController.TreasureEquipmentGenerated += ContructOffer;
        FieldMovementController.TreasureFoundAnimator += GetTreasureRiftAnimator;
        DataManager.LoadEquipment += LoadEquipment;
    }

    private void OnDisable()
    {
        TreasureController.TreasureEquipmentGenerated -= ContructOffer;
        FieldMovementController.TreasureFoundAnimator -= GetTreasureRiftAnimator;
        DataManager.LoadEquipment -= LoadEquipment;
    }
    public void SetOfferState(int value) =>
        state = (OfferState)value;

    public void ContructOffer(EquipmentScriptableObject incomingEquipment)=>
        StartCoroutine(PitchOffer(incomingEquipment));

    private void GetTreasureRiftAnimator(Animator animator) =>
        treasureRiftAnimator = animator;

    public void SwapTrinket(int index)
    {
        if(incomingEquipment.equipmentType == EquipmentType.Trinket)
            uiController.CompareEquipment(PartyController.partyMembers[0].Value, PartyController.protagonistEquipment, incomingEquipment, index);
    }

    IEnumerator PitchOffer(EquipmentScriptableObject proposedEquipment)
    {
        incomingEquipment = proposedEquipment;

        uiController.gameObject.SetActive(true);
        uiController.InitialiseEquipmentUI(PartyController.protagonistEquipment, incomingEquipment);
        uiController.CompareEquipment(PartyController.partyMembers[0].Value, PartyController.protagonistEquipment, incomingEquipment);
        FieldMovementController.lockedInPlace = true;

        state = OfferState.Pending;
        while (state == OfferState.Pending)
        {
            yield return null;
        }

        switch (state)
        {
            case OfferState.OfferA:
                CarryOutOffer(incomingEquipment);
                break;
            case OfferState.OfferB:
                CarryOutOffer(incomingEquipment, false);
                break;
        }

        uiController.gameObject.SetActive(false);
        FieldMovementController.lockedInPlace = false;

        if(treasureRiftAnimator != null)
        {
            treasureRiftAnimator.Play("Dismiss");
            treasureRiftAnimator = null;
        }
    }

    private void CarryOutOffer(EquipmentScriptableObject incomingEquipment, bool leftSlot = true)
    {
        switch (incomingEquipment.equipmentType)
        {
            case EquipmentType.Weapon:
                PartyController.protagonistEquipment.weapon = incomingEquipment;
                break;
            case EquipmentType.Armour:
                PartyController.protagonistEquipment.defense = incomingEquipment;
                break;
            case EquipmentType.Trinket:
                if(leftSlot) PartyController.protagonistEquipment.trinkets[0] = incomingEquipment;
                else PartyController.protagonistEquipment.trinkets[1] = incomingEquipment;
                break;
        }

        if (EquipmentUpdated != null)
            EquipmentUpdated.Invoke();
    }

    private void LoadEquipment(DataManager.EquipmentState incomingEquipment)
    {
        PartyController.protagonistEquipment.weapon = incomingEquipment.weapon;
        PartyController.protagonistEquipment.defense = incomingEquipment.armour;
        PartyController.protagonistEquipment.trinkets[0] = incomingEquipment.trinkets[0];
        PartyController.protagonistEquipment.trinkets[1] = incomingEquipment.trinkets[1];

        if (EquipmentUpdated != null)
            EquipmentUpdated.Invoke();
    }
}
