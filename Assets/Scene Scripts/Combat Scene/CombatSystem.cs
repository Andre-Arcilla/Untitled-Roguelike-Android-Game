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
            if (!xpGainList.Exists(e => e.character == member))
            {
                xpGainList.Add(new XPGain
                {
                    character = member,
                    XP = 0
                });
            }
        }

        if (TownManager.Instance.isLabyrinth)
        {
            wall1.SetActive(true);
            wall2.SetActive(true);
        }
        else
        {
            wall1.SetActive(false);
            wall2.SetActive(false);
        }
    }

    [System.Serializable]
    public class XPGain
    {
        public CharacterData character;
        public int XP;
    }

    [SerializeField] private List<XPGain> xpGainList = new List<XPGain>();
    [SerializeField] private int goldEarned;
    public List<XPGain> GetXPGainList() => xpGainList;
    public int goldAmount => goldEarned;

    [SerializeField] private int waves = 1;
    [SerializeField] private int currentWave = 0;
    [SerializeField] private TMP_Text waveText;
    [SerializeField] private GameObject wall1;
    [SerializeField] private GameObject wall2;

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

    public void AddGoldFound(int goldEarned)
    {
        this.goldEarned += goldEarned;
    }

    public void AddXP(CharacterData character, int XP)
    {
        XPGain entry = xpGainList.Find(e => e.character == character);

        if (entry != null)
        {
            entry.XP += XP;
        }
        else
        {
            XPGain newEntry = new XPGain
            {
                character = character,
                XP = XP
            };
            xpGainList.Add(newEntry);
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
