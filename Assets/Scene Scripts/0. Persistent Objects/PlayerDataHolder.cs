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
    private class PartyDataWrapper
    {
        public List<CharacterData> members = new List<CharacterData>();
    }

    public List<CharacterData> partyMembers = new List<CharacterData>();

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

        Debug.Log("Party loaded with " + partyMembers.Count + " members.");
    }

    public void SavePartyInfo()
    {
        PartyDataWrapper wrapper = new PartyDataWrapper();
        wrapper.members = partyMembers;

        string saveFile = JsonUtility.ToJson(wrapper, true);
        string path = Path.Combine(Application.persistentDataPath, "PartyData.json");

        File.WriteAllText(path, saveFile);

        Debug.Log("Party saved to: " + path);
    }
}
