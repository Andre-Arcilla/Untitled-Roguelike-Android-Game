using SerializeReferenceEditor;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static UnityEditor.Progress;

public class CharacterInfo : MonoBehaviour
{
    [Header("Character Resources")]
    [SerializeField] public int currentHP;
    [SerializeField] public int currentEN;
    [SerializeField] public int barrier;
    [SerializeField] public int maxHP;
    [SerializeField] public int maxEN;
    [SerializeField] private TMP_Text HPText;
    [SerializeField] private TMP_Text ENText;

    [Header("Character Information")]
    [SerializeField] private GameObject spriteHolder;
    [SerializeField] private GameObject sprite;
    [SerializeField] private CharacterDeck characterDeck;
    [SerializeField] public CharacterData characterData;
    [SerializeField] public Stats stats;
    [SerializeField] public List<EquipmentDataSO> equipmentList;
    [SerializeField] private List<CardDataSO> deck;
    [SerializeReference, SR] public List<IStatusEffect> activeEffects = new List<IStatusEffect>();

    [Header("Others")]
    [SerializeField] private ClassDatabase enemyClassDatabase;
    [SerializeField] private ClassDatabase classDatabase;
    [SerializeField] private RaceDatabase raceDatabase;
    [SerializeField] private EquipmentDatabase equipmentDatabase;

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
        SetCharacterEquipment();
        GenerateCharacterSprite();
        characterDeck.SetDeck(deck);
    }

    public void SetResources()
    {
        if (gameObject.GetComponent<Targetable>().team == Team.Enemy)
        {
            ENText.transform.parent.gameObject.SetActive(false);
        }

        maxEN = stats.totalEN / 5;
        maxHP = Mathf.FloorToInt(stats.totalHP / 1.5f);
        currentHP = maxHP;
        currentEN = maxEN;
        UpdateResourcesView();
    }

    public void EndTurnRestoreMana()
    {
        maxEN += 2;
        currentEN = maxEN;
    }

    private void GenerateCharacterStats()
    {
        RaceDataSO selectedRace = raceDatabase.allRaces.Find(r => r.raceName == characterData.basicInfo.raceName);
        stats.totalHP = selectedRace.HP + characterData.allocatedStats.allocatedHP;
        stats.totalEN = selectedRace.EN + characterData.allocatedStats.allocatedEN;
        stats.totalPWR = selectedRace.PWR + characterData.allocatedStats.allocatedPWR;
        stats.totalSPD = selectedRace.SPD + characterData.allocatedStats.allocatedSPD;

        SetResources();
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

    private void SetCharacterEquipment()
    {
        //give effects

        List<string> equipmentNames = new List<string>
        {
            characterData.equipment.armor,
            characterData.equipment.weapon,
            characterData.equipment.accessory1,
            characterData.equipment.accessory2,
            characterData.equipment.accessory3
        };

        foreach (string equipmentName in equipmentNames)
        {
            if (string.IsNullOrWhiteSpace(equipmentName)) continue;

            EquipmentDataSO foundEquipment = equipmentDatabase.allEquipments.Find(i => i.equipmentName == equipmentName);

            if (foundEquipment != null)
            {
                //set the equipment to character list
                equipmentList.Add(foundEquipment);

                //add bonus stats to character stats
                stats.totalHP += foundEquipment.bonusHP;
                stats.totalEN += foundEquipment.bonusEN;
                stats.totalPWR += foundEquipment.bonusPWR;
                stats.totalSPD += foundEquipment.bonusSPD;

                //add bonus cards to character deck
                if (foundEquipment.cards != null && foundEquipment.cards.Count > 0)
                {
                    deck.AddRange(foundEquipment.cards);
                }

                //add bonus effects to character's effects list
                if (foundEquipment.effects != null && foundEquipment.effects.Count > 0)
                {
                    foreach (IStatusEffect effect in foundEquipment.effects)
                    {
                        IStatusEffect cloned = CloneStatusEffect(effect);
                        ApplyStatusEffect(cloned);
                    }
                }
            }
        }
    }

    private void GenerateCharacterSprite()
    {
        string targetClassName = characterData.classes[0];
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
            sprite.AddComponent<DisplayCardView>();
            sprite.GetComponent<DisplayCardView>().SetCardView(gameObject.transform.Find("Card View").gameObject);
        }
        else if (gameObject.GetComponent<Targetable>().team == Team.Enemy)
        {
            ClassDataSO enemyClass = enemyClassDatabase.allClasses.Find(c => c.className == targetClassName);

            Vector3 position = sprite.transform.position;
            Destroy(sprite);
            sprite = Instantiate(enemyClass.spriteMale, position, Quaternion.identity, spriteHolder.transform);
            sprite.transform.localRotation = Quaternion.Euler(0, 180, 0);
        }
        sprite.AddComponent<PolygonCollider2D>();
        sprite.AddComponent<Rigidbody2D>();
        sprite.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
        gameObject.GetComponent<Targetable>().targetCollider = sprite.GetComponent<Collider2D>();
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

    public static IStatusEffect CloneStatusEffect(IStatusEffect original)
    {
        string json = JsonUtility.ToJson(original);
        return (IStatusEffect)JsonUtility.FromJson(json, original.GetType());
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

    public void UpdateResourcesView()
    {
        HPText.text = "×" + currentHP.ToString();
        ENText.text = "×" + currentEN.ToString();
    }
}

