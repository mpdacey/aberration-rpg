using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundEffectsStorage : MonoBehaviour
{
    public static Dictionary<SpellScriptableObject.SpellType, AudioClip> spellSFXDictionary;

    [System.Serializable]
    public struct SpellSFX
    {
        public SpellScriptableObject.SpellType key;
        public AudioClip value;
    }

    public SpellSFX[] spellSFXs;

    private void OnEnable()
    {
        spellSFXDictionary = new Dictionary<SpellScriptableObject.SpellType, AudioClip>();

        foreach (var item in spellSFXs)
            spellSFXDictionary.Add(item.key, item.value);
    }
}
