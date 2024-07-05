using System;
using System.Collections;
using UnityEngine;

public class GoalRiftController : MonoBehaviour, IInteractable
{
    public static event Action GoalRiftEntered;

    public AudioClip playerWarpSFX;
    private bool portalIsActive;

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
        if(!portalIsActive)
            StartCoroutine(EnterRift());

        return true;
    }

    public IEnumerator EnterRift()
    {
        portalIsActive = true;
        yield return new WaitForSeconds(0.3f);

        AudioManager.PlayAudioClip(playerWarpSFX);
        if(GoalRiftEntered != null)
            GoalRiftEntered.Invoke();
    }

    private void SetEndGoal(Vector2 endLocation)
    {
        transform.position = new Vector3(endLocation.x*5, 0, endLocation.y * 5);
        portalIsActive = false;
    }
}
