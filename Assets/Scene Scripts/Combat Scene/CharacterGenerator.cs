using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;

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
    [SerializeField] private EnemyDatabase enemyDatabase;
    [SerializeField] private List<BackgroundController> backgrounds;
    [SerializeField] private CloudSpawner clouds;
    [SerializeField] public float moveSpeed => backgrounds.Find(bg => bg.name == "floor bg").parallaxSpeed;
    [SerializeField] public float moveDelay = 2f;
    [SerializeField] private Button endTurnBtn;
    [SerializeField] private List<GameObject> allyStatuses;
    [SerializeField] private List<GameObject> enemyStatuses;
    [SerializeField] private GameObject statusEffectBox;
    private bool isStatusVisible = false;

    private void Start()
    {

        GenerateParty();

        DisablePlayerRaycasts();
        DOVirtual.DelayedCall(1f, () =>
        {
            CombatSystem.Instance.GenerateNewEnemyGroup();
        });

        CharacterManager.Instance.SetCardViews();
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

        enemyStatuses.Clear();
        TargetingSystem.Instance.enemies.members.Clear();
        EnemyActionsManager.Instance.enemyList.Clear();

        DisablePlayerRaycasts();
        DOVirtual.DelayedCall(1f, () =>
        {
            GenerateEnemy();
        });
    }


    private void GenerateParty()
    {
        List<CharacterData> partyMembers = PlayerDataHolder.Instance.partyMembers;

        int positionIndex = 0;
        Vector2[] positions = new Vector2[]
        {
            new Vector2(-0.75f, 0.35f),
            new Vector2(-1.5f, 0.35f),
            new Vector2(-2.25f, 0.35f),
            new Vector2(-3.0f, 0.35f)
        };

        for (int i = 0; i < partyMembers.Count && i < positions.Length; i++)
        {
            GameObject characterObj = Instantiate(prefab);
            characterObj.transform.SetParent(allyParent.transform, false);

            GameObject childObj = characterObj.transform.Find("Character Sprite").gameObject;
            childObj.transform.localPosition = new Vector3(positions[positionIndex].x, positions[positionIndex].y, childObj.transform.localPosition.z);

            CharacterInfo info = characterObj.GetComponent<CharacterInfo>();
            info.characterData = partyMembers[i];

            if (info.characterData.isAlive == false)
            {
                Destroy(characterObj);
                continue;
            }

            info.gameObject.GetComponent<Targetable>().team = Team.Player;

            allyStatuses.Add(childObj.transform.Find("Status Effects").gameObject);
            TargetingSystem.Instance.allies.members.Add(info.gameObject.GetComponent<Targetable>());
            CharacterManager.Instance.characterList.Add(info.gameObject);
            info.Initialize();
            positionIndex++;
        }

        Debug.Log("Generated " + partyMembers.Count + " party members.");
    }

    private void GenerateEnemy()
    {
        int count = Random.Range(1, 5);

        Vector2[] positions = new Vector2[] 
        {
            new Vector2(0.75f, 0.35f),
            new Vector2(1.5f, 0.35f),
            new Vector2(2.25f, 0.35f),
            new Vector2(3.0f, 0.35f)
        };

        enemyParent.transform.DOKill();
        enemyParent.transform.position = new Vector3(allyParent.transform.position.x + 15, 0f);
        
        for (int i = 0; i < count && i < positions.Length; i++)
        {
            CharacterDataSO randomEnemySO = enemyDatabase.allEnemies[Random.Range(0, enemyDatabase.allEnemies.Count)];
            CharacterData randomEnemy = ConvertSOToCharacterData(randomEnemySO);

            GameObject characterObj = Instantiate(prefab);
            characterObj.transform.SetParent(enemyParent.transform, false);
            //characterObj.transform.Find("Card View").gameObject.SetActive(false);

            GameObject childObj = characterObj.transform.Find("Character Sprite").gameObject;
            childObj.transform.localPosition = new Vector3(positions[i].x, positions[i].y, childObj.transform.localPosition.z);

            CharacterInfo info = characterObj.GetComponent<CharacterInfo>();
            info.characterData = randomEnemy;
            info.gameObject.GetComponent<Targetable>().team = Team.Enemy;

            enemyStatuses.Add(childObj.transform.Find("Status Effects").gameObject);
            TargetingSystem.Instance.enemies.members.Add(info.gameObject.GetComponent<Targetable>());
            EnemyActionsManager.Instance.AddEnemyDeck(info);

            info.Initialize();
        }

        StartCoroutine(MoveAndScrollCoroutine(Vector3.zero));
    }

    private IEnumerator MoveAndScrollCoroutine(Vector3 targetPos)
    {
        CharacterManager.Instance.DisplayCardView();
        DisablePlayerRaycasts();
        clouds.driftSpeed = 3;

        foreach (Transform child in allyParent.transform)
        {
            Animator anim = child.GetComponentInChildren<Animator>();
            if (anim != null && child.GetComponentInChildren<CharacterInfo>().currentHP > 0)
            {
                anim.SetTrigger("doWalk");
            }
        }

        while (Vector3.Distance(enemyParent.transform.position, targetPos) > 0.01f)
        {
            // Move enemyParent towards target
            enemyParent.transform.position = Vector3.MoveTowards(enemyParent.transform.position, targetPos, moveSpeed * Time.deltaTime);

            // Scroll backgrounds based on deltaTime
            foreach (var bg in backgrounds)
            {
                bg.Scroll(Time.deltaTime);
            }

            yield return new WaitForFixedUpdate(); // wait for next frame
        }

        foreach (Transform child in allyParent.transform)
        {
            Animator anim = child.GetComponentInChildren<Animator>();
            if (anim != null && child.GetComponentInChildren<CharacterInfo>().currentHP > 0)
            {
                anim.SetTrigger("doStop");
            }
        }

        CharacterManager.Instance.SelectFirstCharacter();
        EnablePlayerRaycasts();
        clouds.driftSpeed = 1;
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
        data.equipment.armor = so.equipment.armor;
        data.equipment.weapon = so.equipment.weapon;
        data.equipment.accessory1 = so.equipment.accessory1;
        data.equipment.accessory2 = so.equipment.accessory2;
        data.equipment.accessory3 = so.equipment.accessory3;

        return data;
    }

    public void DisablePlayerRaycasts()
    {
        foreach (Transform child in allyParent.transform)
        {
            CharacterInfo character = child.GetComponentInChildren<CharacterInfo>();
            if (character.currentHP <= 0)
            {
                character.transform.Find("Character Sprite").GetComponentInChildren<Collider2D>().enabled = false;
            }
        }

        Camera.main.GetComponent<Physics2DRaycaster>().enabled = false;
        endTurnBtn.interactable = false;
    }

    public void EnablePlayerRaycasts()
    {
        foreach (Transform child in allyParent.transform)
        {
            CharacterInfo character = child.GetComponentInChildren<CharacterInfo>();
            if (character.currentHP <= 0)
            {
                character.transform.Find("Character Sprite").GetComponentInChildren<Collider2D>().enabled = false;
            }
        }

        Camera.main.GetComponent<Physics2DRaycaster>().enabled = true;
        endTurnBtn.interactable = true;
    }

    public void StatusEffectView()
    {
        if (isStatusVisible)
        {
            HideStatusEffect();
        }
        else
        {
            ShowStatusEffect();
        }
    }

    private void ShowStatusEffect()
    {
        statusEffectBox.SetActive(true);

        foreach (GameObject view in allyStatuses)
        {
            view.SetActive(true);
        }

        foreach (GameObject view in enemyStatuses)
        {
            view.SetActive(true);
        }
        isStatusVisible = true;
    }

    private void HideStatusEffect()
    {
        statusEffectBox.SetActive(false);

        foreach (GameObject view in allyStatuses)
        {
            view.SetActive(false);
        }
        foreach (GameObject view in enemyStatuses)
        {
            view.SetActive(false);
        }
        isStatusVisible = false;
    }
}
