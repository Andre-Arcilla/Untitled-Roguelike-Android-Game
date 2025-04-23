using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CardSpriteHover : MonoBehaviour
{
    public static CardSpriteHover Instance { get; private set; }

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

    [SerializeField] private CardSprite selectedCard;
    [SerializeField] private bool dragging;

    public void Show(Card card)
    {
        if (dragging == false)
        {
            selectedCard.gameObject.SetActive(true);
            selectedCard.Setup(card);
        }
    }

    public void Hide()
    {
        if (dragging == false)
        {
            selectedCard.gameObject?.SetActive(false);
        }
    }

    public void Drag(bool isDragging)
    {
        dragging = isDragging;
    }
}
