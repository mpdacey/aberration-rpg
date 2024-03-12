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

    private void OnEnable()
    {
        FieldMovementController.FieldMovementEvent += UpdateThreatLevel;
        CombatController.CombatVictory += ResetThreatLevel;
    }

    private void OnDisable()
    {
        FieldMovementController.FieldMovementEvent -= UpdateThreatLevel;
        CombatController.CombatVictory -= ResetThreatLevel;
    }

    private void UpdateThreatLevel()
    {
        if (UnityEngine.Random.Range(0, levelRankUpMax) < timer)
        {
            timer = 0;
            if (threatLevel == ThreatLevel.High)
            {
                if (ThreatTriggered != null) ThreatTriggered.Invoke();
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
