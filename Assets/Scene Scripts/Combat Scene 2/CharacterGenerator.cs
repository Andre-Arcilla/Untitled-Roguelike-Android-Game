using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterGenerator : MonoBehaviour
{
    public static CharacterGenerator Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    [SerializeField] private GameObject prefab;
    [SerializeField] private GameObject allyParent;
    [SerializeField] private GameObject enemyParent;
    [SerializeField] private EnemyDatabase enemyDatabase; // Your new enemy SO database

    private void Start()
    {
        GenerateParty();
        GenerateEnemy();
    }

    public void ChangeEnemies()
    {
        if (enemyParent != null)
        {
            foreach (Transform child in enemyParent.transform)
            {
                Destroy(child.gameObject);
                DOTween.Kill(child.gameObject);
            }
        }

        TargetingSystem.Instance.enemies.members.Clear();
        GenerateEnemy();
    }


    private void GenerateParty()
    {
        List<CharacterData> partyMembers = PlayerDataHolder.Instance.partyMembers;

        // Predefined positions for up to 4 party members
        Vector2[] positions = new Vector2[]
        {
            new Vector2(-0.75f, 0f),  // A
            new Vector2(-2.25f, 0f),  // B
            new Vector2(-1.5f, 0.5f), // C
            new Vector2(-3.0f, 0.5f)  // D
        };

        for (int i = 0; i < partyMembers.Count && i < positions.Length; i++)
        {
            GameObject characterObj = Instantiate(prefab);
            characterObj.transform.SetParent(allyParent.transform, false);

            GameObject childObj = characterObj.transform.Find("Character Sprite").gameObject;
            childObj.transform.localPosition = new Vector3(positions[i].x, positions[i].y, childObj.transform.localPosition.z);

            CharacterInfo info = characterObj.GetComponent<CharacterInfo>();
            info.characterData = partyMembers[i];
            info.gameObject.GetComponent<Targetable>().team = Target.Ally;

            TargetingSystem.Instance.allies.members.Add(info.gameObject.GetComponent<Targetable>());
            CharacterManager.Instance.characterList.Add(info.gameObject);

            info.Initialize();
        }

        Debug.Log("Generated " + partyMembers.Count + " party members.");
    }

    private void GenerateEnemy()
    {
        int count = Random.Range(1, 5); // 1 to 4 enemies

        Vector2[] positions = new Vector2[] {
        new Vector2(0.75f, 0f),
        new Vector2(2.25f, 0f),
        new Vector2(1.5f, 0.5f),
        new Vector2(3.0f, 0.5f)
    };

        for (int i = 0; i < count && i < positions.Length; i++)
        {
            CharacterDataSO randomEnemySO = enemyDatabase.allEnemies[Random.Range(0, enemyDatabase.allEnemies.Count)];
            CharacterData randomEnemy = ConvertSOToCharacterData(randomEnemySO);

            GameObject characterObj = Instantiate(prefab);
            characterObj.transform.SetParent(enemyParent.transform, false);
            characterObj.transform.Find("Card View").gameObject.SetActive(false);

            GameObject childObj = characterObj.transform.Find("Character Sprite").gameObject;
            childObj.transform.localPosition = new Vector3(positions[i].x, positions[i].y, childObj.transform.localPosition.z);

            CharacterInfo info = characterObj.GetComponent<CharacterInfo>();
            info.characterData = randomEnemy;
            info.gameObject.GetComponent<Targetable>().team = Target.Enemy;

            TargetingSystem.Instance.enemies.members.Add(info.gameObject.GetComponent<Targetable>());

            info.Initialize();
        }

        Debug.Log("Generated " + count + " enemies.");
    }

    private CharacterData ConvertSOToCharacterData(CharacterDataSO so)
    {
        CharacterData data = new CharacterData();

        // Copy Basic Info
        data.basicInfo.characterName = so.basicInfo.characterName;
        data.basicInfo.level = so.basicInfo.level;
        data.basicInfo.gender = so.basicInfo.gender;
        data.basicInfo.raceName = so.basicInfo.raceName;

        // Copy Allocated Stats
        data.allocatedStats.allocatedHP = so.allocatedStats.allocatedHP;
        data.allocatedStats.allocatedEN = so.allocatedStats.allocatedEN;
        data.allocatedStats.allocatedPWR = so.allocatedStats.allocatedPWR;
        data.allocatedStats.allocatedSPD = so.allocatedStats.allocatedSPD;

        // Copy Classes
        data.classes = new List<string>(so.classes);

        // Copy Equipment
        data.equipment.headGear = so.equipment.headGear;
        data.equipment.chestArmor = so.equipment.chestArmor;
        data.equipment.legwear = so.equipment.legwear;
        data.equipment.gloves = so.equipment.gloves;
        data.equipment.boots = so.equipment.boots;
        data.equipment.mainHand = so.equipment.mainHand;
        data.equipment.offHand = so.equipment.offHand;
        data.equipment.accessory1 = so.equipment.accessory1;
        data.equipment.accessory2 = so.equipment.accessory2;
        data.equipment.accessory3 = so.equipment.accessory3;

        return data;
    }
}
