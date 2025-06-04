using SerializeReferenceEditor;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.XR;

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
    [SerializeField] private TMP_Text statusesText;

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
        SetResources();
        GenerateCharacterSprite();
        characterDeck.SetDeck(deck);
        UpdateResourcesView();
    }

    public void SetResources()
    {
        if (gameObject.GetComponent<Targetable>().team == Team.Enemy)
        {
            ENText.transform.parent.gameObject.SetActive(false);
        }

        maxEN = stats.totalEN / 5;
        maxHP = stats.totalHP;
        currentHP = maxHP;
        currentEN = maxEN;
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
        // Check for stacking
        if (newEffect.AllowsStacking)
        {
            newEffect.OnApply(this);
            activeEffects.Add(newEffect);
            return;
        }

        for (int i = 0; i < activeEffects.Count; i++)
        {
            var existing = activeEffects[i];

            if (existing.Name == newEffect.Name)
            {
                if (newEffect.Override)
                {
                    existing.OnRemove();
                    activeEffects[i] = newEffect;
                    newEffect.OnApply(this);
                    Debug.Log($"{existing.Name} effect overridden.");
                }
                else
                {
                    existing.Duration += newEffect.Duration;
                    Debug.Log($"{existing.Name} effect duration extended by {newEffect.Duration} turns.");
                }
                return;
            }
        }

        // If no match found, add it
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

    public bool TriggerOnHitEffects(CharacterInfo attacker, int damage)
    {
        bool damageNegated = false;
        List<IStatusEffect> toRemove = new();

        foreach (var effect in activeEffects)
        {
            if (effect is CounterStatusEffect counter)
            {
                counter.OnTrigger(attacker, damage);
                if (counter.NegatesDamage)
                {
                    damageNegated = true;
                }

                if (counter.Duration <= 0 || counter.ExpiresOnHit)
                {
                    toRemove.Add(effect);
                }
            }
            else if (effect is BlockStatusEffect block)
            {
                block.OnTrigger(attacker);
                damageNegated = true;

                block.Duration--;
                if (block.Duration <= 0 || block.ExpiresOnHit)
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

    public int ApplyPreDamageModifiers(int baseDamage)
    {
        int modifiedDamage = baseDamage;

        foreach (var effect in activeEffects)
        {
            if (effect is DamageReductionStatusEffect dmgReduce)
            {
                modifiedDamage = dmgReduce.ModifyIncomingDamage(modifiedDamage);
            }
        }

        return modifiedDamage;
    }

    public void UpdateResourcesView()
    {
        if (currentHP <= 0)
        {
            activeEffects.Clear();
        }

        HPText.text = "×" + currentHP.ToString();
        ENText.text = "×" + currentEN.ToString();

        foreach (GameObject card in characterDeck.hand)
        {
            if (!card.GetComponent<CardInformation>().isSelected)
            {
                card.GetComponent<CardInformation>().card.UpdateManaCost(currentEN);
                card.GetComponent<CardInformation>().UpdateCard();
            }
        }

        statusesText.text = "";

        string Normalize(string input) => input.Replace(" ", "").ToLowerInvariant();

        if (activeEffects.Any(e =>
           (Normalize(e.Name) == "healthup" ||
            Normalize(e.Name) == "energyup" ||
            Normalize(e.Name) == "powerup" ||
            Normalize(e.Name) == "speedup")
             && e.IsDebuff == false))
        {
            statusesText.text += "<color=#00D6FF>Stat Up</color>\n";
        }
        if (activeEffects.Any(e => Normalize(e.Name) == "damagereduction"))
        {
            statusesText.text += "<color=#00D6FF>Dmg Red</color>\n";
        }
        if (activeEffects.Any(e => Normalize(e.Name) == "taunt"))
        {
            statusesText.text += "<color=#00D6FF>Taunt</color>\n";
        }
        var counterEffects = activeEffects.Where(e => Normalize(e.Name) == "counter").ToList();
        if (counterEffects.Any())
        {
            int totalcounterDuration = counterEffects.Sum(e => e.Duration);
            statusesText.text += $"<color=#00D6FF>{totalcounterDuration}× Counter</color>\n";
        }
        var dodgeEffects = activeEffects.Where(e => Normalize(e.Name) == "block").ToList();
        if (dodgeEffects.Any())
        {
            int totaldodgeDuration = dodgeEffects.Sum(e => e.Duration);
            statusesText.text += $"<color=#00D6FF>{totaldodgeDuration}× Block</color>\n";
        }
        var regenerationEffects = activeEffects.Where(e => Normalize(e.Name) == "regeneration").ToList();
        if (regenerationEffects.Any())
        {
            int totalregenerationDuration = regenerationEffects.Sum(e => e.Duration);
            statusesText.text += $"<color=#00D6FF>{totalregenerationDuration}× Regeneration</color>\n";
        }

        //check for debuffs
        if (activeEffects.Any(e =>
           (Normalize(e.Name) == "healthup" ||
            Normalize(e.Name) == "energyup" ||
            Normalize(e.Name) == "powerup" ||
            Normalize(e.Name) == "speedup")
             && e.IsDebuff))
        {
            statusesText.text += "<color=#FF6F5D>Stat Down</color>\n";
        }
        if (activeEffects.Any(e => Normalize(e.Name) == "lockdown"))
        {
            statusesText.text += "<color=#FF6F5D>Lock Down</color>\n";
        }
        var burnEffects = activeEffects.Where(e => Normalize(e.Name) == "burn").ToList();
        if (burnEffects.Any())
        {
            int totalBurnDuration = burnEffects.Sum(e => e.Duration);
            statusesText.text += $"<color=#FF6F5D>{totalBurnDuration}× Burn</color>\n";
        }
    }
}

