using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Linq;

public class EquipmentUIController : MonoBehaviour
{
    [System.Serializable]
    public struct EquipmentStatUISlider
    {
        public Slider baseSlider;
        public Slider positiveSlider;
        public Slider negativeSlider;
        public TextMeshProUGUI sliderValueText;
    }

    struct SpellUIObject
    {
        public GameObject spellGameObject;
        public Image spellBackground;
        public Image spellIcon;
        public TextMeshProUGUI inclusionSymbol;
        public TextMeshProUGUI spellName;
        public TextMeshProUGUI spellCost;
    }

    [SerializeField] EquipmentStatUISlider[] statSliders;
    [SerializeField] Transform spellUIContainer;
    [SerializeField] Image[] affinityImages;
    [SerializeField] TextMeshProUGUI titleText;
    [SerializeField] TextMeshProUGUI acceptButtonText;
    [SerializeField] Color positiveColour;
    [SerializeField] Color negativeColour;
    [SerializeField] AffinitySpritesScriptableObject affinitySprites;
    SpellUIObject[] spellObjects;
    int currentDisplayedSpellIndex = 0;
    EquipmentScriptableObject currentEquipment;

    private void Start()
    {
        InitialiseSpellList();
    }

    public void CompareEquipment(PartyController.PartyMember protagonist, PartyController.ProtagonistEquipment allEquipment, EquipmentScriptableObject incomingEquipment)
    {
        switch (incomingEquipment.equipmentType)
        {
            case EquipmentScriptableObject.EquipmentType.Weapon:
                currentEquipment = allEquipment.weapon;
                break;
            case EquipmentScriptableObject.EquipmentType.Armour:
                currentEquipment = allEquipment.defense;
                break;
            case EquipmentScriptableObject.EquipmentType.Trinket:
                // TODO: have logic for multiple trinkets.
                currentEquipment = allEquipment.trinkets[0];
                break;
            default: 
                currentEquipment = allEquipment.weapon;
                break;
        }

        titleText.text = $"Aquired\n{incomingEquipment.equipmentName}";
        acceptButtonText.text = $"Replace {currentEquipment.equipmentName}";

        SetStatSliders(statSliders[0], protagonist.partyMemberBaseStats.combatantBaseStats.strength, currentEquipment.equipmentStats.strength, incomingEquipment.equipmentStats.strength);
        SetStatSliders(statSliders[1], protagonist.partyMemberBaseStats.combatantBaseStats.magic, currentEquipment.equipmentStats.magic, incomingEquipment.equipmentStats.magic);
        SetStatSliders(statSliders[2], protagonist.partyMemberBaseStats.combatantBaseStats.agility, currentEquipment.equipmentStats.agility, incomingEquipment.equipmentStats.agility);
        SetStatSliders(statSliders[3], protagonist.partyMemberBaseStats.combatantBaseStats.endurance, currentEquipment.equipmentStats.endurance, incomingEquipment.equipmentStats.endurance);
        SetStatSliders(statSliders[4], protagonist.partyMemberBaseStats.combatantBaseStats.luck, currentEquipment.equipmentStats.luck, incomingEquipment.equipmentStats.luck);

        SetEquipmentSpells(allEquipment, currentEquipment.equipmentType, currentEquipment.equipmentSpells, incomingEquipment.equipmentSpells);

        SetAffinities(allEquipment, currentEquipment.equipmentType, currentEquipment.equipmentAffinties, incomingEquipment.equipmentAffinties);
    }

