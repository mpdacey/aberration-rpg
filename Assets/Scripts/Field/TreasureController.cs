using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class TreasureController : MonoBehaviour
{
    public static event Action<PartyController.PartyMember, PartyController.ProtagonistEquipment, EquipmentScriptableObject> TreasureEquipmentGenerated;

    [SerializeField] EquipmentScriptableObject[][] possibleEquipmentTreasure;
    PartyController partyController;

    private void Start()
    {
        partyController = GetComponent<PartyController>();
    }

    private void OnEnable()
    {
        FieldMovementController.TreasureFound += ProvideTreasureEquipment;
    }

    private void OnDisable()
    {
        FieldMovementController.TreasureFound -= ProvideTreasureEquipment;
    }

    public void ProvideTreasureEquipment()
    {
        if (possibleEquipmentTreasure == null)
        {
            Debug.LogError("There are no equipment tiers in treasure controller");
            return;
        }

        int tierRangeModifier = Random.Range(-5, 5) / 4;
        int tierIndex = Mathf.Clamp(GameController.CurrentLevel / 5 + tierRangeModifier, 0, possibleEquipmentTreasure.Length - 1);
        if (possibleEquipmentTreasure[tierIndex] == null)
        {
            Debug.LogError($"There are no equipment items in tier {tierIndex} in treasure controller");
            return;
        }

        int itemIndex = Mathf.Min(Mathf.FloorToInt(possibleEquipmentTreasure[tierIndex].Length * Random.value), possibleEquipmentTreasure[tierIndex].Length-1);

        EquipmentScriptableObject selectedEquipment = Instantiate(possibleEquipmentTreasure[tierIndex][itemIndex]);
        selectedEquipment.equipmentStats.strength += Mathf.Max(0,Random.Range(-3, 6));
        selectedEquipment.equipmentStats.magic += Mathf.Max(0, Random.Range(-3, 6));
        selectedEquipment.equipmentStats.endurance += Mathf.Max(0, Random.Range(-3, 6));
        selectedEquipment.equipmentStats.agility += Mathf.Max(0, Random.Range(-3, 6));
        selectedEquipment.equipmentStats.luck += Mathf.Max(0, Random.Range(-3, 6));

        if (TreasureEquipmentGenerated != null)
            TreasureEquipmentGenerated.Invoke(partyController.protagonist, partyController.protagonistEquipment, selectedEquipment);
    }
}
