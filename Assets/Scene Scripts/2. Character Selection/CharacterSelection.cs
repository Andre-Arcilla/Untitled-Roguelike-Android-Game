using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CharacterSelection : MonoBehaviour
{
    [SerializeField] private StatManager statManager;
    [SerializeField] private ClassManager classManager;

    [SerializeField] private PlayerSaveData playerData = new PlayerSaveData();

    [System.Serializable]
    public class PlayerSaveData
    {
        public PlayerInfo basicInfo;
        public PlayerStats allocatedStats;
        public List<string> classes = new List<string>();
        public PlayerEquipment equipment;

        [System.Serializable]
        public class PlayerInfo
        {
            public string playerName;
            public int level;
            public string gender;
            public string raceName;
        }

        [System.Serializable]
        public class PlayerStats
        {
            public int allocatedHP;
            public int allocatedEN;
            public int allocatedPWR;
            public int allocatedSPD;
            public int statPoints;
        }

        [System.Serializable]
        public class PlayerEquipment
        {
            public string armor;
            public string weapon;
            public string accessory1;
            public string accessory2;
            public string accessory3;
        }
    }

    [System.Serializable]
    private class PartyDataWrapper
    {
        public List<CharacterData> members = new List<CharacterData>();
        public List<string> inventory = new List<string>();
        public int gold = 0;
    }

    public void Create()
    {
        UpdatePlayerInfo();

        if (GameManager.Instance == null)
        {
            SceneManager.LoadScene("Persistent Data");
            SceneManager.LoadScene("Town Selection");
        }
        else
        {
            SceneManager.LoadScene("Town Selection");
        }
    }

    public void Cancel()
    {
        SceneManager.LoadScene("Main Menu");
    }

    public void UpdatePlayerInfo()
    {
        // Fill PlayerSaveData from UI
        playerData.basicInfo = new PlayerSaveData.PlayerInfo
        {
            playerName = "NewPlayer", // Replace this with actual input if needed
            level = 1,
            gender = classManager.selectedGender,
            raceName = statManager.selectedRace.raceName
        };

        playerData.allocatedStats = new PlayerSaveData.PlayerStats
        {
            allocatedHP = statManager.allocatedHP,
            allocatedEN = statManager.allocatedEN,
            allocatedPWR = statManager.allocatedPWR,
            allocatedSPD = statManager.allocatedSPD,
            statPoints = statManager.RemainingStatPoints
        };

        playerData.classes.Clear();
        playerData.classes.Add(classManager.selectedClass.className);

        playerData.equipment = new PlayerSaveData.PlayerEquipment
        {
            armor = null,
            weapon = null,
            accessory1 = null,
            accessory2 = null,
            accessory3 = null
        };

        // Convert to CharacterData
        CharacterData character = new CharacterData
        {
            isAlive = true,
            basicInfo = new CharacterData.BasicInfo
            {
                characterName = playerData.basicInfo.playerName,
                level = playerData.basicInfo.level,
                xp = 0, // Assuming new characters start at 0 XP
                gender = playerData.basicInfo.gender,
                raceName = playerData.basicInfo.raceName
            },
            allocatedStats = new CharacterData.AllocatedStats
            {
                allocatedHP = playerData.allocatedStats.allocatedHP,
                allocatedEN = playerData.allocatedStats.allocatedEN,
                allocatedPWR = playerData.allocatedStats.allocatedPWR,
                allocatedSPD = playerData.allocatedStats.allocatedSPD
            },
            classes = new List<string>(playerData.classes),
            equipment = new CharacterData.Equipment
            {
                armor = playerData.equipment.armor,
                weapon = playerData.equipment.weapon,
                accessory1 = playerData.equipment.accessory1,
                accessory2 = playerData.equipment.accessory2,
                accessory3 = playerData.equipment.accessory3
            }
        };

        // Save character inside a wrapper
        PartyDataWrapper wrapper = new PartyDataWrapper
        {
            members = new List<CharacterData> { character },
            inventory = new List<string>(),
            gold = 0
        };

        string path = Path.Combine(Application.persistentDataPath, "PartyData.json");
        string json = JsonUtility.ToJson(wrapper, true);
        File.WriteAllText(path, json);

        Debug.Log("Character saved to: " + path);
    }
}
