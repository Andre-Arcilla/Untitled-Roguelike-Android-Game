using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class TownBtn : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private TownDataSO town;

    public void OnPointerClick(PointerEventData eventData)
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