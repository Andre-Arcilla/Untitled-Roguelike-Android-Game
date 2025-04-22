using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.Splines.Examples;
using UnityEngine;

public class CharacterDeck : MonoBehaviour
{
    [SerializeField] private List<CardDataSO> deck; //remove serializefield
    [SerializeField] private List<CardDataSO> hand; //remove serializefield
    [SerializeField] private List<CardDataSO> discard; //remove serializefield
    [SerializeField] private CardHolder cardHolder;
    [SerializeField] private GameObject cardParent;
    [SerializeField] private int handSize;
    [SerializeField] private Transform discardPos;
    [SerializeField] private Transform deckPos;


    public void SetDeck(List<CardDataSO> _deck)
    {
        deck = new List<CardDataSO>(_deck);
        Shuffle(deck);
        DrawAction();
    }

    public void GenerateCards()
    {
        StartCoroutine(GenerateCardsCoroutine());
    }

    private IEnumerator GenerateCardsCoroutine()
    {
        foreach (CardDataSO cardData in hand)
        {
            Card card = new Card(cardData);
            CardSprite cardSprite = CardSpriteGenerator.Instance.GenerateCardSprite(card, deckPos.position, Quaternion.identity, cardParent.transform);
            yield return cardHolder.AddCard(cardSprite); // Wait if AddCard is a coroutine
        }
    }

    public void DrawAction()
    {
        if (hand.Count == handSize)
        {
            Debug.Log("Hand is full, not drawing cards");
            return;
        }

        if (deck.Count < handSize)
        {
            Debug.Log("insufficient cards, shuffling discard back to deck");
            Shuffle(discard);
            
            foreach (CardDataSO card in discard)
            {
                deck.Add(card);
            }

            discard.Clear();

            Debug.Log("deck replenished");
        }

        for (int i = 0; i < handSize; i++)
        {
            hand.Add(deck[0]);
            deck.RemoveAt(0);
        }

        GenerateCards();
        Debug.Log("cards drawn");
    }

    public void DiscardAction()
    {

        StartCoroutine(cardHolder.DiscardCardsToPile(discardPos));

        // Add card data to discard pile
        foreach (var card in hand)
        {
            discard.Add(card);
        }

        hand.Clear();

        Debug.Log("cards discarded");
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
