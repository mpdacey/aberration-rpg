using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Defective.JSON;

public class DataManager : MonoBehaviour
{
    const string FLOOR_KEY = "Cryptemental_Floor";
    const string EQUIPMENT_KEY = "Cryptemental_Equipment";
    const string PROTAG_SP_KEY = "Cryptemental_PlayerSP";
    const string PARTY_KEY = "Cryptemental_Party";
    const string DISCOVERED_AFFINITIES_KEY = "Cryptemental_DiscoveredAffinities";

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
        JSONObject equipmentJSON = new();

        equipmentJSON.AddField("Weapon", GetEquipmentObject(PartyController.protagonistEquipment.weapon));
        equipmentJSON.AddField("Armour", GetEquipmentObject(PartyController.protagonistEquipment.defense));

        JSONObject trinketsJSON = new()
        {
            GetEquipmentObject(PartyController.protagonistEquipment.trinkets[0]),
            GetEquipmentObject(PartyController.protagonistEquipment.trinkets[1])
        };
        equipmentJSON.AddField("Trinkets", trinketsJSON);

        PlayerPrefs.SetString(EQUIPMENT_KEY, equipmentJSON.ToString());

        Debug.Log(PlayerPrefs.GetString(EQUIPMENT_KEY));
    }

    private void SetPartyMonsterPlayerPrefs()
    {
        JSONObject partyMonstersObject = new();
        for (int i = 1; i < PartyController.partyMembers.Length; i++)
        {
            JSONObject currentMonsterObject = new();
            if (PartyController.partyMembers[i].HasValue)
            {
                currentMonsterObject.AddField("ID", PartyController.partyMembers[i].Value.partyMemberBaseStats.Id);
                currentMonsterObject.AddField("SP", PartyController.partyMembers[i].Value.currentSP);
            }

            partyMonstersObject.Add(currentMonsterObject);
        }

        PlayerPrefs.SetString(PARTY_KEY, partyMonstersObject.ToString());

        Debug.Log(PlayerPrefs.GetString(PARTY_KEY));
    }

    private void SetSeenAffinitiesPlayerPrefs()
    {
        JSONObject seenAffinitiesObject = new();

        var seenAffinities = SeenMonsterAffinities.GetAllSeenAffinities();
        foreach (KeyValuePair<CombatantScriptableObject, bool[]> item in seenAffinities)
        {
            BitArray bitArray = new(item.Value);
            int[] storedValue = new int[1];
            bitArray.CopyTo(storedValue, 0);

            JSONObject currentMonsterAffinities = new();
            currentMonsterAffinities.AddField("ID", item.Key.Id);
            currentMonsterAffinities.AddField("Affinities", storedValue[0]);
            seenAffinitiesObject.Add(currentMonsterAffinities);
        }

        PlayerPrefs.SetString(DISCOVERED_AFFINITIES_KEY, seenAffinitiesObject.ToString());

        Debug.Log(PlayerPrefs.GetString(DISCOVERED_AFFINITIES_KEY));
    }

    private JSONObject GetEquipmentObject(EquipmentScriptableObject scriptableObject)
    {
        JSONObject currentJSON = new();

        if (scriptableObject == null) return currentJSON;

        currentJSON.AddField("ID", scriptableObject.Id);

        EquipmentScriptableObject baseEquipment = (EquipmentScriptableObject)equipmentDatabase.database[scriptableObject.Id];
        JSONObject bonusesObject = new() 
        {
            scriptableObject.equipmentStats.strength - baseEquipment.equipmentStats.strength,
            scriptableObject.equipmentStats.magic - baseEquipment.equipmentStats.magic,
            scriptableObject.equipmentStats.endurance - baseEquipment.equipmentStats.endurance,
            scriptableObject.equipmentStats.agility - baseEquipment.equipmentStats.agility,
            scriptableObject.equipmentStats.luck - baseEquipment.equipmentStats.luck
        };
        currentJSON.AddField("Bonuses", bonusesObject);

        currentJSON.AddField("Upgraded", scriptableObject.equipmentName.EndsWith("+"));

        return currentJSON;
    }
}
