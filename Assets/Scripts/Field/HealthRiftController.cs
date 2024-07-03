using System;
using UnityEngine;

public class HealthRiftController : MonoBehaviour, IInteractable
{
    public static event Action<float> RecoverHealth;

    [Range(0, 1)]
    public float percentHealthReplenished = 0.3f;

    public void Interact()
    {
        ReplenishHealth();
    }

    private void ReplenishHealth()
    {
        if (RecoverHealth != null)
            RecoverHealth.Invoke(percentHealthReplenished);

        Dismiss();
    }

    private void Dismiss()
    {
        GetComponent<Animator>().Play("Dismiss");
    }
}
