using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreasureRiftController : GoalRiftController
{
    private void OnEnable()
    {
        
    }

    private void OnDisable()
    {

    }

    protected override void PerformAction()
    {
        Debug.Log("Treasure got");
    }
}
