using UnityEngine;

[CreateAssetMenu(fileName = "Spell", menuName ="Scriptable Objects/Spell")]
public class SpellScriptableObject : ScriptableObject
{
    public enum SpellType
    {
        Fire,
        Ice,
        Thunder,
        Wind,
        Blunt,
        Sharp
    }

    public string spellName;
    public SpellType spellType;
    public float spellBaseDamage;
    public float spellHitRate;
    public int spellCost;
    public bool spellMultitarget;
}
