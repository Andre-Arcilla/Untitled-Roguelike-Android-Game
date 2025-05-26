using UnityEngine;
using UnityEngine.EventSystems;

public class DisplayCardView : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private GameObject cardView;

    public void OnPointerClick(PointerEventData eventData)
    {
        GameObject parentGO = cardView?.transform?.parent?.gameObject;

        if (parentGO != null && CharacterManager.Instance.characterList.Contains(parentGO))
        {
            CharacterManager.Instance.DisplayCardView(cardView);
        }
        else
        {
            Debug.LogWarning("Parent GameObject not found or not in characterList.");
        }
    }

    public void SetCardView(GameObject newCardView)
    {
        cardView = newCardView;
    }
}
