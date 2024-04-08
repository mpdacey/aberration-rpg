using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DamageVFXStateNames", menuName = "Scriptable Objects/Damage VFX States")]
public class DamageVFXScriptableObject : ScriptableObject
{
    [System.Serializable]
    public struct DamageVFXAnimationPair
    {
        public SpellScriptableObject.SpellType spellType;
        public string spellAnimationKey;
    }

    public DamageVFXAnimationPair[] vfxPairs;
    public Dictionary<SpellScriptableObject.SpellType, string> vfxDictionary = new Dictionary<SpellScriptableObject.SpellType, string>();

    void OnEnable()
    {
        foreach (var pair in vfxPairs) vfxDictionary.Add(pair.spellType, pair.spellAnimationKey);
    }
}
