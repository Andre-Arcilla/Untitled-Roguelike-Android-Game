using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.Splines.Examples;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public class CharacterDeck : MonoBehaviour
{
    [SerializeField] private List<CardDataSO> deckList;
    [SerializeField] public List<GameObject> deck;
    [SerializeField] public List<GameObject> hand;
    [SerializeField] public List<GameObject> discard;
    [SerializeField] public List<GameObject> playing;
    [SerializeField] private CardHolder cardHolder;
    [SerializeField] private GameObject deckParent; // Deck Parent for all cards
    [SerializeField] private GameObject handParent; // Hand Parent for drawn cards
    [SerializeField] private GameObject discardParent; // Discard Parent for discarded cards
    [SerializeField] private GameObject playParent; // play Parent for cards in play
    [SerializeField] private int handSize; // Size of hand
    [SerializeField] private int maxHandSize; // Max size of hand
    [SerializeField] public Transform discardPos; // Discard position for animation
    [SerializeField] public Transform deckPos; // Deck position for animation

    // Set the deck with shuffled card data
    public void SetDeck(List<CardDataSO> _deck)
    {
        transform.Find("Card View").gameObject.SetActive(false);

        deckList = new List<CardDataSO>(_deck); // Assign the provided deck data
        deck.Clear(); // Clear the deck list to prepare for new cards
        GenerateCards(); // Generate new cards from the deck data
        StartCoroutine(DrawHandAction());
    }

    private void ShuffleCardsToDeck()
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
    private void GenerateCards()
    {
        StartCoroutine(GenerateCardsCoroutine());
    }

    // Coroutine to generate the cards and instantiate them
    private IEnumerator GenerateCardsCoroutine()
    {
        for (int i = 0; i < deckList.Count; i++)
        {
            CharacterInfo owner = gameObject.GetComponent<CharacterInfo>();
            CardDataSO cardData = deckList[i]; // Get the card data at the current index
            Card card = new Card(cardData, owner); // Create a Card object from the card data
            CardInformation cardInfo = CardSpriteGenerator.Instance.GenerateCardSprite(card, deckPos.position, Quaternion.identity, deckParent.transform);

            // Set the card's name based on its index in the original deck (deckList)
            cardInfo.name = "Card_" + i.ToString() + " (" + card.cardName.ToString() + ")"; // Name format: "Card_0 (slash)", "Card_1 (arrow)", ...
            deck.Add(cardInfo.gameObject); // Add the new card GameObject to the deck list
        }
        ShuffleCardsToDeck();
        yield return null;
    }

    //coroutine method to wait for discard before draw
    public IEnumerator DiscardDrawCoroutine()
    {
        yield return DiscardHandAction();
        yield return DrawHandAction();
    }

    //draw cards then animate
    private IEnumerator DrawHandAction()
    {
        if (hand.Count == handSize)
        {
            yield return null;
        }

        while (hand.Count < handSize)
        {
            // Reshuffle if deck is empty
            if (deck.Count == 0 && discard.Count > 0)
            {
                Shuffle(discard);

                foreach (GameObject card in new List<GameObject>(discard))
                {
                    CardInformation cardInfo = card.GetComponent<CardInformation>();

                    cardInfo.isSelected = false;
                    cardInfo.isDragging = false;
                    cardInfo.isDeselecting = false;
                    cardInfo.isUsing = false;

                    deck.Add(card);
                    card.transform.SetParent(deckParent.transform, false);
                }

                discard.Clear();
            }

            if (deck.Count == 0)
            {
                break;
            }

            GameObject currentCard = deck[0];

            if (hand.Count < maxHandSize)
            {
                // Draw card from deck to hand
                deck.Remove(currentCard);
                hand.Add(currentCard);
                currentCard.transform.SetParent(handParent.transform, false);
            }
            else if (hand.Count >= maxHandSize)
            {
                // Draw card from deck to discard
                deck.Remove(currentCard);
                discard.Add(currentCard);
                currentCard.transform.SetParent(discardParent.transform, false);
            }
        }

        yield return cardHolder.DrawHandAnimation();
    }

    //animate then discard cards
    private IEnumerator DiscardHandAction()
    {
        yield return cardHolder.DiscardHandAnimation();

        while (hand.Count > 0)
        {
            GameObject currentCard = hand[0];

            hand.Remove(currentCard);
            discard.Add(currentCard);
            currentCard.transform.SetParent(discardParent.transform, false);
        }
    }

    //draw card effect method to draw x amount of cards
    public void DrawCard(int amount, CardInformation card)
    {
        StartCoroutine(DrawCardCoroutine(amount, card));
    }

    //draw card effect draw coroutine
    private IEnumerator DrawCardCoroutine(int amount, CardInformation card)
    {
        foreach (GameObject cardObj in hand)
        {
            cardObj.GetComponent<Collider2D>().enabled = false;
        }

        yield return StartPlayCard(card);

        yield return new WaitForSeconds(0.2f);

        yield return EndPlayCard(card);

        yield return EndPlaySortHand();

        for (int i = 0; i < amount; i++)
        {
            // Reshuffle if deck is empty
            if (deck.Count == 0 && discard.Count > 0)
            {
                Shuffle(discard);

                foreach (GameObject cardObj in new List<GameObject>(discard))
                {
                    deck.Add(cardObj);
                    cardObj.transform.SetParent(deckParent.transform, false);
                }

                discard.Clear();
                Debug.Log("Deck reshuffled from discard");
            }

            if (deck.Count == 0)
            {
                Debug.LogWarning("No more cards to draw.");
                break;
            }

            GameObject drawnCard = deck[0];
            drawnCard.GetComponent<Collider2D>().enabled = false;

            if (hand.Count < maxHandSize)
            {
                // Draw card from deck to hand
                deck.RemoveAt(0);
                hand.Add(drawnCard);
                drawnCard.transform.SetParent(playParent.transform, false);
                yield return cardHolder.DrawCardAnimation(drawnCard.GetComponent<CardInformation>());
            }
            else if (hand.Count >= maxHandSize)
            {
                // Draw card from deck to discard
                deck.RemoveAt(0);
                discard.Add(drawnCard);
                drawnCard.transform.SetParent(playParent.transform, false);
                yield return cardHolder.DrawCardAnimation(drawnCard.GetComponent<CardInformation>(), discardParent);
            }
        }

        foreach (GameObject cardObj in hand)
        {
            cardObj.GetComponent<Collider2D>().enabled = true;
        }
    }

    //method to add card to playing field
    public IEnumerator StartPlayCard(CardInformation card)
    {
        //prevent card interaction on play start
        int layer = LayerMask.NameToLayer("Ignore Raycast");
        foreach (Transform t in transform.parent.GetComponentsInChildren<Transform>(true))
        {
            t.gameObject.layer = layer;
        }

        Vector3 dropZone = new Vector3(0, 1, card.transform.position.z);
        Targetable cardOwner = GetComponentInParent<Targetable>();
        card.transform.Find("Card Front").gameObject.SetActive(true);
        card.transform.Find("Card Back").gameObject.SetActive(false);

        if (cardOwner.team == Team.Player)
        {
            var sequence = DOTween.Sequence();
            sequence.Append(card.transform.DOMove(dropZone, 0.25f));
            sequence.Join(card.transform.DOLocalRotate(Vector2.zero, 0.25f));
            sequence.Join(card.transform.DOScale(1f, 0.25f));
            sequence.SetLink(gameObject).SetAutoKill(true);

            yield return sequence.WaitForCompletion();
        }

        hand.Remove(card.gameObject);
        playing.Add(card.gameObject);

        card.gameObject.transform.SetParent(playParent.transform, false);

        if (cardOwner.team == Team.Enemy)
        {
            card.transform.position = cardOwner.gameObject.transform.Find("Character Sprite").gameObject.transform.position;
            card.transform.localScale = Vector3.zero;

            var sequence = DOTween.Sequence();
            sequence.Append(card.transform.DOScale(0.3f, 0.25f));
            sequence.Append(card.transform.DOMove(dropZone, 0.35f));
            sequence.Join(card.transform.DOLocalRotate(Vector2.zero, 0.35f));
            sequence.Join(card.transform.DOScale(1.2f, 0.35f));
            sequence.SetLink(gameObject).SetAutoKill(true);

            yield return sequence.WaitForCompletion();
        }

        card.transform.position = dropZone;
    }

    //method to remove card from playing field
    public IEnumerator EndPlayCard(CardInformation card)
    {
        Vector3 dropZone = new Vector3(discardPos.position.x, discardPos.position.y, card.transform.position.z);
        Targetable cardOwner = GetComponentInParent<Targetable>();

        if (cardOwner.team == Team.Player)
        {
            var sequence = DOTween.Sequence();
            sequence.Append(card.transform.DOMove(dropZone, 0.25f));
            sequence.Join(card.transform.DOScale(1, 0.25f));
            sequence.Join(card.transform.DOLocalRotateQuaternion(Quaternion.identity, 0.25f));
            sequence.Append(card.transform.DOLocalRotateQuaternion(Quaternion.Euler(0f, 90f, 0f), 0.25f).SetEase(Ease.InOutQuad));
            sequence.AppendCallback(() =>
            {
                card.transform.Find("Card Front").gameObject.SetActive(false);
                card.transform.Find("Card Back").gameObject.SetActive(true);
            });
            sequence.Append(card.transform.DOLocalRotateQuaternion(Quaternion.Euler(0f, 0f, 0f), 0.15f).SetEase(Ease.InOutQuad));
            sequence.SetLink(gameObject).SetAutoKill(true);

            yield return sequence.WaitForCompletion();
        }

        if (cardOwner.team == Team.Enemy)
        {
            dropZone = cardOwner.gameObject.transform.Find("Character Sprite").gameObject.transform.position;

            var sequence = DOTween.Sequence();
            sequence.Append(card.transform.DOMove(dropZone, 0.35f));
            sequence.Join(card.transform.DOLocalRotate(Vector2.zero, 0.35f));
            sequence.Join(card.transform.DOScale(0.3f, 0.35f));
            sequence.Append(card.transform.DOScale(Vector3.zero, 0.25f));
            sequence.SetLink(gameObject).SetAutoKill(true);

            yield return sequence.WaitForCompletion();
        }

        playing.Remove(card.gameObject);
        discard.Add(card.gameObject);

        card.gameObject.transform.SetParent(discardParent.transform, false);
        card.isDragging = false;
        card.isUsing = false;

        //returns card interaction on play end
        int layer = LayerMask.NameToLayer("Default");
        foreach (Transform t in transform.parent.GetComponentsInChildren<Transform>(true))
        {
            t.gameObject.layer = layer;
        }
    }

    public IEnumerator EndPlaySortHand()
    {
        yield return cardHolder.SortCards();
    }

    public IEnumerator UpdateSelectedCardPos()
    {
        yield return null;
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
