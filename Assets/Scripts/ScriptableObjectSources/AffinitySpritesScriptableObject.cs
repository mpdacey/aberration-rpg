using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AffinitySprites", menuName = "Scriptable Objects/Storage/Affinity Sprites")]
public class AffinitySpritesScriptableObject : ScriptableObject
{
    [System.Serializable]
    public struct AffinitySpritePair
    {
        public CombatantScriptableObject.AttributeAffinity key;
        public Sprite value;
    }

    [SerializeField] private AffinitySpritePair[] affinitySpritePairs;
    public Sprite unknownAffinitySprite;
    public Dictionary<CombatantScriptableObject.AttributeAffinity, Sprite> affinitySpriteDictionary;

    private void OnEnable()
    {
        affinitySpriteDictionary = new Dictionary<CombatantScriptableObject.AttributeAffinity, Sprite>();
        foreach (AffinitySpritePair pair in affinitySpritePairs) affinitySpriteDictionary.Add(pair.key, pair.value);
    }
}
