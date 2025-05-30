using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public class NextTownBtn : MonoBehaviour
{
    [SerializeField] private TownDataSO town;
    [SerializeField] private bool isLabyrinthEntrance = false;

    public void GoToTown()
    {
        if (town != null && !string.IsNullOrEmpty(town.sceneName))
        {
            TownManager.Instance.townFrom = TownSystem.Instance.currentTown;
            TownManager.Instance.townTo = town;

            int roll1 = Random.Range(3, 7);
            int roll2 = Random.Range(3, 7);
            int roll3 = Random.Range(3, 7);
            int roll4 = Random.Range(3, 7);
            int roll5 = Random.Range(3, 7);
            int roll6 = Random.Range(3, 7);
            Debug.Log("Roll 1: " + roll1);
            Debug.Log("Roll 1: " + roll2);
            Debug.Log("Roll 1: " + roll3);
            Debug.Log("Roll 1: " + roll4);
            Debug.Log("Roll 1: " + roll5);
            Debug.Log("Roll 1: " + roll6);

            int averageRoll = (roll1 + roll2 + roll3 + roll4 + roll5 + roll6) / 6;
            Debug.Log("Average Roll: " + averageRoll);
            Debug.Log("Random range: " + Random.Range(3, 7));

            TownManager.Instance.waves = averageRoll;

            TownManager.Instance.isLabyrinth = false;

            if (isLabyrinthEntrance)
            {
                TownManager.Instance.isLabyrinth = true;
            }
            SceneManager.LoadScene("Combat Scene 2");
        }
        else
        {
            Debug.LogError("Scene name is missing or destination town is null.");
        }
    }
}
