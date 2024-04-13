using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MonsterAffinityUIController : MonoBehaviour
{
    // Sort affinity image array in this order: Fire, Ice, Thunder, Wind, Blunt, and Sharp.
    [Header("Object References")]
    public TextMeshProUGUI monsterNameText;
    public Image[] affinityImages;
    [Header("Sprites")]
    [SerializeField] private AffinitySpritesScriptableObject affinitySpritesSO;

    private FormationScriptableObject currentFormation;

    private void OnEnable()
    {
        FormationSelector.FormationSelected += SetFormation;
        MonsterTargetOnSelect.MonsterTargetSelected += SetSelectedMonsterAffinities;
    }

    private void OnDisable()
    {
        FormationSelector.FormationSelected -= SetFormation;
        MonsterTargetOnSelect.MonsterTargetSelected -= SetSelectedMonsterAffinities;
    }

    private void SetFormation(FormationScriptableObject formation) =>
        currentFormation = formation;

    private void SetSelectedMonsterAffinities(int selectedMonsterIndex)
    {
        switch (currentFormation.monsters.Length)
        {
            case 1:
                SetMonsterAffinities(currentFormation.monsters[0]);
                break;
            case 2:
                SetMonsterAffinities(currentFormation.monsters[selectedMonsterIndex/2]);
                break;
            case 3:
                SetMonsterAffinities(currentFormation.monsters[selectedMonsterIndex]);
                break;
        }
    }

    private void SetMonsterAffinities(CombatantScriptableObject monster)
    {
        monsterNameText.text = monster.combatantName;
        bool[] knownAffinities = SeenMonsterAffinities.GetAffinityWitnesses(monster);

        for(int i = 0; i < knownAffinities.Length; i++)
        {
            if (knownAffinities[i]) affinityImages[i].sprite = affinitySpritesSO.affinitySpriteDictionary[monster.combatantAttributes[(SpellScriptableObject.SpellType)i]];
            else affinityImages[i].sprite = affinitySpritesSO.unknownAffinitySprite;
        }
    }

    public void ShowMonsterAffinities() =>
        transform.GetChild(0).gameObject.SetActive(true);

    public void HideMonsterAffinities()=>
        transform.GetChild(0).gameObject.SetActive(false);
}
