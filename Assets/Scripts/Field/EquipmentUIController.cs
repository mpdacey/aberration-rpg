using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Linq;
using AffinityItem = CombatantScriptableObject.AttributeAffinityDictionaryItem;
using AffinityType = CombatantScriptableObject.AttributeAffinity;
using EquipmentType = EquipmentScriptableObject.EquipmentType;
using SpellType = SpellScriptableObject.SpellType;
using UnityEngine.EventSystems;

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

    enum SpellUIState
    {
        Positive,
        Negative,
        Neutral
    }

    [SerializeField] EquipmentStatUISlider[] statSliders;
    [SerializeField] Transform spellUIContainer;
    [SerializeField] Image[] affinityImages;
    [SerializeField] TextMeshProUGUI titleText;
    [SerializeField] TextMeshProUGUI offerButton1Text;
    [SerializeField] Button offerButton2Button;
    [SerializeField] TextMeshProUGUI offerButton2Text;
    [SerializeField] Color positiveColour;
    [SerializeField] Color negativeColour;
    [SerializeField] AffinitySpritesScriptableObject affinitySprites;
    SpellUIObject[] spellObjects;
    int currentDisplayedSpellIndex = 0;
    int selectedTrinketIndex = 0;

    private void Start()
    {
        InitialiseSpellList();
    }

    public void InitialiseEquipmentUI(PartyController.ProtagonistEquipment allEquipment, EquipmentScriptableObject incomingEquipment)
    {
        titleText.text = $"Aquired\n{incomingEquipment.equipmentName}";

        switch (incomingEquipment.equipmentType)
        {
            case EquipmentType.Weapon:
                if (allEquipment.weapon != null) offerButton1Text.text = $"Replace {allEquipment.weapon.equipmentName}";
                else offerButton1Text.text = $"Equip {incomingEquipment.equipmentType}";
                offerButton2Button.gameObject.SetActive(false);
                break;
            case EquipmentType.Armour:
                if (allEquipment.defense != null) offerButton1Text.text = $"Replace {allEquipment.defense.equipmentName}";
                else offerButton1Text.text = $"Equip {incomingEquipment.equipmentType}";
                offerButton2Button.gameObject.SetActive(false);
                break;
            case EquipmentType.Trinket:
                if (allEquipment.trinkets == null)
                    allEquipment.trinkets = new EquipmentScriptableObject[2];

                if (allEquipment.trinkets[0] != null) offerButton1Text.text = $"Replace {allEquipment.trinkets[0].equipmentName}";
                else offerButton1Text.text = $"Equip {incomingEquipment.equipmentType}";

                offerButton2Button.gameObject.SetActive(true);
                if (allEquipment.trinkets[1] != null) offerButton2Text.text = $"Replace {allEquipment.trinkets[1].equipmentName}";
                else offerButton2Text.text = $"Equip {incomingEquipment.equipmentType}";
                break;
        }
    }

    public void CompareEquipment(PartyController.PartyMember protagonist, PartyController.ProtagonistEquipment allEquipment, EquipmentScriptableObject incomingEquipment, int trinketIndex = 0)
    {
        EquipmentScriptableObject currentEquipment = null;

        switch (incomingEquipment.equipmentType)
        {
            case EquipmentType.Weapon:
                currentEquipment = allEquipment.weapon;
                break;
            case EquipmentType.Armour:
                currentEquipment = allEquipment.defense;
                break;
            case EquipmentType.Trinket:
                // TODO: have logic for multiple trinkets.
                currentEquipment = allEquipment.trinkets[trinketIndex];
                break;
            default: 
                currentEquipment = allEquipment.weapon;
                break;
        }

        if(currentEquipment != null)
        {
            SetStatSliders(statSliders[0], protagonist.partyMemberBaseStats.combatantBaseStats.strength, currentEquipment.equipmentStats.strength, incomingEquipment.equipmentStats.strength);
            SetStatSliders(statSliders[1], protagonist.partyMemberBaseStats.combatantBaseStats.magic, currentEquipment.equipmentStats.magic, incomingEquipment.equipmentStats.magic);
            SetStatSliders(statSliders[2], protagonist.partyMemberBaseStats.combatantBaseStats.agility, currentEquipment.equipmentStats.agility, incomingEquipment.equipmentStats.agility);
            SetStatSliders(statSliders[3], protagonist.partyMemberBaseStats.combatantBaseStats.endurance, currentEquipment.equipmentStats.endurance, incomingEquipment.equipmentStats.endurance);
            SetStatSliders(statSliders[4], protagonist.partyMemberBaseStats.combatantBaseStats.luck, currentEquipment.equipmentStats.luck, incomingEquipment.equipmentStats.luck);

            SetEquipmentSpells(allEquipment, incomingEquipment.equipmentType, currentEquipment.equipmentSpells, incomingEquipment.equipmentSpells, trinketIndex);

            SetAffinities(allEquipment, incomingEquipment.equipmentType, currentEquipment.equipmentAffinties, incomingEquipment.equipmentAffinties, trinketIndex);
        }
        else
        {
            SetStatSliders(statSliders[0], protagonist.partyMemberBaseStats.combatantBaseStats.strength, 0, incomingEquipment.equipmentStats.strength);
            SetStatSliders(statSliders[1], protagonist.partyMemberBaseStats.combatantBaseStats.magic, 0, incomingEquipment.equipmentStats.magic);
            SetStatSliders(statSliders[2], protagonist.partyMemberBaseStats.combatantBaseStats.agility, 0, incomingEquipment.equipmentStats.agility);
            SetStatSliders(statSliders[3], protagonist.partyMemberBaseStats.combatantBaseStats.endurance, 0, incomingEquipment.equipmentStats.endurance);
            SetStatSliders(statSliders[4], protagonist.partyMemberBaseStats.combatantBaseStats.luck, 0, incomingEquipment.equipmentStats.luck);

            SetEquipmentSpells(allEquipment, incomingEquipment.equipmentType, new SpellScriptableObject[0], incomingEquipment.equipmentSpells, trinketIndex);

            SetAffinities(allEquipment, incomingEquipment.equipmentType, new AffinityItem[0], incomingEquipment.equipmentAffinties, trinketIndex);
        }

        transform.GetChild(0).gameObject.SetActive(true);
    }

    private void SetStatSliders(EquipmentStatUISlider stat, int cumulativeStat, int currentStat, int incomingStat)
    {
        stat.sliderValueText.text = (cumulativeStat + incomingStat - currentStat).ToString();

        // Worse stat
        if(currentStat > incomingStat)
        {
            stat.baseSlider.value = cumulativeStat + incomingStat - currentStat;
            stat.negativeSlider.value = cumulativeStat;
            stat.positiveSlider.value = 0;
            stat.sliderValueText.text += $"<size=12> </size><b><size=34><color=#{ColorUtility.ToHtmlStringRGB(negativeColour)}>-{currentStat-incomingStat}</color></size></b>";
        }
        // Better stat
        else if (currentStat < incomingStat)
        {
            stat.baseSlider.value = cumulativeStat;
            stat.negativeSlider.value = 0;
            stat.positiveSlider.value = cumulativeStat + incomingStat - currentStat;
            stat.sliderValueText.text += $"<size=12> </size><b><size=34><color=#{ColorUtility.ToHtmlStringRGB(positiveColour)}>+{incomingStat - currentStat}</color></size></b>";
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

    private void SetSpellListItem(SpellScriptableObject spell, SpellUIState spellState)
    {
        var currentSpellObject = spellObjects[currentDisplayedSpellIndex];

        switch (spellState)
        {
            case SpellUIState.Positive:
                currentSpellObject.spellBackground.color = positiveColour;
                currentSpellObject.inclusionSymbol.color = positiveColour;
                currentSpellObject.inclusionSymbol.text = "+";
                break;
            case SpellUIState.Negative:
                currentSpellObject.spellBackground.color = negativeColour;
                currentSpellObject.inclusionSymbol.color = negativeColour;
                currentSpellObject.inclusionSymbol.text = "-";
                break;
            case SpellUIState.Neutral:
                currentSpellObject.spellBackground.color = Color.grey;
                currentSpellObject.inclusionSymbol.color = Color.grey;
                currentSpellObject.inclusionSymbol.text = "";
                break;
        }
        
        currentSpellObject.spellName.text = spell.spellName;
        currentSpellObject.spellCost.text = spell.spellCost.ToString();
        currentSpellObject.spellIcon.sprite = SpellIcons.icons[spell.spellType];
        currentSpellObject.spellGameObject.SetActive(true);

        currentDisplayedSpellIndex++;
    }

    private void SetEquipmentSpells(PartyController.ProtagonistEquipment allEquipment, EquipmentType equipmentType, SpellScriptableObject[] currentEquipmentSpells, SpellScriptableObject[] incomingEquipmentSpells, int trinketIndex = 0)
    {
        void AddSpellsToSet(ref HashSet<SpellScriptableObject> hashSet, EquipmentType type, SpellScriptableObject[] spells)
        {
            foreach (var spell in spells)
                hashSet.Add(spell);
        }

        HideSpellList();

        // Create set
        HashSet<SpellScriptableObject> knownSpells = new();

        // Search all equipment. Add spells to set.
        if(allEquipment.weapon != null && equipmentType != EquipmentType.Weapon)
            AddSpellsToSet(ref knownSpells, EquipmentType.Weapon, allEquipment.weapon.equipmentSpells);

        if (allEquipment.defense != null && equipmentType != EquipmentType.Armour)
            AddSpellsToSet(ref knownSpells, EquipmentType.Armour, allEquipment.defense.equipmentSpells);

        if (allEquipment.trinkets != null)
        {
            if(allEquipment.trinkets[0] != null && (equipmentType != EquipmentType.Trinket || (equipmentType == EquipmentType.Trinket && trinketIndex != 0)))
                AddSpellsToSet(ref knownSpells, EquipmentType.Trinket, allEquipment.trinkets[0].equipmentSpells);
            if(allEquipment.trinkets[1] != null && (equipmentType != EquipmentType.Trinket || (equipmentType == EquipmentType.Trinket && trinketIndex != 1)))
                AddSpellsToSet(ref knownSpells, EquipmentType.Trinket, allEquipment.trinkets[1].equipmentSpells);
        }

        // Positive spells first. If incoming spells aren't in set or current spells, display them here.
        foreach(var incomingSpell in incomingEquipmentSpells)
        {
            if (knownSpells.Contains(incomingSpell) || currentEquipmentSpells.Contains(incomingSpell)) continue;

            SetSpellListItem(incomingSpell, SpellUIState.Positive);
        }

        // Neutral spells
        foreach (var incomingSpell in incomingEquipmentSpells)
        {
            if (knownSpells.Contains(incomingSpell) || currentEquipmentSpells.Contains(incomingSpell))
            {
                SetSpellListItem(incomingSpell, SpellUIState.Neutral);
            }
        }

        // Negative spells next. If current spells arn't in set or incoming spells, display them here.
        foreach (var outgoingSpell in currentEquipmentSpells)
        {
            if (knownSpells.Contains(outgoingSpell) || incomingEquipmentSpells.Contains(outgoingSpell)) continue;

            SetSpellListItem(outgoingSpell, SpellUIState.Negative);
        }
    }

    private void SetAffinities(PartyController.ProtagonistEquipment allEquipment, EquipmentType equipmentType, AffinityItem[] currentEquipmentAffinties, AffinityItem[] incomingEquipmentAffinties, int trinketIndex = 0)
    {
        void TallyAffinities(ref Dictionary<SpellType, AffinityType> runningTally, AffinityItem[] equipmentAffinities)
        {
            for(int i = 0; i < equipmentAffinities.Length; i++)
            {
                if(!runningTally.ContainsKey(equipmentAffinities[i].key) || runningTally[equipmentAffinities[i].key] < equipmentAffinities[i].value)
                {
                    runningTally[equipmentAffinities[i].key] = equipmentAffinities[i].value;
                }
            }
        }

        // Set known affinities
        Dictionary<SpellType, AffinityType> knownAffinities = new Dictionary<SpellType, AffinityType>();
        Dictionary<SpellType, AffinityType> currentAffinities = new Dictionary<SpellType, AffinityType>();
        Dictionary<SpellType, AffinityType> incomingAffinities = new Dictionary<SpellType, AffinityType>();

        // Iterate through all equipment disregarding the compared equipment
        if (allEquipment.weapon != null && equipmentType != EquipmentType.Weapon)
            TallyAffinities(ref knownAffinities, allEquipment.weapon.equipmentAffinties);

        if (allEquipment.defense != null && equipmentType != EquipmentType.Armour)
            TallyAffinities(ref knownAffinities, allEquipment.defense.equipmentAffinties);

        if (allEquipment.trinkets != null)
        {
            if (allEquipment.trinkets[0] != null && (equipmentType != EquipmentType.Trinket || (equipmentType == EquipmentType.Trinket && trinketIndex != 0)))
                TallyAffinities(ref knownAffinities, allEquipment.trinkets[0].equipmentAffinties);
            if (allEquipment.trinkets[1] != null && (equipmentType != EquipmentType.Trinket || (equipmentType == EquipmentType.Trinket && trinketIndex != 1)))
                TallyAffinities(ref knownAffinities, allEquipment.trinkets[1].equipmentAffinties);
        }

        TallyAffinities(ref currentAffinities, currentEquipmentAffinties);
        TallyAffinities(ref incomingAffinities, incomingEquipmentAffinties);

        AffinityType[] equipmentAffinties = new AffinityType[6] { 0,0,0,0,0,0 };

        //TODO: make second trinket follow logic too.

        for (int i = 0; i < equipmentAffinties.Length; i++)
        {
            if (knownAffinities.ContainsKey((SpellType)i))
                equipmentAffinties[i] = knownAffinities[(SpellType)i];

            int knownAffinityValue = knownAffinities.ContainsKey((SpellType)i) ? (int)knownAffinities[(SpellType)i] : 0;
            int currentAffinityValue = currentAffinities.ContainsKey((SpellType)i) ? (int)currentAffinities[(SpellType)i] : -1;
            int incomingAffinityValue = incomingAffinities.ContainsKey((SpellType)i) ? (int)incomingAffinities[(SpellType)i] : -1;

            equipmentAffinties[i] = (AffinityType)Mathf.Max(knownAffinityValue, incomingAffinityValue);

            int currentMax = Mathf.Max(knownAffinityValue, currentAffinityValue);
            int incomingMax = Mathf.Max(knownAffinityValue, incomingAffinityValue);

            if (incomingMax == currentMax) affinityImages[i].color = Color.white;
            else if ((incomingMax > currentMax && incomingMax != (int)AffinityType.Weak) || currentMax == (int)AffinityType.Weak) affinityImages[i].color = positiveColour;
            else affinityImages[i].color = negativeColour;

            affinityImages[i].sprite = affinitySprites.affinitySpriteDictionary[equipmentAffinties[i]];
        }
    }
}