    private void SetStatSliders(EquipmentStatUISlider stat, int cumulativeStat, int currentStat, int incomingStat)
    {
        stat.sliderValueText.text = incomingStat.ToString();

        // Worse stat
        if(currentStat > incomingStat)
        {
            stat.baseSlider.value = cumulativeStat + currentStat - incomingStat;
            stat.negativeSlider.value = cumulativeStat;
            stat.positiveSlider.value = 0;
            stat.sliderValueText.text += $"<color=#{ColorUtility.ToHtmlStringRGB(negativeColour)}> -{currentStat-incomingStat}</color>";
        }
        // Better stat
        else if (currentStat < incomingStat)
        {
            stat.baseSlider.value = cumulativeStat;
            stat.negativeSlider.value = 0;
            stat.positiveSlider.value = cumulativeStat + incomingStat - currentStat;
            stat.sliderValueText.text += $"<color=#{ColorUtility.ToHtmlStringRGB(positiveColour)}> +{incomingStat - currentStat}</color>";
        }
        else
        {
            stat.baseSlider.value = cumulativeStat;
            stat.negativeSlider.value = 0;
            stat.positiveSlider.value = 0;
        }
    }

    private void InitialiseSpellList()
    {
        spellObjects = new SpellUIObject[4];

        for (int i = 0; i < spellUIContainer.childCount; i++)
        {
            Transform currentChild = spellUIContainer.GetChild(i);

            SpellUIObject currentObject = new();
            currentObject.spellGameObject = currentChild.gameObject;
            currentObject.spellBackground = currentChild.GetComponent<Image>();
            currentObject.inclusionSymbol = currentChild.GetChild(0).GetComponent<TextMeshProUGUI>();
            currentObject.spellIcon = currentChild.GetChild(1).GetChild(0).GetComponent<Image>();
            currentObject.spellName = currentChild.GetChild(1).GetComponentsInChildren<TextMeshProUGUI>()[0];
            currentObject.spellCost = currentChild.GetChild(1).GetComponentsInChildren<TextMeshProUGUI>()[1];

            spellObjects[i] = currentObject;
        }

        HideSpellList();
    }

    private void HideSpellList()
    {
        foreach (var item in spellObjects)
            item.spellGameObject.SetActive(false);

        currentDisplayedSpellIndex = 0;
    }

    private void SetSpellListItem(SpellScriptableObject spell, bool isPositive)
    {
        var currentSpellObject = spellObjects[currentDisplayedSpellIndex];

        currentSpellObject.spellGameObject.SetActive(true);
        currentSpellObject.spellBackground.color = isPositive ? positiveColour : negativeColour;
        currentSpellObject.inclusionSymbol.color = isPositive ? positiveColour : negativeColour;
        currentSpellObject.inclusionSymbol.text = isPositive ? "+" : "-";
        currentSpellObject.spellName.text = spell.spellName;
        currentSpellObject.spellCost.text = spell.spellCost.ToString();
        currentSpellObject.spellIcon.sprite = SpellIcons.icons[spell.spellType];

        currentDisplayedSpellIndex++;
    }

    private void SetEquipmentSpells(PartyController.ProtagonistEquipment allEquipment, EquipmentScriptableObject.EquipmentType equipmentType, SpellScriptableObject[] currentEquipmentSpells, SpellScriptableObject[] incomingEquipmentSpells)
    {
        void AddSpellsToSet(ref HashSet<SpellScriptableObject> hashSet, EquipmentScriptableObject.EquipmentType type, SpellScriptableObject[] spells)
        {
            if (type == equipmentType) return;

            foreach (var spell in spells)
                hashSet.Add(spell);
        }

        HideSpellList();

        // Create set
        HashSet<SpellScriptableObject> knownSpells = new();

        // Search all equipment. Add spells to set.
        if(allEquipment.weapon != null)
            AddSpellsToSet(ref knownSpells, EquipmentScriptableObject.EquipmentType.Weapon, allEquipment.weapon.equipmentSpells);

        if (allEquipment.defense != null)
            AddSpellsToSet(ref knownSpells, EquipmentScriptableObject.EquipmentType.Armour, allEquipment.defense.equipmentSpells);

        if (allEquipment.trinkets != null && allEquipment.trinkets[0] != null)
            AddSpellsToSet(ref knownSpells, EquipmentScriptableObject.EquipmentType.Trinket, allEquipment.trinkets[0].equipmentSpells);

        //TODO: make second trinket follow logic too.

        // Positive spells first. If incoming spells aren't in set or current spells, display them here.
        foreach(var incomingSpell in incomingEquipmentSpells)
        {
            if (knownSpells.Contains(incomingSpell) || currentEquipmentSpells.Contains(incomingSpell)) continue;

            SetSpellListItem(incomingSpell, true);
        }

        // Negative spells next. If current spells arn't in set or incoming spells, display them here.
        foreach(var outgoingSpell in currentEquipmentSpells)
        {
            if (knownSpells.Contains(outgoingSpell) || incomingEquipmentSpells.Contains(outgoingSpell)) continue;

            SetSpellListItem(outgoingSpell, false);
        }
    }

