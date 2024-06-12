using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    const string FLOOR_KEY = "Floor";
    const string EQUIPMENT_KEY = "Equipment";
    const string PROTAG_SP_KEY = "PlayerSP";
    const string PARTY_KEY = "Party";
    const string DISCOVERED_AFFINITIES_KEY = "DiscoveredAffinities";

    public ScriptableObjectDatabase monsterDatabase;
    public ScriptableObjectDatabase equipmentDatabase;

    [Serializable]
    struct PlayerEquipmentObject
    {
        public string ID;
        public int[] bonuses;
        public bool upgraded;
    }

    public void SaveProgress()
    {
        PlayerPrefs.SetInt(FLOOR_KEY, GameController.CurrentLevel);
        PlayerPrefs.SetInt(PROTAG_SP_KEY, PartyController.partyMembers[0].Value.currentSP);

        SetEquipmentPlayerPrefs();
        SetPartyMonsterPlayerPrefs();
        SetSeenAffinitiesPlayerPrefs();
    }

    private void SetEquipmentPlayerPrefs()
    {
        PlayerEquipmentObject[] equipmentObjects = new PlayerEquipmentObject[4];
        equipmentObjects[0] = GetEquipmentObject(PartyController.protagonistEquipment.weapon);
        equipmentObjects[1] = GetEquipmentObject(PartyController.protagonistEquipment.defense);
        equipmentObjects[2] = GetEquipmentObject(PartyController.protagonistEquipment.trinkets[0]);
        equipmentObjects[3] = GetEquipmentObject(PartyController.protagonistEquipment.trinkets[1]);
        PlayerPrefs.SetString(EQUIPMENT_KEY, JsonUtility.ToJson(equipmentObjects));
    }

    private void SetPartyMonsterPlayerPrefs()
    {
        Dictionary<string, int> monsterObjects = new Dictionary<string, int>();
        for (int i = 1; i < PartyController.partyMembers.Length; i++)
            if (PartyController.partyMembers[i].HasValue)
                monsterObjects.Add(PartyController.partyMembers[i].Value.partyMemberBaseStats.Id, PartyController.partyMembers[i].Value.currentSP);
        PlayerPrefs.SetString(PARTY_KEY, JsonUtility.ToJson(monsterObjects));
    }

    private void SetSeenAffinitiesPlayerPrefs()
    {
        Dictionary<string, int> convertedSeenAffinities = new Dictionary<string, int>();
        var seenAffinities = SeenMonsterAffinities.GetAllSeenAffinities();
        foreach (KeyValuePair<CombatantScriptableObject, bool[]> item in seenAffinities)
        {
            BitArray bitArray = new(item.Value);
            int[] storedValue = new int[1];
            bitArray.CopyTo(storedValue, 0);
            convertedSeenAffinities.Add(item.Key.Id, storedValue[0]);
        }
        PlayerPrefs.SetString(DISCOVERED_AFFINITIES_KEY, JsonUtility.ToJson(convertedSeenAffinities));
    }

    private PlayerEquipmentObject GetEquipmentObject(EquipmentScriptableObject scriptableObject)
    {
        PlayerEquipmentObject current = new PlayerEquipmentObject();

        if (scriptableObject == null) return current;

        EquipmentScriptableObject baseEquipment = (EquipmentScriptableObject)equipmentDatabase.database[scriptableObject.Id];

        current.ID = scriptableObject.Id;
        current.bonuses = new int[5];
        current.bonuses[0] = scriptableObject.equipmentStats.strength - baseEquipment.equipmentStats.strength;
        current.bonuses[1] = scriptableObject.equipmentStats.magic - baseEquipment.equipmentStats.magic;
        current.bonuses[2] = scriptableObject.equipmentStats.endurance - baseEquipment.equipmentStats.endurance;
        current.bonuses[3] = scriptableObject.equipmentStats.agility - baseEquipment.equipmentStats.agility;
        current.bonuses[4] = scriptableObject.equipmentStats.luck - baseEquipment.equipmentStats.luck;

        current.upgraded = scriptableObject.equipmentName.EndsWith("+");

        return current;
    }
}
