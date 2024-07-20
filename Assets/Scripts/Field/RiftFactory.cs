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
        if (!PartyController.partyMembers[0].HasValue) return;
        ClearRifts();

        foreach(var riftPosition in possibleRiftPositions)
        {
            int protagLuck = PartyController.partyMembers[0].Value.partyMemberBaseStats.combatantBaseStats.luck;
            float luckWeight = 1 - Mathf.Clamp((protagLuck * 0.4f - GameController.CurrentLevel) * 0.1f, -0.2f, 0.2f);
            for (int i = 0; i < rifts.Length; i++)
            {
                if (Random.value * luckWeight > rifts[i].spawnChance)
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
