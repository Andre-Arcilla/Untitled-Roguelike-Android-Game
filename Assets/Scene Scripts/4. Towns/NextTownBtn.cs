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

            TownManager.Instance.waves = Random.Range(3, 7);

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
