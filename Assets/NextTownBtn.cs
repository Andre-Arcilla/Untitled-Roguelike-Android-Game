using UnityEngine.SceneManagement;
using UnityEngine;

public class NextTownBtn : MonoBehaviour
{
    [SerializeField] private TownDataSO town;

    public void GoToTown()
    {
        if (town != null && !string.IsNullOrEmpty(town.sceneName))
        {
            SceneManager.LoadScene(town.sceneName);
        }
        else
        {
            Debug.LogError("Scene name is missing or destination town is null.");
        }
    }
}
