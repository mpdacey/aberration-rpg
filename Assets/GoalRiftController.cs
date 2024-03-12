using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalRiftController : MonoBehaviour
{
    public Transform player;

    private void OnEnable()
    {
        LevelGenerator.GoalLocationFound += SetEndGoal;
    }

    private void OnDisable()
    {
        LevelGenerator.GoalLocationFound -= SetEndGoal;
    }

    private void Update()
    {
        CheckForPlayer(player);
    }

    private void CheckForPlayer(Transform player)
    {
        transform.rotation = player.rotation;

        if (player.position == transform.position)
            Debug.Log("Goal found");
    }

    private void SetEndGoal(Vector2 endLocation)
    {
        transform.position = new Vector3(endLocation.x*5, 0, endLocation.y * 5);
    }
}
