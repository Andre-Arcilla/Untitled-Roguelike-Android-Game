using System.Collections.Generic;
using System.IO;
using Unity.Collections;
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
        // Basic Info
        public PlayerInfo basicInfo;

        // Allocated Stats
        public PlayerStats allocatedStats;

        // Classes
        public List<string> classes = new List<string>();

        // Equipment section
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
        }

        [System.Serializable]
        public class PlayerEquipment
        {
            public string headGear;
            public string chestArmor;
            public string legwear;
            public string gloves;
            public string boots;
            public string mainHand;
            public string offHand;
            public string accessory1;
            public string accessory2;
            public string accessory3;
        }
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
        // Basic Info
        playerData.basicInfo = new PlayerSaveData.PlayerInfo
        {
            playerName = "nionioiongf",  // Replace with actual player name if needed
            level = 1,  // Adjust the level as necessary
            gender = classManager.selectedGender,
            raceName = statManager.selectedRace.raceName
        };

        // Allocated Stats
        playerData.allocatedStats = new PlayerSaveData.PlayerStats
        {
            allocatedHP = statManager.allocatedHP,
            allocatedEN = statManager.allocatedEN,
            allocatedPWR = statManager.allocatedPWR,
            allocatedSPD = statManager.allocatedSPD
        };

        // Classes
        playerData.classes.Clear();
        playerData.classes.Add(classManager.selectedClass.className);

        // Equipment
        playerData.equipment = new PlayerSaveData.PlayerEquipment
        {
            headGear = null,
            chestArmor = null,
            legwear = null,
            gloves = null,
            boots = null,
            mainHand = null,
            offHand = null,
            accessory1 = null,
            accessory2 = null,
            accessory3 = null
        };

        string saveFile = JsonUtility.ToJson(playerData, true);
        string path = Path.Combine(Application.persistentDataPath, "Character1Data.json");
        File.WriteAllText(path, saveFile);

        Debug.Log("Player data saved to: " + path);
    }
}
