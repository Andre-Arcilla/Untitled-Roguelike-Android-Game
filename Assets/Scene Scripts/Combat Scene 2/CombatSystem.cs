using UnityEngine;

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
    }

    //call a method if all enemies are dead
    //incremen current round
    //if current round is equal to rounds, stop and call another method

    [SerializeField] private int rounds;
    [SerializeField] private int currentRound = 0;

    public void GenerateNewEnemyGroup()
    {
        if (currentRound == rounds)
        {
            CombatCompleteWin();
            return;
        }
        else
        {
            CharacterGenerator.Instance.ChangeEnemies();
            currentRound++;
        }
    }

    private void CombatCompleteWin()
    {
        Debug.Log("|-------------------|");
        Debug.Log("|  Combat complete  |");
        Debug.Log("|-------------------|");
    }

    public void CombatCompleteLose()
    {
        Debug.Log("|-------------------|");
        Debug.Log("|  Combat complete  |");
        Debug.Log("|-------------------|");
    }
}