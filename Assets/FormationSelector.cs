using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FormationSelector : MonoBehaviour
{
    [System.Serializable]
    public struct FormationTier
    {
        public FormationScriptableObject[] formations;
    }

    public FormationScriptableObject introFormation;
    [SerializeField] FormationTier[] formationTiers;
    private bool hasFoughtBefore = false;
    
    public FormationScriptableObject GetFormation()
    {
        if (!hasFoughtBefore)
        {
            hasFoughtBefore = true;
            return introFormation;
        }

        int tierRangeModifier = Random.Range(-5, 5)/2;
        int tierIndex = Mathf.Clamp(GameController.CurrentLevel / 3 + tierRangeModifier, 0, formationTiers.Length - 1);
        int randomEncounterIndex = Random.Range(0, formationTiers[tierIndex].formations.Length - 1);

        return formationTiers[tierIndex].formations[randomEncounterIndex];
    }
}