    private void SetAffinities(PartyController.ProtagonistEquipment allEquipment, EquipmentScriptableObject.EquipmentType equipmentType, CombatantScriptableObject.AttributeAffinityDictionaryItem[] currentEquipmentAffinties, CombatantScriptableObject.AttributeAffinityDictionaryItem[] incomingEquipmentAffinties)
    {
        void TallyAffinities(ref Dictionary<SpellScriptableObject.SpellType, CombatantScriptableObject.AttributeAffinity> runningTally, EquipmentScriptableObject.EquipmentType type, CombatantScriptableObject.AttributeAffinityDictionaryItem[] equipmentAffinities)
        {
            if (type == equipmentType) return;

            for(int i = 0; i < equipmentAffinities.Length; i++)
            {
                if(runningTally[(SpellScriptableObject.SpellType)i] < equipmentAffinities[i].value)
                    runningTally[(SpellScriptableObject.SpellType)i] = equipmentAffinities[i].value;
            }
        }

        // Set known affinities
        Dictionary<SpellScriptableObject.SpellType, CombatantScriptableObject.AttributeAffinity> knownAffinities = new Dictionary<SpellScriptableObject.SpellType, CombatantScriptableObject.AttributeAffinity>();

        // Iterate through all equipment disregarding the compared equipment
        if(allEquipment.weapon != null)
            TallyAffinities(ref knownAffinities, EquipmentScriptableObject.EquipmentType.Weapon, allEquipment.weapon.equipmentAffinties);

        if (allEquipment.defense != null)
            TallyAffinities(ref knownAffinities, EquipmentScriptableObject.EquipmentType.Armour, allEquipment.defense.equipmentAffinties);

        if (allEquipment.trinkets != null && allEquipment.trinkets[0] != null)
            TallyAffinities(ref knownAffinities, EquipmentScriptableObject.EquipmentType.Trinket, allEquipment.trinkets[0].equipmentAffinties);

        //TODO: make second trinket follow logic too.

        for(int i = 0; i < currentEquipmentAffinties.Length; i++)
        {
            // Neutral
            if(knownAffinities[(SpellScriptableObject.SpellType)i] >= incomingEquipmentAffinties[i].value || knownAffinities[(SpellScriptableObject.SpellType)i] >= currentEquipmentAffinties[i].value)
            {
                affinityImages[i].sprite = affinitySprites.affinitySpriteDictionary[knownAffinities[(SpellScriptableObject.SpellType)i]];
                affinityImages[i].color = Color.white;
            }
            // Positive affinity
            else if(incomingEquipmentAffinties[i].value > currentEquipmentAffinties[i].value)
            {
                affinityImages[i].sprite = affinitySprites.affinitySpriteDictionary[incomingEquipmentAffinties[i].value];
                affinityImages[i].color = positiveColour;
            }
            // Negative affinity
            else if (currentEquipmentAffinties[i].value > incomingEquipmentAffinties[i].value)
            {
                affinityImages[i].sprite = affinitySprites.affinitySpriteDictionary[currentEquipmentAffinties[i].value];
                affinityImages[i].color = negativeColour;
            }
            // Neutral
            else
            {
                affinityImages[i].sprite = affinitySprites.affinitySpriteDictionary[currentEquipmentAffinties[i].value];
                affinityImages[i].color = Color.white;
            }
        }
    }
}
