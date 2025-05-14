using UnityEngine;
using UnityEngine.SceneManagement;

public class CombatSystem : MonoBehaviour
{
    public static CombatSystem Instance { get; private set; }

    private void Awake()
    {
        // Check if an instance already exists
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // Only one allowed
            return;
        }

        Instance = this;
        waves = TownManager.Instance.waves;
        currentWave = 1;
    }

    //call a method if all enemies are dead
    //incremen current round
    //if current round is equal to rounds, stop and call another method

    [SerializeField] private int waves;
    [SerializeField] private int currentWave = 1;

    public void GenerateNewEnemyGroup()
    {
        if (currentWave == waves)
        {
            CombatCompleteWin();
            return;
        }
        else
        {
            CharacterGenerator.Instance.ChangeEnemies();
            currentWave++;
        }
    }

    private void CombatCompleteWin()
    {
        Debug.Log("|-------------------|");
        Debug.Log("|  Combat complete  |");
        Debug.Log("|-------------------|");

        SceneManager.LoadScene(TownManager.Instance.townTo.sceneName);
    }

    public void CombatCompleteLose()
    {
        Debug.Log("|----------------------|");
        Debug.Log("|  Your party is dead  |");
        Debug.Log("|----------------------|");

        SceneManager.LoadScene(TownManager.Instance.townFrom.sceneName);
    }
}