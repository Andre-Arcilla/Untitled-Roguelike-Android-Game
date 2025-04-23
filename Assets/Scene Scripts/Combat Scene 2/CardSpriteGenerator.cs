using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

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

    [SerializeField] private CardSprite cardPrefab;

    public CardSprite GenerateCardSprite(Card card, Vector3 position, Quaternion rotation, Transform parent)
    {
        CardSprite cardSprite = Instantiate(cardPrefab, position, rotation, parent);
        cardSprite.transform.localScale = Vector3.zero;
        cardSprite.Setup(card);
        return cardSprite;
    }
}
