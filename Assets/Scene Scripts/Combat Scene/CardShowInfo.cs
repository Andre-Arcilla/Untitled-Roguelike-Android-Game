using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CardShowInfo : MonoBehaviour
{
    public static CardShowInfo Instance { get; private set; }

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

    [SerializeField] private CardInformation cardPreview;
    [SerializeField] private bool dragging;
    [HideInInspector] public CardInformation _cardPreview => cardPreview;

    public void Show(Card card)
    {
        if (dragging == false)
        {
            cardPreview.gameObject.SetActive(true);
            cardPreview.Setup(card);
        }
    }

    public void Hide()
    {
        if (!dragging)
        {
            if (cardPreview != null)
            {
                cardPreview.transform.position = new Vector3(-7f, cardPreview.transform.position.y, cardPreview.transform.position.z);
                cardPreview.gameObject.SetActive(false);
            }
        }
    }

    public void Drag(bool isDragging)
    {
        dragging = isDragging;
    }
}
