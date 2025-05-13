using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class CardSpriteGenerator : MonoBehaviour
{
    public static CardSpriteGenerator Instance { get; private set; }

    private void Awake()
    {
        // Check if an instance already exists
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // Only one allowed
            return;
        }

        Instance = this;
    }

    [SerializeField] private CardInformation cardPrefab;
    [SerializeField] private InventoryCardInformation invCardPrefab;

    public CardInformation GenerateCardSprite(Card card, Vector3 position, Quaternion rotation, Transform parent)
    {
        CardInformation cardSprite = Instantiate(cardPrefab, position, rotation, parent);
        cardSprite.transform.localScale = Vector3.zero;
        cardSprite.Setup(card);
        return cardSprite;
    }

    public InventoryCardInformation GenerateCardSprite(Card card, Transform parent, int amount)
    {
        InventoryCardInformation cardSprite = Instantiate(invCardPrefab, parent);
        cardSprite.Setup(card, amount);
        return cardSprite;
    }
}
