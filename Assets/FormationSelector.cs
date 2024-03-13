using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FormationSelector : MonoBehaviour
{

    public FormationScriptableObject introFormation;
    private bool hasFoughtBefore = false;
    
    public FormationScriptableObject GetFormation()
    {
        return introFormation;
    }
}
