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

    [SerializeField] private CardSprite hoverSprite;

    public void Show(Card card, Vector3 position)
    {
        hoverSprite.gameObject.SetActive(true);
        hoverSprite.Setup(card);
        hoverSprite.transform.position = position;
    }

    public void Hide()
    {
        hoverSprite.gameObject?.SetActive(false);
        hoverSprite.transform.position = Vector3.zero;
    }
}
