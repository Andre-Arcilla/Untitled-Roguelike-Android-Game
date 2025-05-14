using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardDisplay : MonoBehaviour
{
    [SerializeField] private Transform cardParent;
    [SerializeField] private ClassDataSO selectedClass;

    // Dictionary to hold cards and their counts
    private Dictionary<CardDataSO, int> cardDictionary = new Dictionary<CardDataSO, int>();

    public void UpdateSelectedClass(ClassDataSO _class)
    {
        selectedClass = _class;

        // Clear previous cards and dictionary
        foreach (Transform child in cardParent)
        {
            Destroy(child.gameObject);
        }
        cardDictionary.Clear();

        // Populate dictionary with card counts
        foreach (CardDataSO card in selectedClass.startingDeck)
        {
            if (cardDictionary.ContainsKey(card))
                cardDictionary[card]++;
            else
                cardDictionary[card] = 1;
        }

        GenerateCards();
    }
    private void GenerateCards()
    {
        foreach (var entry in cardDictionary)
        {
            CardDataSO cardData = entry.Key;
            int count = entry.Value;

            Card card = new Card(cardData, null);
            InventoryCardInformation cardInfo = CardSpriteGenerator.Instance.GenerateCardSprite(card, cardParent, count);
            cardInfo.name = $"Card_{card.cardName} (×{count})";
        }
    }
}
