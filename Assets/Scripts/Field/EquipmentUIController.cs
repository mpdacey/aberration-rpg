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

    private void OnEnable()
    {
        TreasureController.TreasureEquipmentGenerated += CompareEquipment;
    }

    private void OnDisable()
    {
        TreasureController.TreasureEquipmentGenerated -= CompareEquipment;
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
        void TallyAffinities(ref Dictionary<SpellScriptableObject.SpellType, CombatantScriptableObject.AttributeAffinity> runningTally, CombatantScriptableObject.AttributeAffinityDictionaryItem[] equipmentAffinities)
        {
            for(int i = 0; i < equipmentAffinities.Length; i++)
            {
                if(!runningTally.ContainsKey((SpellScriptableObject.SpellType)i) || runningTally[(SpellScriptableObject.SpellType)i] < equipmentAffinities[i].value)
                    runningTally[(SpellScriptableObject.SpellType)i] = equipmentAffinities[i].value;
            }
        }

        // Set known affinities
        Dictionary<SpellScriptableObject.SpellType, CombatantScriptableObject.AttributeAffinity> knownAffinities = new Dictionary<SpellScriptableObject.SpellType, CombatantScriptableObject.AttributeAffinity>();
        Dictionary<SpellScriptableObject.SpellType, CombatantScriptableObject.AttributeAffinity> currentAffinities = new Dictionary<SpellScriptableObject.SpellType, CombatantScriptableObject.AttributeAffinity>();
        Dictionary<SpellScriptableObject.SpellType, CombatantScriptableObject.AttributeAffinity> incomingAffinities = new Dictionary<SpellScriptableObject.SpellType, CombatantScriptableObject.AttributeAffinity>();

        // Iterate through all equipment disregarding the compared equipment
        if (allEquipment.weapon != null && equipmentType != EquipmentScriptableObject.EquipmentType.Weapon)
            TallyAffinities(ref knownAffinities, allEquipment.weapon.equipmentAffinties);

        if (allEquipment.defense != null && equipmentType != EquipmentScriptableObject.EquipmentType.Armour)
            TallyAffinities(ref knownAffinities, allEquipment.defense.equipmentAffinties);

        if (allEquipment.trinkets != null && equipmentType != EquipmentScriptableObject.EquipmentType.Trinket && allEquipment.trinkets[0] != null)
            TallyAffinities(ref knownAffinities, allEquipment.trinkets[0].equipmentAffinties);

        TallyAffinities(ref currentAffinities, currentEquipmentAffinties);
        TallyAffinities(ref incomingAffinities, incomingEquipmentAffinties);

        CombatantScriptableObject.AttributeAffinity[] equipmentAffinties = new CombatantScriptableObject.AttributeAffinity[6] { 0,0,0,0,0,0 };

        //TODO: make second trinket follow logic too.

        for (int i = 0; i < equipmentAffinties.Length; i++)
        {
            if (knownAffinities.ContainsKey((SpellScriptableObject.SpellType)i))
                equipmentAffinties[i] = knownAffinities[(SpellScriptableObject.SpellType)i];

            int knownAffinityValue = knownAffinities.ContainsKey((SpellScriptableObject.SpellType)i) ? (int)knownAffinities[(SpellScriptableObject.SpellType)i] : 0;
            int currentAffinityValue = currentAffinities.ContainsKey((SpellScriptableObject.SpellType)i) ? (int)currentAffinities[(SpellScriptableObject.SpellType)i] : -1;
            int incomingAffinityValue = incomingAffinities.ContainsKey((SpellScriptableObject.SpellType)i) ? (int)incomingAffinities[(SpellScriptableObject.SpellType)i] : -1;

            equipmentAffinties[i] = (CombatantScriptableObject.AttributeAffinity)Mathf.Max(knownAffinityValue, incomingAffinityValue);

            int offsetIndex = (i + 2) % equipmentAffinties.Length;

            if (incomingAffinityValue > knownAffinityValue && incomingAffinityValue > currentAffinityValue && incomingAffinityValue != (int)CombatantScriptableObject.AttributeAffinity.Weak) affinityImages[offsetIndex].color = positiveColour;
            else if (incomingAffinityValue < knownAffinityValue && incomingAffinityValue < currentAffinityValue || incomingAffinityValue > knownAffinityValue && incomingAffinityValue == (int)CombatantScriptableObject.AttributeAffinity.Weak) affinityImages[offsetIndex].color = negativeColour;
            else affinityImages[offsetIndex].color = Color.white;

            affinityImages[offsetIndex].sprite = affinitySprites.affinitySpriteDictionary[equipmentAffinties[i]];
        }
    }
}
