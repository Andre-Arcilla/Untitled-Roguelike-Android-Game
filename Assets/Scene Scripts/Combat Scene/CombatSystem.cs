using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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

        waves = TownManager.Instance?.waves ?? 2;
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

    [SerializeField] private int waves = 1;
    [SerializeField] private int currentWave = 0;
    [SerializeField] private TMP_Text waveText;

    public void GenerateNewEnemyGroup()
    {
        if (currentWave < waves)
        {
            Debug.Log("current wave" + currentWave);
            Debug.Log("wave" + waves);
            CharacterGenerator.Instance.ChangeEnemies();
            currentWave++;
            waveText.text = $"Encounters: {currentWave}/{waves}";
        }
        else if (currentWave >= waves)
        {
            Debug.Log("current wave" + currentWave);
            Debug.Log("wave" + waves);
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
        if (TownManager.Instance.isLabyrinth)
        {
            string townName = TownManager.Instance.townTo.townName;
            var entry = PlayerDataHolder.Instance.partyClearedTowns.Find(e => e.townName == townName);

            if (entry != null)
            {
                entry.hasCleared = true;
            }
            else
            {
                Debug.LogWarning($"Town '{townName}' not found in clearedTowns list.");
            }
        }

        SceneManager.LoadScene("Victory Screen", LoadSceneMode.Additive);
    }

    public void CombatCompleteLose(bool playerLose)
    {
        if (!playerLose)
        {
            return;
        }

        SceneManager.LoadScene(TownManager.Instance.townFrom.sceneName);
    }
}
