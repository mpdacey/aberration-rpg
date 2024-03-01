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
        Physical
    }

    public string spellName;
    public SpellType spellType;
    public float spellBaseDamage;
    public int spellCost;
    public float spellHitRate;
    public bool spellMultitarget;
}
