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
        public string currentTown;
        public List<string> inventory = new List<string>();
        public int gold;
        public List<TownClearEntry> clearedTowns = new();
    }

    [SerializeField] public List<CharacterData> partyMembers = new List<CharacterData>();
    [SerializeField] public string partyCurrentTown;
    [SerializeField] public List<string> partyInventory = new List<string>();
    [SerializeField] public int partyGold;
    [SerializeField] public List<TownClearEntry> partyClearedTowns = new List<TownClearEntry>();

    public void LoadPartyFromJson()
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
        partyCurrentTown = wrapper.currentTown;
        partyInventory = wrapper.inventory ?? new List<string>();
        partyGold = wrapper.gold;
        partyClearedTowns = wrapper.clearedTowns;
    }

    public void SavePartyInfo()
    {
        PartyDataWrapper wrapper = new PartyDataWrapper
        {
            members = partyMembers,
            currentTown = partyCurrentTown,
            inventory = partyInventory,
            gold = partyGold,
            clearedTowns = partyClearedTowns
        };

        string saveFile = JsonUtility.ToJson(wrapper, true);
        string path = Path.Combine(Application.persistentDataPath, "PartyData.json");

        File.WriteAllText(path, saveFile);
    }

    public void ClearData()
    {
        partyMembers = new List<CharacterData>();
        partyCurrentTown = "";
        partyInventory = new List<string>();
        partyGold = 0;
        partyClearedTowns = new List<TownClearEntry>();
    }
}
