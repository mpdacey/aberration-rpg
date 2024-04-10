using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MonsterAffinityUIController : MonoBehaviour
{
    [System.Serializable]
    public struct AffinitySpritePair
    {
        public CombatantScriptableObject.AttributeAffinity key;
        public Sprite value;
    }

    // Sort affinity image array in this order: Fire, Ice, Thunder, Wind, Blunt, and Sharp.
    [Header("Object References")]
    public Image[] affinityImages;
    [Header("Sprites")]
    [SerializeField] private AffinitySpritePair[] affinitySpritePairs;
    [SerializeField] private Sprite unknownAffinitySprite;
    private Dictionary<CombatantScriptableObject.AttributeAffinity, Sprite> affinitySpriteDictionary;

    void Start()
    {
        affinitySpriteDictionary = new Dictionary<CombatantScriptableObject.AttributeAffinity, Sprite>();
        foreach (AffinitySpritePair pair in affinitySpritePairs) affinitySpriteDictionary.Add(pair.key, pair.value);
    }
}
