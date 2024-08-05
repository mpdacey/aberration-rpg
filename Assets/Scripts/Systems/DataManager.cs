using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Defective.JSON;
using SpellType = SpellScriptableObject.SpellType;
using Cryptemental.Data;

public class DataManager : MonoBehaviour
{
    public event Action<int> SetFloorLevel;

    public ScriptableObjectDatabase monsterDatabase;
    public ScriptableObjectDatabase equipmentDatabase;

    #region Saving Progress
    public void SaveProgress()
    {
        PlayerPrefs.SetInt(PlayerPrefsKeys.FLOOR_KEY, GameController.CurrentLevel);
        PlayerPrefs.SetInt(PlayerPrefsKeys.PROTAG_SP_KEY, PartyController.partyMembers[0].Value.currentSP);
        PlayerPrefs.SetString(PlayerPrefsKeys.EQUIPMENT_KEY, GetEquipmentJSONString());
        PlayerPrefs.SetString(PlayerPrefsKeys.PARTY_KEY, GetPartyMonsterJSONString());
        PlayerPrefs.SetString(PlayerPrefsKeys.DISCOVERED_AFFINITIES_KEY, GetSeenAffinitiesJSONString());
    }

    private string GetEquipmentJSONString()
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

        return equipmentJSON.ToString();
    }

    private string GetPartyMonsterJSONString()
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

        return partyMonstersObject.ToString();
    }

    private string GetSeenAffinitiesJSONString()
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

        return seenAffinitiesObject.ToString();
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
        if (!PlayerPrefs.HasKey(PlayerPrefsKeys.FLOOR_KEY))
        {
            Debug.LogError("Attempted to load player prefs without saving first");
            return;
        }

        if (SetFloorLevel != null) SetFloorLevel.Invoke(PlayerPrefs.GetInt(PlayerPrefsKeys.FLOOR_KEY));
        LoadProtagonistPlayerSP();
        LoadEquipmentPlayerPrefs();
        LoadPartyMonsterPlayerPrefs();
        LoadSeenAffinitiesPlayerPrefs();
    }

    private void LoadProtagonistPlayerSP()
    {
        if (!PartyController.partyMembers[0].HasValue)
        {
            Debug.LogError("Party controller does not have protagonist party member.");
            return;
        }

        var temp = PartyController.partyMembers[0].Value;
        temp.currentSP = PlayerPrefs.GetInt(PlayerPrefsKeys.PROTAG_SP_KEY);
        PartyController.partyMembers[0] = temp;
    }

    private void LoadEquipmentPlayerPrefs()
    {
        if (!PlayerPrefs.HasKey(PlayerPrefsKeys.EQUIPMENT_KEY))
        {
            Debug.LogError("Equipment not saved in Player Prefs");
            return;
        }

        JSONObject equipmentJSONObject = JSONObject.Create(PlayerPrefs.GetString(PlayerPrefsKeys.EQUIPMENT_KEY));

        PartyController.protagonistEquipment.weapon = GetEquipmentScriptableObject(equipmentJSONObject.GetField("Weapon"));
        PartyController.protagonistEquipment.defense = GetEquipmentScriptableObject(equipmentJSONObject.GetField("Armour"));
        PartyController.protagonistEquipment.trinkets[0] = GetEquipmentScriptableObject(equipmentJSONObject.GetField("Trinkets").list[0]);
        PartyController.protagonistEquipment.trinkets[1] = GetEquipmentScriptableObject(equipmentJSONObject.GetField("Trinkets").list[1]);
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
        if (!PlayerPrefs.HasKey(PlayerPrefsKeys.PARTY_KEY))
        {
            Debug.LogError("Party not saved in Player Prefs");
            return;
        }

        List<JSONObject> partyMonstersJSONObject = JSONObject.Create(PlayerPrefs.GetString(PlayerPrefsKeys.PARTY_KEY)).list;

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
        if (!PlayerPrefs.HasKey(PlayerPrefsKeys.DISCOVERED_AFFINITIES_KEY))
        {
            Debug.LogError("Affinities were not saved in Player Prefs");
            return;
        }

        JSONObject seenAffinitiesJSON = JSONObject.Create(PlayerPrefs.GetString(PlayerPrefsKeys.DISCOVERED_AFFINITIES_KEY));

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

    public void ClearProgress()
    {
        PlayerPrefs.SetInt(PlayerPrefsKeys.FLOOR_KEY, 0);
        PlayerPrefs.SetInt(PlayerPrefsKeys.PROTAG_SP_KEY, 0);
        PlayerPrefs.SetString(PlayerPrefsKeys.EQUIPMENT_KEY, "{}");
        PlayerPrefs.SetString(PlayerPrefsKeys.PARTY_KEY, "{}");
    }

    public void SaveMusicVolume(float normalisedVolume) =>
        PlayerPrefs.SetFloat(PlayerPrefsKeys.VOLUME_MUSIC_KEY, Mathf.Clamp01(normalisedVolume));

    public void SaveSoundVolume(float normalisedVolume) =>
        PlayerPrefs.SetFloat(PlayerPrefsKeys.VOLUME_SOUND_KEY, Mathf.Clamp01(normalisedVolume));

    public Vector2 LoadVolumes(Vector2 defaultVolumes)
    {
        Vector2 volumes = defaultVolumes;

        if (PlayerPrefs.HasKey(PlayerPrefsKeys.VOLUME_MUSIC_KEY))
            volumes.x = PlayerPrefs.GetFloat(PlayerPrefsKeys.VOLUME_MUSIC_KEY);
        else
            Debug.LogWarning("Player prefs does not have music volume");

        if (PlayerPrefs.HasKey(PlayerPrefsKeys.VOLUME_SOUND_KEY))
            volumes.y = PlayerPrefs.GetFloat(PlayerPrefsKeys.VOLUME_SOUND_KEY);
        else
            Debug.LogWarning("Player prefs does not have sound volume");

        return volumes;
    }
}
