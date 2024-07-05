using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RiftFactory : MonoBehaviour
{
    [System.Serializable]
    private struct RiftType
    {
        public GameObject riftPrefab;
        [Range(0, 1)]
        public float spawnChance;

    }

    public Transform riftContainer;
    [SerializeField]
    private RiftType[] rifts;

    private void OnEnable()
    {
        LevelGenerator.RiftLocationsFound += GenerateTreasure;
    }

    private void OnDisable()
    {
        LevelGenerator.RiftLocationsFound -= GenerateTreasure;
    }

    private void GenerateTreasure(List<Vector2> possibleRiftPositions)
    {
        ClearRifts();

        foreach(var riftPosition in possibleRiftPositions)
        {
            for(int i = 0; i < rifts.Length; i++)
            {
                if (Random.value > rifts[i].spawnChance)
                    continue;

                var rift = Instantiate(rifts[i].riftPrefab, riftContainer);
                rift.transform.position = new Vector3(riftPosition.x * 5, 0, riftPosition.y * 5);
                break;
            }
        }
    }

    private void ClearRifts()
    {
        for (int i = 0; i < riftContainer.childCount; i++)
            Destroy(riftContainer.GetChild(i).gameObject);
    }
}
