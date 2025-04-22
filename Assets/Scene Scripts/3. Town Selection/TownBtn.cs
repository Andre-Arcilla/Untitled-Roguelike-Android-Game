using UnityEditor.EditorTools;
using UnityEngine.SceneManagement;
using UnityEngine;

public class TownBtn : MonoBehaviour
{
    [SerializeField] private TownDataSO town;

    public void OnMouseDown()
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
