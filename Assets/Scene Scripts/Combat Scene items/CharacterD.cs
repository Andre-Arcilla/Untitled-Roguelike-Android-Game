using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterD : MonoBehaviour
{
    public Sprite[] CardFaces;
    public GameObject prefab;
    public GameObject CardHolder;

    public string[] cards = new string[] { "Slash", "Flash", "Wave", "Arrow", "Slash", "Flash", "Wave", "Arrow" };
    public List<string> deck;
    private Dictionary<string, Sprite> cardSprites;

    void InitializeCardSprites()
    {
        cardSprites = new Dictionary<string, Sprite>
        {
            { "Slash", CardFaces[0] },
            { "Flash", CardFaces[1] },
            { "Wave", CardFaces[2] },
            { "Arrow", CardFaces[3] }
        };
    }


    // Start is called before the first frame update
    void Start()
    {
        InitializeCardSprites();
        StartDeck();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartDeck()
    {
        deck = GenerateDeck();
        Shuffle(deck);
        StartCoroutine(GenerateCardFaces());



        print("Deck Size: " + deck.Count);

        //testing purposes only
        foreach (string card in deck)
        {
            print(card);
        }
    }

    public List<string> GenerateDeck()
    {
        List<string> newDeck = new List<string>();
        foreach (string card in cards)
        {
            newDeck.Add(card);
        }

        return newDeck;
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

    IEnumerator GenerateCardFaces()
    {
        //starting Z position of cards
        float zOffset = -1f;

        foreach (string card in deck)
        {
            yield return new WaitForSeconds(0.05f);
            GameObject newCard = Instantiate(prefab);
            newCard.transform.SetParent(CardHolder.transform, false);

            //change the Z position of each cards so that they appear to be stacked
            newCard.transform.localPosition = new Vector3(0f, 0f, zOffset);

            newCard.name = card;

            // Find SpriteRenderer
            SpriteRenderer spriteRenderer = newCard.GetComponentInChildren<SpriteRenderer>();
            if (spriteRenderer == null)
            {
                Debug.LogError("SpriteRenderer not found on " + newCard.name);
                continue;
            }

            // Check if the dictionary has the key
            if (cardSprites.ContainsKey(card))
            {
                spriteRenderer.sprite = cardSprites[card];
            }
            else
            {
                Debug.LogError("Card not found in dictionary: " + card);
            }

            zOffset -= 0.1f;
        }
    }
}
