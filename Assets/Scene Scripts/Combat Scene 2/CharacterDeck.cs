using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.Splines.Examples;
using UnityEngine;

public class CharacterDeck : MonoBehaviour
{
    [SerializeField] private List<CardDataSO> deckList;
    [SerializeField] public List<GameObject> deck;
    [SerializeField] public List<GameObject> hand;
    [SerializeField] public List<GameObject> discard;
    [SerializeField] private CardHolder cardHolder;
    [SerializeField] private GameObject handParent; // Hand Parent for drawn cards
    [SerializeField] private GameObject deckParent; // Deck Parent for all cards
    [SerializeField] private GameObject discardParent; // Discard Parent for discarded cards
    [SerializeField] private int handSize; // Max size of hand
    [SerializeField] private Transform discardPos; // Discard position for animation
    [SerializeField] private Transform deckPos; // Deck position for animation

    // Set the deck with shuffled card data
    public void SetDeck(List<CardDataSO> _deck)
    {
        transform.Find("Card View").gameObject.SetActive(false);

        deckList = new List<CardDataSO>(_deck); // Assign the provided deck data
        deck.Clear(); // Clear the deck list to prepare for new cards
        GenerateCards(); // Generate new cards from the deck data
        DrawAction();
    }

    public void ShuffleCardsToDeck()
    {
        Shuffle(deck);

        // Move all cards to the discard parent first
        foreach (GameObject card in deck)
        {
            card.transform.SetParent(discardParent.transform, false); // Move the cards to the discard parent
        }

        // Now move all cards back to the deck parent
        foreach (GameObject card in deck)
        {
            card.transform.SetParent(deckParent.transform, false); // Move the cards back to the deck parent
        }
    }

    // Generate cards and instantiate them in the deck
    public void GenerateCards()
    {
        StartCoroutine(GenerateCardsCoroutine());
    }

    // Coroutine to generate the cards and instantiate them
    private IEnumerator GenerateCardsCoroutine()
    {
        for (int i = 0; i < deckList.Count; i++)
        {
            CardDataSO cardData = deckList[i]; // Get the card data at the current index
            Card card = new Card(cardData); // Create a Card object from the card data
            CardInformation cardInfo = CardSpriteGenerator.Instance.GenerateCardSprite(card, deckPos.position, Quaternion.identity, deckParent.transform);

            // Set the card's name based on its index in the original deck (deckList)
            cardInfo.name = "Card_" + i.ToString() + " (" + card.cardName.ToString() + ")"; // Name format: "Card_0", "Card_1", ...
            deck.Add(cardInfo.gameObject); // Add the new card GameObject to the deck list
        }
        ShuffleCardsToDeck();
        yield return null;
    }

    public void DrawAction()
    {
        if (hand.Count == handSize)
        {
            return;
        }

        List<CardInformation> newlyDrawnSprites = new();

        while (hand.Count < handSize)
        {
            // Reshuffle if deck is empty
            if (deck.Count == 0 && discard.Count > 0)
            {
                Shuffle(discard);

                foreach (GameObject card in new List<GameObject>(discard))
                {
                    deck.Add(card);
                    card.transform.SetParent(deckParent.transform, false);
                }

                discard.Clear();
                Debug.Log("Deck reshuffled from discard");
            }

            if (deck.Count == 0)
            {
                Debug.LogWarning("No more cards to draw.");
                break;
            }

            // Draw card from deck
            GameObject drawnCard = deck[0];
            deck.RemoveAt(0);
            hand.Add(drawnCard);
            drawnCard.transform.SetParent(handParent.transform, false);

            // Get the card sprite and queue it for visual addition
            CardInformation cardSprite = drawnCard.GetComponent<CardInformation>();
            if (cardSprite == null)
            {
                Debug.LogError("CardSprite is NULL on: " + drawnCard.name);
            }
            newlyDrawnSprites.Add(cardSprite);
        }

        StartCoroutine(cardHolder.AddCards(deckPos, newlyDrawnSprites));
    }

    public void DiscardAction()
    {
        if (hand.Count == 0) return;

        // Prepare card sprites for animation
        List<CardInformation> cardSprites = new List<CardInformation>();
        List<GameObject> cardsToDiscard = new List<GameObject>(hand); // Create a copy

        foreach (GameObject cardObj in cardsToDiscard)
        {
            if (cardObj.TryGetComponent(out CardInformation sprite))
            {
                cardSprites.Add(sprite);
            }
        }

        // Start animation coroutine
        StartCoroutine(RunDiscardProcess(cardSprites, cardsToDiscard));
    }

    private IEnumerator RunDiscardProcess(List<CardInformation> cardSprites, List<GameObject> cardsToDiscard)
    {
        // Start discard animation
        yield return cardHolder.DiscardCardsToPile(discardPos);

        // After animation completes, update parent and lists
        foreach (GameObject card in cardsToDiscard)
        {
            card.transform.SetParent(discardParent.transform, false);
            discard.Add(card);
            hand.Remove(card); // Safely remove from hand
        }
    }

    public void DrawCards(int amount)
    {

    }

    void Shuffle<T>(List<T> list)
    {
        System.Random random = new System.Random();

        int n = list.Count;
        while (n > 1)
        {
            int k = random.Next(0, n);
            n--;
            T temp = list[k];
            list[k] = list[n];
            list[n] = temp;
        }
    }
}
