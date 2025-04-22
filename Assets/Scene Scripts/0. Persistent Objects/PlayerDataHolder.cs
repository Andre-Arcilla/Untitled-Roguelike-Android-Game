using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.U2D.Animation;
using UnityEngine;

public class PlayerDataHolder : MonoBehaviour
{
    public static PlayerDataHolder Instance;

    public CharacterData playerData;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        LoadCharacterFromJson();
    }

    private void LoadCharacterFromJson()
    {
        string jsonFilePath = Path.Combine(Application.persistentDataPath, "Character1Data.json");

        if (!File.Exists(jsonFilePath))
        {
            Debug.LogError("JSON file not found at: " + jsonFilePath);
            return;
        }

        string json = File.ReadAllText(jsonFilePath);
        playerData = JsonUtility.FromJson<CharacterData>(json);

        Debug.Log("Data loaded from JSON: " + playerData.basicInfo.playerName);
    }

    public void SavePlayerInfo()
    {
        string saveFile = JsonUtility.ToJson(playerData, true);
        string path = Path.Combine(Application.persistentDataPath, "Character1Data.json");
        File.WriteAllText(path, saveFile);

        Debug.Log("Player data saved to: " + path);
    }
}

