using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterInfo : MonoBehaviour
{
    [SerializeField] private GameObject spriteHolder;
    [SerializeField] private GameObject sprite;
    [SerializeField] private ClassDatabase classDatabase;
    [SerializeField] private RaceDatabase raceDatabase;
    [SerializeField] private CharacterDeck characterDeck;
    [SerializeField] private bool useSaveFile; //temp
    [SerializeField] public CharacterData characterData; //remove serializefield
    [SerializeField] public Stats stats; //remove serializefield
    [SerializeField] private List<CardDataSO> deck; //remove serializefield
    [SerializeField] public int currentMana; //remove serializefield

    [System.Serializable]
    public class Stats
    {
        public int totalHP;
        public int totalEN;
        public int totalPWR;
        public int totalSPD;
    }

    private void Start()
    {
        if (useSaveFile == true)
        {
            characterData = PlayerDataHolder.Instance.playerData;
        }

        GenerateCharacterStats();
        GenerateCharacterDeck();
        GenerateCharacterSprite();
        characterDeck.SetDeck(deck);
    }

    public void setMana()
    {
        currentMana = stats.totalEN / 5;
    }

    private void GenerateCharacterStats()
    {
        RaceDataSO selectedRace = raceDatabase.allRaces.Find(r => r.raceName == characterData.basicInfo.raceName);
        stats.totalHP = selectedRace.HP + characterData.allocatedStats.allocatedHP;
        stats.totalEN = selectedRace.EN + characterData.allocatedStats.allocatedEN;
        stats.totalPWR = selectedRace.PWR + characterData.allocatedStats.allocatedPWR;
        stats.totalSPD = selectedRace.SPD + characterData.allocatedStats.allocatedSPD;

        setMana();
    }

    private void GenerateCharacterSprite()
    {
        Debug.Log("sprite get for " + characterData.basicInfo.playerName);

        string targetClassName = characterData.classes[0]; // First class
        string playerGender = characterData.basicInfo.gender.ToLower();

        ClassDataSO selectedClass = classDatabase.allClasses.Find(c => c.className == targetClassName);

        if (playerGender == "male")
        {
            Vector3 position = sprite.transform.position;
            Destroy(sprite);
            sprite = Instantiate(selectedClass.spriteMale, position, Quaternion.identity, spriteHolder.transform);
        }
        else if (playerGender == "female")
        {
            Vector3 position = sprite.transform.position;
            Destroy(sprite);
            sprite = Instantiate(selectedClass.spriteFemale, position, Quaternion.identity, spriteHolder.transform);
        }
    }

    private void GenerateCharacterDeck()
    {
        foreach (var targetClassName in characterData.classes)
        {
            ClassDataSO selectedClass = classDatabase.allClasses.Find(c => c.className == targetClassName);

            if (selectedClass == null)
            {
                Debug.LogError($"Class '{targetClassName}' not found in database.");
                continue; // Skip to the next class if the current one is not found
            }

            // Merge the current class's deck into the character's deck
            deck.AddRange(selectedClass.startingDeck);
        }
    }
}

