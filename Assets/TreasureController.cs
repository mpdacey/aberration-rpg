using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreasureController : MonoBehaviour
{
    public GameObject treasureRiftPrefab;
    public Transform treasureContainer;
    [Range(0,1)]
    public float treasureSpawnChance = 0.4f;

    private void OnEnable()
    {
        LevelGenerator.TreasureLocationsFound += GenerateTreasure;
    }

    private void OnDisable()
    {
        LevelGenerator.TreasureLocationsFound -= GenerateTreasure;
    }

    private void GenerateTreasure(List<Vector2> possibleTreasurePositions)
    {
        ClearTreasure();

        foreach(var treasurePosition in possibleTreasurePositions)
        {
            if (Random.value > treasureSpawnChance)
                continue;

            var treasure = Instantiate(treasureRiftPrefab, treasureContainer);
            treasure.transform.position = new Vector3(treasurePosition.x * 5, 0, treasurePosition.y * 5);
        }
    }

    private void ClearTreasure()
    {
        for (int i = 0; i < treasureContainer.childCount; i++)
            Destroy(treasureContainer.GetChild(i).gameObject);
    }
}
