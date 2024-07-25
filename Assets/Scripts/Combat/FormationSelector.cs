using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class FormationSelector : MonoBehaviour
{
    public static event Action FormationSelected;
    public static event Action<int> FormationThreatLevel;

    [Serializable]
    public struct FormationTier
    {
        public FormationScriptableObject[] formations;
    }

    public static FormationScriptableObject CurrentFormation { get => currentFormation; }
    private static FormationScriptableObject currentFormation;
    public FormationScriptableObject introFormation;
    [SerializeField] FormationTier[] formationTiers;
    private bool hasFoughtBefore = false;

    private void OnEnable()
    {
        MonsterEncounterController.ThreatTriggered += GetFormation;
        GameController.ResetGameEvent += ResetHasFoughtBefore;
        GameController.ContinueGameEvent += TickHasFoughtBefore;
    }

    private void OnDisable()
    {
        MonsterEncounterController.ThreatTriggered -= GetFormation;
        GameController.ResetGameEvent -= ResetHasFoughtBefore;
        GameController.ContinueGameEvent -= TickHasFoughtBefore;
    }

    private void GetFormation()
    {
        if (!hasFoughtBefore)
        {
            TickHasFoughtBefore();
            InvokeFormation(introFormation);
            InvokeThreatLevelEvent(-1);
            return;
        }

        int tierRangeModifier = Random.Range(-5, 5)/2;
        int tierIndex = Mathf.Clamp(GameController.CurrentLevel / 3 + tierRangeModifier, 0, formationTiers.Length - 1);
        int randomEncounterIndex = Random.Range(0, formationTiers[tierIndex].formations.Length);

        InvokeFormation(formationTiers[tierIndex].formations[randomEncounterIndex]);
        InvokeThreatLevelEvent(tierIndex - GameController.CurrentLevel / 3);
    }

    private void InvokeFormation(FormationScriptableObject formation)
    {
        currentFormation = formation;
        if (FormationSelected != null)
            FormationSelected.Invoke();
    }

    private void TickHasFoughtBefore() =>
        hasFoughtBefore = true;

    private void ResetHasFoughtBefore() =>
        hasFoughtBefore = false;

    private void InvokeThreatLevelEvent(int threatLevel)
    {
        if (FormationThreatLevel != null)
            FormationThreatLevel.Invoke(threatLevel);
    }
        
}
