using UnityEngine;
using UnityEngine.UI;

public class TargetAffinityController : MonoBehaviour
{
    public Image[] affinityImages;
    [SerializeField] private AffinitySpritesScriptableObject affinitySpritesSO;

    private void OnEnable()
    {
        CombatController.SetTargetAffinity += SetTargetAffinities;
    }

    private void OnDisable()
    {
        CombatController.SetTargetAffinity -= SetTargetAffinities;
        DisableImages();
    }

    private void SetTargetAffinities(SpellScriptableObject.SpellType spellType)
    {
        switch (FormationSelector.CurrentFormation.monsters.Length)
        {
            case 1:
                SetTargetAffinitySprite(0, 1, spellType);
                break;
            case 2:
                SetTargetAffinitySprite(0, 0, spellType);
                SetTargetAffinitySprite(1, 2, spellType);
                break;
            case 3:
                SetTargetAffinitySprite(0, 0, spellType);
                SetTargetAffinitySprite(1, 1, spellType);
                SetTargetAffinitySprite(2, 2, spellType);
                break;
        }
    }

    private void SetTargetAffinitySprite(int formationIndex, int targetIndex, SpellScriptableObject.SpellType spellType)
    {
        bool affinityIsKnown = SeenMonsterAffinities.GetAffinityWitnesses(FormationSelector.CurrentFormation.monsters[formationIndex])[(int)spellType];
        var affinity = FormationSelector.CurrentFormation.monsters[formationIndex].combatantAttributes[spellType];
        if (!affinityIsKnown)
            affinityImages[targetIndex].sprite = affinitySpritesSO.unknownAffinitySprite;
        else if (affinity != CombatantScriptableObject.AttributeAffinity.None)
            affinityImages[targetIndex].sprite = affinitySpritesSO.affinitySpriteDictionary[affinity];

        affinityImages[targetIndex].enabled = !(affinity == CombatantScriptableObject.AttributeAffinity.None && affinityIsKnown);
    }

    private void DisableImages()
    {
        foreach (var image in affinityImages) image.enabled = false;
    }
}
