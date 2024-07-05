using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class TreasureController : MonoBehaviour
{
    public static event Action<EquipmentScriptableObject> TreasureEquipmentGenerated;

    [Serializable]
    public struct EquipmentTierArray
    {
        public EquipmentScriptableObject[] equipmentItems;
    }

    [SerializeField] EquipmentTierArray[] equipmentTiers;

    private void OnEnable()
    {
        TreasureRiftController.TreasureFound += ProvideTreasureEquipment;
    }

    private void OnDisable()
    {
        TreasureRiftController.TreasureFound -= ProvideTreasureEquipment;
    }

    public void ProvideTreasureEquipment()
    {
        if (equipmentTiers == null)
        {
            Debug.LogError("There are no equipment tiers in treasure controller");
            return;
        }

        int tierRangeModifier = Random.Range(-5, 6) / 2;
        float tierQuality = Mathf.Max(GameController.CurrentLevel + tierRangeModifier, 0) / 7f;
        int tierIndex = Mathf.Clamp(Mathf.FloorToInt(tierQuality), 0, equipmentTiers.Length - 1);
        if (equipmentTiers[tierIndex].equipmentItems == null || equipmentTiers[tierIndex].equipmentItems.Length <= 0)
        {
            Debug.LogError($"There are no equipment items in tier {tierIndex} in treasure controller");
            return;
        }

        int itemIndex = Mathf.Clamp(Mathf.FloorToInt(equipmentTiers[tierIndex].equipmentItems.Length * Random.value), 0, equipmentTiers[tierIndex].equipmentItems.Length-1);

        EquipmentScriptableObject selectedEquipment = Instantiate(equipmentTiers[tierIndex].equipmentItems[itemIndex]);
        selectedEquipment.equipmentStats.strength += Mathf.Max(0,Random.Range(-2, 4));
        selectedEquipment.equipmentStats.magic += Mathf.Max(0, Random.Range(-2, 4));
        selectedEquipment.equipmentStats.endurance += Mathf.Max(0, Random.Range(-2, 4));
        selectedEquipment.equipmentStats.agility += Mathf.Max(0, Random.Range(-2, 4));
        selectedEquipment.equipmentStats.luck += Mathf.Max(0, Random.Range(-2, 4));

        bool upgraded = (tierQuality - Math.Truncate(tierQuality)) > 0.5f;
        if (upgraded)
        {
            selectedEquipment.equipmentName += "+";
            selectedEquipment.equipmentStats.strength += Mathf.Max(0, Random.Range(-1, 3));
            selectedEquipment.equipmentStats.magic += Mathf.Max(0, Random.Range(-1, 3));
            selectedEquipment.equipmentStats.endurance += Mathf.Max(0, Random.Range(-1, 3));
            selectedEquipment.equipmentStats.agility += Mathf.Max(0, Random.Range(-1, 3));
            selectedEquipment.equipmentStats.luck += Mathf.Max(0, Random.Range(-1, 3));
        }

        selectedEquipment.Id = equipmentTiers[tierIndex].equipmentItems[itemIndex].Id;

        if (TreasureEquipmentGenerated != null)
            TreasureEquipmentGenerated.Invoke(selectedEquipment);
    }
}
