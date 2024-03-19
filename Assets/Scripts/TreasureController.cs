using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreasureController : MonoBehaviour
{
    public GameObject treasurePrefab;

    private void OnEnable()
    {
        LevelGenerator.TreasureLocations += GenerateNewTreasure;
    }

    private void OnDisable()
    {
        LevelGenerator.TreasureLocations -= GenerateNewTreasure;
    }

    private void GenerateNewTreasure(List<Vector2> treasureLocations)
    {
        DestroyAllTreasure();

        foreach(var treasurePosition in treasureLocations)
        {
            GameObject treasure = Instantiate(treasurePrefab);
            treasure.transform.parent = transform;
            treasure.GetComponent<TreasureRiftController>().SetInitialPosition(treasurePosition);
        }
    }

    private void DestroyAllTreasure()
    {
        for (int i = 0; i < transform.childCount; i++)
            Destroy(transform.GetChild(i).gameObject);
    }
}
