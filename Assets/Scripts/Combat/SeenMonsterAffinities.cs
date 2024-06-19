using System.Collections.Generic;

public class SeenMonsterAffinities
{
    private static Dictionary<CombatantScriptableObject, bool[]> seenMonsterAffinities = new Dictionary<CombatantScriptableObject, bool[]>();

    public static Dictionary<CombatantScriptableObject, bool[]> GetAllSeenAffinities()
    {
        return seenMonsterAffinities;
    }

    public static bool[] GetAffinityWitnesses(CombatantScriptableObject monster)
    {
        if (!seenMonsterAffinities.ContainsKey(monster))
            seenMonsterAffinities.Add(monster, new bool[6]);

        return seenMonsterAffinities[monster];
    }

    public static void UpdateAffinityWitness(CombatantScriptableObject monster, SpellScriptableObject.SpellType afflictedSpellType)
    {
        bool[] witnessedAffinties = GetAffinityWitnesses(monster);
        witnessedAffinties[(int)afflictedSpellType] = true;
        seenMonsterAffinities[monster] = witnessedAffinties;
    }

    public static void ClearSeenAffinity() =>
        seenMonsterAffinities.Clear();
}
