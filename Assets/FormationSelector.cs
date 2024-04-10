using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class FormationSelector : MonoBehaviour
{
    public static event Action<FormationScriptableObject> FormationSelected;

    [Serializable]
    public struct FormationTier
    {
        public FormationScriptableObject[] formations;
    }

    public FormationScriptableObject introFormation;
    [SerializeField] FormationTier[] formationTiers;
    [SerializeField] private bool hasFoughtBefore = false;

    private void OnEnable()
    {
        MonsterEncounterController.ThreatTriggered += GetFormation;
    }

    private void OnDisable()
    {
        MonsterEncounterController.ThreatTriggered -= GetFormation;
    }

    private void GetFormation()
    {
        if (!hasFoughtBefore)
        {
            hasFoughtBefore = true;
            InvokeFormation(introFormation);
            return;
        }

        int tierRangeModifier = Random.Range(-5, 5)/2;
        int tierIndex = Mathf.Clamp(GameController.CurrentLevel / 3 + tierRangeModifier, 0, formationTiers.Length - 1);
        int randomEncounterIndex = Random.Range(0, formationTiers[tierIndex].formations.Length - 1);

        InvokeFormation(formationTiers[tierIndex].formations[randomEncounterIndex]);
    }

    private void InvokeFormation(FormationScriptableObject formation)
    {
        if (FormationSelected != null)
            FormationSelected.Invoke(formation);
    }
        
}
