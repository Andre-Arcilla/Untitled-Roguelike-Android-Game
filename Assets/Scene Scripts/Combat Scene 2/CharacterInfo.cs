using SerializeReferenceEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterInfo : MonoBehaviour
{
    [Header("Character Resources")]
    [SerializeField] public int currentHP;
    [SerializeField] public int currentEN;
    [SerializeField] public int maxHP;
    [SerializeField] public int barrier;

    [Header("Character Information")]
    [SerializeField] private GameObject spriteHolder;
    [SerializeField] private GameObject sprite;
    [SerializeField] private ClassDatabase classDatabase;
    [SerializeField] private ClassDatabase enemyClassDatabase;
    [SerializeField] private RaceDatabase raceDatabase;
    [SerializeField] private CharacterDeck characterDeck;
    [SerializeField] private bool useSaveFile; //temp
    [SerializeField] public CharacterData characterData;
    [SerializeField] public Stats stats;
    [SerializeField] private List<CardDataSO> deck;
    [SerializeReference, SR] public List<IStatusEffect> activeEffects = new List<IStatusEffect>();


    [System.Serializable]
    public class Stats
    {
        public int totalHP;
        public int totalEN;
        public int totalPWR;
        public int totalSPD;
    }

    public void Initialize()
    { 
        gameObject.name = characterData.basicInfo.characterName;

        GenerateCharacterStats();
        GenerateCharacterDeck();
        GenerateCharacterSprite();
        characterDeck.SetDeck(deck);
    }

    public void SetResources()
    {
        currentEN = stats.totalEN / 5;
        maxHP = stats.totalHP / 2;
        currentHP = maxHP;
    }

    private void GenerateCharacterStats()
    {
        Debug.Log(gameObject.name);
        RaceDataSO selectedRace = raceDatabase.allRaces.Find(r => r.raceName == characterData.basicInfo.raceName);
        stats.totalHP = selectedRace.HP + characterData.allocatedStats.allocatedHP;
        stats.totalEN = selectedRace.EN + characterData.allocatedStats.allocatedEN;
        stats.totalPWR = selectedRace.PWR + characterData.allocatedStats.allocatedPWR;
        stats.totalSPD = selectedRace.SPD + characterData.allocatedStats.allocatedSPD;

        SetResources();
    }

    private void GenerateCharacterSprite()
    {
        Debug.Log("sprite get for " + characterData.basicInfo.characterName);

        string targetClassName = characterData.classes[0]; // First class
        string playerGender = characterData.basicInfo.gender.ToLower();

        if (gameObject.GetComponent<Targetable>().team == Team.Player)
        {
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
        else if (gameObject.GetComponent<Targetable>().team == Team.Enemy)
        {
            ClassDataSO enemyClass = enemyClassDatabase.allClasses.Find(c => c.className == targetClassName);

            Vector3 position = sprite.transform.position;
            Destroy(sprite);
            sprite = Instantiate(enemyClass.spriteMale, position, Quaternion.identity, spriteHolder.transform);
            sprite.transform.localRotation = Quaternion.Euler(0, 180, 0);
        }
    }

    private void GenerateCharacterDeck()
    {
        foreach (var targetClassName in characterData.classes)
        {
            ClassDataSO selectedClass = null;

            if (gameObject.GetComponent<Targetable>().team == Team.Player)
            {
                selectedClass = classDatabase.allClasses.Find(c => c.className == targetClassName);

                if (selectedClass == null)
                {
                    Debug.LogError($"Class '{targetClassName}' not found in database.");
                    continue;
                }
            }
            else if (gameObject.GetComponent<Targetable>().team == Team.Enemy)
            {
                selectedClass = enemyClassDatabase.allClasses.Find(c => c.className == targetClassName);

                if (selectedClass == null)
                {
                    Debug.LogError($"Class '{targetClassName}' not found in database.");
                    continue;
                }
            }

                // Merge the current class's deck into the character's deck
                deck.AddRange(selectedClass.startingDeck);
        }
    }

    public void ApplyStatusEffect(IStatusEffect newEffect)
    {
        foreach (var effect in activeEffects)
        {
            if (effect.Name == newEffect.Name)
            {
                effect.Duration += newEffect.Duration;
                Debug.Log($"{effect.Name} effect duration extended by {newEffect.Duration} turns.");
                return;
            }
        }

        newEffect.OnApply(this);
        activeEffects.Add(newEffect);
    }

    public void OnTurnStart()
    {
        var expiredEffects = new List<IStatusEffect>();

        foreach (var effect in activeEffects)
        {
            effect.OnTurnStart();

            if (effect.IsExpired)
                expiredEffects.Add(effect);
        }

        foreach (var expired in expiredEffects)
        {
            expired.OnRemove();
            activeEffects.Remove(expired);
        }
    }

    public void OnTurnEnd()
    {
        var expiredThisRound = new List<IStatusEffect>();

        foreach (var effect in activeEffects)
        {
            if (effect.IsShortTerm)
                expiredThisRound.Add(effect);
        }

        foreach (var effect in expiredThisRound)
        {
            effect.OnRemove();
            activeEffects.Remove(effect);
        }
    }

    public bool TriggerOnHitEffects(CharacterInfo attacker)
    {
        bool damageNegated = false;
        List<IStatusEffect> toRemove = new();

        foreach (var effect in activeEffects)
        {
            if (effect is CounterStatusEffect counter)
            {
                counter.OnTrigger(attacker);
                if (counter.NegatesDamage)
                {
                    damageNegated = true;
                }
                if (counter.ExpiresOnHit)
                {
                    toRemove.Add(effect);
                }
            }
            else if (effect is DodgeStatusEffect dodge)
            {
                dodge.OnTrigger(attacker);
                damageNegated = true;
                if (dodge.ExpiresOnHit)
                {
                    toRemove.Add(effect);
                }
            }
        }

        foreach (var effect in toRemove)
        {
            effect.OnRemove();
            activeEffects.Remove(effect);
        }

        return damageNegated;
    }
}

