using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DangerGaugeUIController : MonoBehaviour
{
    [System.Serializable]
    public struct ThreatColourPairs
    {
        public MonsterEncounterController.ThreatLevel key;
        public Color value;
    }

    public Image guageImage;
    public ThreatColourPairs[] threatColourPairs;
    Dictionary<MonsterEncounterController.ThreatLevel, Color> threatLevelColours = new Dictionary<MonsterEncounterController.ThreatLevel, Color>();
    
    void Start()
    {
        foreach (var pair in threatColourPairs)
            threatLevelColours.Add(pair.key, pair.value);
    }

    private void OnEnable()
    {
        MonsterEncounterController.ThreatLevelChanged += ChangeGaugeColour;
    }

    private void OnDisable()
    {
        MonsterEncounterController.ThreatLevelChanged -= ChangeGaugeColour;
    }

    void ChangeGaugeColour(MonsterEncounterController.ThreatLevel threatLevel)
    {
        Debug.Log(threatLevel);
        guageImage.color = threatLevelColours[threatLevel];
    }
}
