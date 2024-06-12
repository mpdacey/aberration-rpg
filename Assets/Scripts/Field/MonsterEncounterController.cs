using System;
using UnityEngine;

public class MonsterEncounterController : MonoBehaviour
{
    public static event Action<ThreatLevel> ThreatLevelChanged;
    public static event Action ThreatTriggered;

    public enum ThreatLevel
    {
        Safe,
        Low,
        Medium,
        High
    }

    public int levelRankUpMax = 7;
    ThreatLevel threatLevel = ThreatLevel.Safe;
    int timer = 0;
    private Vector3 goalPosition;

    private void OnEnable()
    {
        FieldMovementController.PlayerPositionChanged += UpdateThreatLevel;
        CombatController.CombatVictory += ResetThreatLevel;
        GoalRiftController.GoalRiftEntered += ResetThreatLevel;
        GameController.ResetGameEvent += ResetThreatLevel;
        LevelGenerator.GoalLocationFound += SetGoalPosition;
    }

    private void OnDisable()
    {
        FieldMovementController.PlayerPositionChanged -= UpdateThreatLevel;
        CombatController.CombatVictory -= ResetThreatLevel;
        GoalRiftController.GoalRiftEntered -= ResetThreatLevel;
        GameController.ResetGameEvent -= ResetThreatLevel;
        LevelGenerator.GoalLocationFound -= SetGoalPosition;
    }

    private void SetGoalPosition(Vector2 goal) =>
        goalPosition = new Vector3(goal.x, 0, goal.y);

    private void UpdateThreatLevel(Vector3 playerPosition)
    {
        if (UnityEngine.Random.Range(0, levelRankUpMax) < timer)
        {
            timer = 0;
            if (threatLevel == ThreatLevel.High)
            {
                if (ThreatTriggered != null && Vector3.Distance(playerPosition, goalPosition*5) > 9.5f)
                    ThreatTriggered.Invoke();
            }
            else
                threatLevel++;

            if (ThreatLevelChanged != null)
                ThreatLevelChanged.Invoke(threatLevel);
        }
        else timer++;
    }

    private void ResetThreatLevel()
    {
        threatLevel = 0;
        if (ThreatLevelChanged != null)
            ThreatLevelChanged.Invoke(threatLevel);
    }
}
