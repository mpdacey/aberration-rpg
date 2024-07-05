using System;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class TreasureRiftController : MonoBehaviour, IInteractable
{
    public static event Action TreasureFound;

    public bool Interact()
    {
        TreasureHandling();
        return false;
    }

    private void OnDisable()
    {
        // If somehow the rift gets disabled while equipment UI is available
        // this code should remove the event.
        try
        {
            EquipmentController.DismissRift -= DismissRift;
        }
        catch { };
    }

    private void TreasureHandling()
    {
        EquipmentController.DismissRift += DismissRift;
        if (TreasureFound != null)
            TreasureFound.Invoke();
    }

    private void DismissRift()
    {
        EquipmentController.DismissRift -= DismissRift;
        GetComponent<Animator>().Play("Dismiss");
    }
}
