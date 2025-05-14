using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

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
    public class EquipmentSaveData
    {
        public string equipmentName;
    }

    [System.Serializable]
    private class PartyDataWrapper
    {
        public List<CharacterData> members = new List<CharacterData>();
        public List<EquipmentSaveData> inventory = new List<EquipmentSaveData>();
    }

    public List<CharacterData> partyMembers = new List<CharacterData>();
    public List<EquipmentSaveData> inventoryItems = new List<EquipmentSaveData>();


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
        inventoryItems = wrapper.inventory ?? new List<EquipmentSaveData>();

        Debug.Log("Party loaded with " + partyMembers.Count + " members and " + inventoryItems.Count + " inventory items.");
    }

    public void SavePartyInfo()
    {
        PartyDataWrapper wrapper = new PartyDataWrapper
        {
            members = partyMembers,
            inventory = inventoryItems
        };

        string saveFile = JsonUtility.ToJson(wrapper, true);
        string path = Path.Combine(Application.persistentDataPath, "PartyData.json");

        File.WriteAllText(path, saveFile);

        Debug.Log("Party and inventory saved to: " + path);
    }

}
