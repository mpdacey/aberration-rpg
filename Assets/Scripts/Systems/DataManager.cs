using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Defective.JSON;
using SpellType = SpellScriptableObject.SpellType;

public class DataManager : MonoBehaviour
{
    public static event Action<EquipmentState> LoadEquipment;

    const string FLOOR_KEY = "Cryptemental_Floor";
    const string EQUIPMENT_KEY = "Cryptemental_Equipment";
    const string PROTAG_SP_KEY = "Cryptemental_PlayerSP";
    const string PARTY_KEY = "Cryptemental_Party";
    const string DISCOVERED_AFFINITIES_KEY = "Cryptemental_DiscoveredAffinities";

    public ScriptableObjectDatabase monsterDatabase;
    public ScriptableObjectDatabase equipmentDatabase;

    public struct EquipmentState
    {
        public EquipmentScriptableObject weapon;
        public EquipmentScriptableObject armour;
        public EquipmentScriptableObject[] trinkets;
    }

    #region Saving Progress
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

        equipmentJSON.AddField("Weapon", GetEquipmentJSONObject(PartyController.protagonistEquipment.weapon));
        equipmentJSON.AddField("Armour", GetEquipmentJSONObject(PartyController.protagonistEquipment.defense));

        JSONObject trinketsJSON = new()
        {
            GetEquipmentJSONObject(PartyController.protagonistEquipment.trinkets[0]),
            GetEquipmentJSONObject(PartyController.protagonistEquipment.trinkets[1])
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

    private JSONObject GetEquipmentJSONObject(EquipmentScriptableObject scriptableObject)
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
    #endregion

    #region Load Progress
    public void LoadProgress()
    {
        LoadEquipmentPlayerPrefs();
        LoadPartyMonsterPlayerPrefs();
        LoadSeenAffinitiesPlayerPrefs();
    }

    private void LoadEquipmentPlayerPrefs()
    {
        if (LoadEquipment == null) return;

        if (!PlayerPrefs.HasKey(EQUIPMENT_KEY))
        {
            Debug.LogError("Equipment not saved in Player Prefs");
            return;
        }

        JSONObject equipmentJSONObject = JSONObject.Create(PlayerPrefs.GetString(EQUIPMENT_KEY));

        EquipmentState equipmentPacket = new EquipmentState();

        equipmentPacket.weapon = GetEquipmentScriptableObject(equipmentJSONObject.GetField("Weapon"));
        equipmentPacket.armour = GetEquipmentScriptableObject(equipmentJSONObject.GetField("Armour"));
        equipmentPacket.trinkets = new EquipmentScriptableObject[2];
        equipmentPacket.trinkets[0] = GetEquipmentScriptableObject(equipmentJSONObject.GetField("Trinkets").list[0]);
        equipmentPacket.trinkets[1] = GetEquipmentScriptableObject(equipmentJSONObject.GetField("Trinkets").list[1]);

        LoadEquipment.Invoke(equipmentPacket);
    }

    private EquipmentScriptableObject GetEquipmentScriptableObject(JSONObject json)
    {
        if (!json || json.ToString() == "null") return null;

        EquipmentScriptableObject equipment = Instantiate((EquipmentScriptableObject)equipmentDatabase.database[json.GetField("ID").stringValue]);

        if (json.GetField("Upgraded").boolValue)
            equipment.equipmentName += "+";

        var bonuses = json.GetField("Bonuses").list;
        equipment.equipmentStats.strength += bonuses[0].intValue;
        equipment.equipmentStats.magic += bonuses[1].intValue;
        equipment.equipmentStats.endurance += bonuses[2].intValue;
        equipment.equipmentStats.agility += bonuses[3].intValue;
        equipment.equipmentStats.luck += bonuses[4].intValue;

        return equipment;
    }

    private void LoadPartyMonsterPlayerPrefs()
    {
        if (!PlayerPrefs.HasKey(PARTY_KEY))
        {
            Debug.LogError("Party not saved in Player Prefs");
            return;
        }

        List<JSONObject> partyMonstersJSONObject = JSONObject.Create(PlayerPrefs.GetString(PARTY_KEY)).list;

        for(int i = 1; i < PartyController.partyMembers.Length; i++)
        {
            if (partyMonstersJSONObject[i - 1] == null || partyMonstersJSONObject[i - 1].ToString() == "null")
            {
                PartyController.SetPartyMember(null, i);
                continue;
            }
            PartyController.PartyMember monster = new();
            monster.partyMemberBaseStats = (CombatantScriptableObject)monsterDatabase.database[partyMonstersJSONObject[i - 1].GetField("ID").stringValue];
            monster.currentHP = monster.partyMemberBaseStats.combatantMaxHealth;
            monster.currentSP = partyMonstersJSONObject[i - 1].GetField("SP").intValue;
            PartyController.SetPartyMember(monster, i);
        }
    }

    private void LoadSeenAffinitiesPlayerPrefs()
    {
        if (!PlayerPrefs.HasKey(DISCOVERED_AFFINITIES_KEY))
        {
            Debug.LogError("Affinities were not saved in Player Prefs");
            return;
        }

        JSONObject seenAffinitiesJSON = JSONObject.Create(PlayerPrefs.GetString(DISCOVERED_AFFINITIES_KEY));

        if(seenAffinitiesJSON == null || seenAffinitiesJSON.ToString() == "null")
        {
            Debug.LogWarning("seenAffinitiesJSON is null");
            return;
        }

        SeenMonsterAffinities.ClearSeenAffinity();

        JSONObject[] monsterAffinitiesList = seenAffinitiesJSON.list.ToArray();

        for(int i = 0; i < monsterAffinitiesList.Length; i++)
        {
            var monster = (CombatantScriptableObject)monsterDatabase.database[monsterAffinitiesList[i].GetField("ID").stringValue];
            var seenToggles = new BitArray(new int[] { monsterAffinitiesList[i].GetField("Affinities").intValue });
            for(int j = 0; j < seenToggles.Length; j++)
                if (seenToggles.Get(j))
                    SeenMonsterAffinities.UpdateAffinityWitness(monster, (SpellType)j);
        }
    }

    #endregion
}
