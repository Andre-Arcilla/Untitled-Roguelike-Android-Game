using System.Collections;
using System.Collections.Generic;
using Unity.Splines.Examples;
using Unity.VisualScripting;
using UnityEngine;

public class TavernManager : MonoBehaviour
{
    public static TavernManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    [Header("Generation Settings")]
    public int maxCharacters = 5;

    [Header("Databases")]
    [SerializeField] private ClassDatabase classDatabase;
    [SerializeField] private RaceDatabase raceDatabase;
    [SerializeField] private EquipmentDatabase equipmentDatabase;

    [Header("Generated Characters")]
    public List<CharacterData> tavernCharacters = new List<CharacterData>();

    [Header("others")]
    [SerializeField] public Transform parentHolder;
    [SerializeField] private GameObject listObjPrefab;
    [SerializeField] private CharDisplayInfo charDisplay;

    [Header("Name Pool")]
    [SerializeField]
    private List<string> possibleNames = new List<string>
    {
        "Arden", "Bryn", "Eliot", "Emery", "Finley",
        "Harlen", "Jaden", "Kiran", "Linden", "Marlowe",
        "Nova", "Oakley", "Parker", "Quinn", "Rory",
        "Sage", "Tegan", "Vale", "Wren", "Xen",
        "Yarden", "Zephyr", "Ash", "Bay", "Calder",
        "Dale", "Eden", "Fifer", "Gray", "Haven",
        "Ira", "Jules", "Kai", "Lior", "Marin",
        "Nico", "Onyx", "Perry", "Riven", "Sky",
        "Seigen", "Umber", "Vesper", "West", "Yael",
        "Zion", "Rowan", "Sutton", "Ellis", "Soren"
    };

    private void Start()
    {
        GenerateCharacters(AverageLevel());
        GenerateList();
        SendFirstItemData();
    }

    public void GenerateCharacters(int averageLevel)
    {
        tavernCharacters.Clear();
        int count = Random.Range(3, maxCharacters + 1);

        for (int i = 0; i < count; i++)
        {
            CharacterData newChar = new CharacterData();

            // Basic Info// Pick a unique name
            string uniqueName = GetUniqueName();
            newChar.basicInfo.characterName = uniqueName;
            newChar.basicInfo.level = Mathf.Clamp(averageLevel + Random.Range(-5, 6), 1, 99);
            newChar.basicInfo.xp = 0;
            newChar.basicInfo.gender = "Male";

            // Random class and race from databases
            var randomClass = classDatabase.allClasses[Random.Range(0, classDatabase.allClasses.Count)];
            var randomRace = raceDatabase.allRaces[Random.Range(0, raceDatabase.allRaces.Count)];
            newChar.classes.Add(randomClass.className);
            newChar.basicInfo.raceName = randomRace.raceName;

            // Allocate stats: 10 points per level
            int totalPoints = newChar.basicInfo.level * 10;
            AllocateStatsRandomly(newChar.allocatedStats, totalPoints);

            // Optional Equipment
            // Equipment: Rarity-based assignment
            int equipChance = Random.Range(0, 100);

            if (equipChance < 30) // 30% chance: full gear
            {
                newChar.equipment.armor = GetRandomEquipmentNameByType(EquipmentType.Armor);
                newChar.equipment.weapon = GetRandomEquipmentNameByType(EquipmentType.Weapon);
                newChar.equipment.accessory1 = GetRandomEquipmentNameByType(EquipmentType.Accessory);
                newChar.equipment.accessory2 = GetRandomEquipmentNameByType(EquipmentType.Accessory);
                newChar.equipment.accessory3 = GetRandomEquipmentNameByType(EquipmentType.Accessory);
            }
            else if (equipChance < 65) // 35% chance: partial gear
            {
                newChar.equipment.armor = GetRandomEquipmentNameByType(EquipmentType.Armor);
                newChar.equipment.weapon = GetRandomEquipmentNameByType(EquipmentType.Weapon);
                // Accessories remain empty
            }
            // else: no equipment


            newChar.isAlive = true;

            tavernCharacters.Add(newChar);
        }
    }

    private string GetUniqueName()
    {
        HashSet<string> takenNames = new HashSet<string>();

        // Add names from current party members
        foreach (var member in PlayerDataHolder.Instance.partyMembers)
        {
            if (!string.IsNullOrEmpty(member.basicInfo.characterName))
                takenNames.Add(member.basicInfo.characterName);
        }

        // Add names from already generated tavern characters to avoid duplicates in the batch
        foreach (var tavernChar in tavernCharacters)
        {
            if (!string.IsNullOrEmpty(tavernChar.basicInfo.characterName))
                takenNames.Add(tavernChar.basicInfo.characterName);
        }

        // Filter available names
        var availableNames = possibleNames.FindAll(name => !takenNames.Contains(name));

        if (availableNames.Count > 0)
        {
            return availableNames[Random.Range(0, availableNames.Count)];
        }
        else
        {
            // Fallback unique name
            string baseName = "Adventurer";
            int suffix = 1;
            while (takenNames.Contains($"{baseName}_{suffix}"))
            {
                suffix++;
            }
            return $"{baseName}_{suffix}";
        }
    }

    // Helper method
    private string GetRandomEquipmentNameByType(EquipmentType type)
    {
        var filtered = equipmentDatabase.allEquipments.FindAll(e => e.slotType == type);
        if (filtered.Count == 0) return "";
        return filtered[Random.Range(0, filtered.Count)].equipmentName;
    }

    private void AllocateStatsRandomly(CharacterData.AllocatedStats stats, int points)
    {
        int[] buckets = new int[4]; // HP, EN, PWR, SPD
        for (int i = 0; i < points; i++)
        {
            buckets[Random.Range(0, 4)]++;
        }

        stats.allocatedHP = buckets[0];
        stats.allocatedEN = buckets[1];
        stats.allocatedPWR = buckets[2];
        stats.allocatedSPD = buckets[3];
    }

    private int AverageLevel()
    {
        var party = PlayerDataHolder.Instance.partyMembers;

        if (party == null || party.Count == 0)
            return 1; // Default if no party

        int totalLevel = 0;
        foreach (var member in party)
        {
            totalLevel += member.basicInfo.level;
        }

        return Mathf.Max(1, totalLevel / party.Count);
    }

    private void GenerateList()
    {
        foreach (CharacterData character in tavernCharacters)
        {
            GameObject listObject = Instantiate(listObjPrefab, parentHolder);
            listObject.GetComponentInChildren<TavernCharacterInfo>().Setup(character);
        }
    }

    public void SendCharacterData(CharacterData character, GameObject listObject)
    {
        charDisplay.Setup(character, listObject);
    }

    public void RemoveItemFromList(GameObject listObject)
    {
        listObject.transform.SetParent(InventoryManager.Instance.trashcan);
        Destroy(listObject);
        SendFirstItemData();
    }

    private void SendFirstItemData()
    {
        if (parentHolder != null && parentHolder.childCount > 0)
        {
            parentHolder.GetChild(0).GetComponent<TavernCharacterInfo>().SendCharacterData();
            charDisplay.gameObject.SetActive(true);
        }
        else
        {
            charDisplay.gameObject.SetActive(false);
        }
    }
}
