using System;
using System.Collections.Generic;
using UnityEngine;

public class SpellIcons : MonoBehaviour
{
    public static Dictionary<SpellScriptableObject.SpellType, Sprite> icons = new Dictionary<SpellScriptableObject.SpellType, Sprite>();
    public static Sprite empty;
    public static Sprite guard;

    [Serializable]
    private struct IconPair{
        public SpellScriptableObject.SpellType key;
        public Sprite value;
    }

    [SerializeField] IconPair[] localIconPairs;
    [SerializeField] Sprite noneSprite;
    [SerializeField] Sprite guardSprite;

    private void Start()
    {
        foreach (IconPair pair in localIconPairs)
            icons.Add(pair.key, pair.value);

        empty = noneSprite;
        guard = guardSprite;
    }
}
