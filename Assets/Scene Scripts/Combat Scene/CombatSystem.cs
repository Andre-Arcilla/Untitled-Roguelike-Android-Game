using SerializeReferenceEditor;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CombatSystem : MonoBehaviour
{
    public static CombatSystem Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        waves = TownManager.Instance?.waves ?? 1;
        currentWave = 1;
    }

    private void Start()
    {
        foreach (var member in PlayerDataHolder.Instance.partyMembers)
        {
            if (!killCounterList.Exists(e => e.character == member))
            {
                killCounterList.Add(new KillEntry
                {
                    character = member,
                    killCount = 0,
                    levelDiffs = new List<int>()
                });
            }
        }
    }

    [System.Serializable]
    public class KillEntry
    {
        public CharacterData character;
        public int killCount;
        public List<int> levelDiffs = new();
    }

    [SerializeField] private List<KillEntry> killCounterList = new List<KillEntry>();
    [SerializeField] private int goldEarned;
    public List<KillEntry> GetKillCounter() => killCounterList;
    public int goldAmount => goldEarned;

    [SerializeField] private int waves;
    [SerializeField] private int currentWave = 1;

    public void GenerateNewEnemyGroup()
    {
        if (currentWave < waves)
        {
            CharacterGenerator.Instance.ChangeEnemies();
            currentWave++;
        }
        else if (currentWave >= waves)
        {
            CombatCompleteWin();
        }
    }

    public void AddKillCount(CharacterData character, int levelDiff, int goldEarned)
    {
        this.goldEarned += goldEarned;
        levelDiff = Mathf.Max(levelDiff, 1);

        KillEntry entry = killCounterList.Find(e => e.character == character);

        if (entry != null)
        {
            entry.killCount++;
            entry.levelDiffs.Add(levelDiff);
        }
        else
        {
            KillEntry newEntry = new KillEntry
            {
                character = character,
                killCount = 1,
                levelDiffs = new List<int> { levelDiff }
            };

            killCounterList.Add(newEntry);
        }
    }

    private void CombatCompleteWin()
    {
        Debug.Log("|-------------------|");
        Debug.Log("|  Combat complete  |");
        Debug.Log("|-------------------|");

        SceneManager.LoadScene("Victory Screen", LoadSceneMode.Additive);
    }

    public void CombatCompleteLose(bool playerLose)
    {
        if (!playerLose)
        {
            return;
        }

        Debug.Log("|----------------------|");
        Debug.Log("|  Your party is dead  |");
        Debug.Log("|----------------------|");

        SceneManager.LoadScene(TownManager.Instance.townFrom.sceneName);
    }
}
