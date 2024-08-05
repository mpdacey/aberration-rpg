using System;
using System.Collections;
using UnityEngine;
using Cryptemental.Audio;

public class GoalRiftController : MonoBehaviour, IInteractable
{
    public static event Action GoalRiftEntered;
    public bool HasInteracted => hasInteracted;
    private bool hasInteracted = false;

    private void OnEnable()
    {
        LevelGenerator.GoalLocationFound += SetEndGoal;
    }

    private void OnDisable()
    {
        LevelGenerator.GoalLocationFound -= SetEndGoal;
    }

    public bool Interact()
    {
        hasInteracted = true;
        StartCoroutine(EnterRift());

        return true;
    }

    public IEnumerator EnterRift()
    {
        yield return new WaitForSeconds(0.26f);

        if(GoalRiftEntered != null)
            GoalRiftEntered.Invoke();
    }

    private void SetEndGoal(Vector2 endLocation)
    {
        transform.position = new Vector3(endLocation.x*5, 0, endLocation.y * 5);
        hasInteracted = false;
    }
}
