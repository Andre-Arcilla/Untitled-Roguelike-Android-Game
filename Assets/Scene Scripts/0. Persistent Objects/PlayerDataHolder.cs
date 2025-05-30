using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerDataHolder : MonoBehaviour
{
    public static PlayerDataHolder Instance;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        LoadPartyFromJson();
    }

    [System.Serializable]
    public class TownClearEntry
    {
        public string townName;
        public bool hasCleared;
    }

    [System.Serializable]
    private class PartyDataWrapper
    {
        public List<CharacterData> members = new List<CharacterData>();
        public List<string> inventory = new List<string>();
        public int gold;
        public List<TownClearEntry> clearedTowns = new();
    }

    [SerializeField] public List<CharacterData> partyMembers = new List<CharacterData>();
    [SerializeField] public List<string> partyInventory = new List<string>();
    [SerializeField] public int partyGold;
    [SerializeField] public List<TownClearEntry> partyClearedTowns = new List<TownClearEntry>();

    private void LoadPartyFromJson()
    {
        string jsonFilePath = Path.Combine(Application.persistentDataPath, "PartyData.json");

        if (!File.Exists(jsonFilePath))
        {
            Debug.LogError("Party JSON file not found at: " + jsonFilePath);
            return;
        }

        string json = File.ReadAllText(jsonFilePath);

        PartyDataWrapper wrapper = JsonUtility.FromJson<PartyDataWrapper>(json);

        partyMembers = wrapper.members;
        partyInventory = wrapper.inventory ?? new List<string>();
        partyGold = wrapper.gold;
        partyClearedTowns = wrapper.clearedTowns;
    }

    public void SavePartyInfo()
    {
        PartyDataWrapper wrapper = new PartyDataWrapper
        {
            members = partyMembers,
            inventory = partyInventory,
            gold = partyGold,
            clearedTowns = partyClearedTowns
        };

        string saveFile = JsonUtility.ToJson(wrapper, true);
        string path = Path.Combine(Application.persistentDataPath, "PartyData.json");

        File.WriteAllText(path, saveFile);
    }
}
