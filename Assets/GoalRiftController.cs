using System;
using UnityEngine;

public class GoalRiftController : MonoBehaviour
{
    public static event Action GoalRiftEntered;

    public AudioClip playerWarpSFX;
    private Transform player;

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
        if (player == null) player = GetComponent<RotateTowardsCamera>().player;
        CheckForPlayer(player);
    }

    private void CheckForPlayer(Transform player)
    {
        if (Vector3.Distance(player.position, transform.position) < 1f && GoalRiftEntered != null)
        {
            AudioManager.PlayAudioClip(playerWarpSFX);
            GoalRiftEntered.Invoke();
        }
    }

    private void SetEndGoal(Vector2 endLocation)
    {
        transform.position = new Vector3(endLocation.x*5, 0, endLocation.y * 5);
    }
}
