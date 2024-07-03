using System;
using UnityEngine;

public class StaminaRiftController : MonoBehaviour, IInteractable
{
    public static event Action<float> RecoverStamina;

    [Range(0, 1)]
    public float percentStaminaReplenished = 0.15f;

    public void Interact()
    {
        ReplenishStamina();
    }

    private void ReplenishStamina()
    {
        if (RecoverStamina != null)
            RecoverStamina.Invoke(percentStaminaReplenished);

        Dismiss();
    }

    private void Dismiss()
    {
        GetComponent<Animator>().Play("Dismiss");
    }
}
