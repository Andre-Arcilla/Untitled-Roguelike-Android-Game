using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using DG.Tweening;

public class TownBtn : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private TownDataSO town;

    private void Start()
    {
        gameObject.transform.DOScale(2.2f, 0.5f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutSine);
    }

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