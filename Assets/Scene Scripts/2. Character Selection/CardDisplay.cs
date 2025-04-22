using UnityEngine;
using Unity.Collections;
using UnityEngine.UI;

public class CardDisplay : MonoBehaviour
{
    [SerializeField] private GameObject cardPrefab; // UI Image in your Canvas
    [SerializeField] private Transform cardParent;
    [SerializeField] private ClassManager classManager;
    [SerializeField] private ClassDataSO selectedClass;

    public void UpdateSelectedClass(ClassDataSO @class)
    {
        selectedClass = @class;

        // Optional: clear existing cards before generating new ones
        foreach (Transform child in cardParent)
        {
            Destroy(child.gameObject);
        }

        // Generate cards from startingDeck
        foreach (CardDataSO card in selectedClass.startingDeck)
        {
            GameObject newCard = Instantiate(cardPrefab, cardParent);

            // Get the Image component from the prefab (or child if needed)
            Image image = newCard.GetComponent<Image>();

            if (image != null && card.cardSprite != null)
            {
                image.sprite = card.cardSprite;
            }
        }
    }
}