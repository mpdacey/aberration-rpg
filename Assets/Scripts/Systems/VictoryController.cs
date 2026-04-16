using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class VictoryController : GameoverController
{
    public UnityEvent VictoryAchievedEvent;
    static public UnityAction VictoryAchievedAction;

    const int VICTORY_REALM = 10;

    private void OnEnable()
    {
        GoalRiftController.GoalRiftEntered += OnRiftEntered;
    }

    private void OnDisable()
    {
        GoalRiftController.GoalRiftEntered -= OnRiftEntered;
    }

    private void OnRiftEntered()
    {
        UpdateFloorCounter();
        if (int.Parse(gameoverFloorCounter.text) >= VICTORY_REALM)
        {
            VictoryAchievedEvent.Invoke();
            VictoryAchievedAction.Invoke();
        }
    }
}
