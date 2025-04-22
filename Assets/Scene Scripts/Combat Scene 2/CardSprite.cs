using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CardSprite : MonoBehaviour
{
    [SerializeField] private string cardName;
    [SerializeField] private TMP_Text cardMana;
    [SerializeField] private SpriteRenderer sprite;
    [SerializeField] private GameObject wrapper;

    private Vector3 offset; // To store the offset for dragging
    private Vector3 originalPosition; // To store the original position when dragging starts

    public Card card {  get; private set; }

    public void Setup(Card card)
    {
        this.card = card;
        cardName = card.cardName;
        cardMana.text = card.mana.ToString();
        sprite.sprite = card.sprite;
    }
    private void OnMouseDown()
    {
        // Hide the wrapper when the card is clicked and show the hover preview
        wrapper.SetActive(false);
        Vector3 pos = new(transform.position.x, transform.position.y + 1, transform.position.z);

        CardSpriteHover.Instance.Show(card, pos);

        // Store the original position and calculate the offset for dragging
        originalPosition = transform.position;
        offset = transform.position - Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }

    private void OnMouseDrag()
    {
        // Follow the cursor while dragging
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition) + offset;
        mousePosition.z = originalPosition.z; // Keep the z position fixed
        transform.position = mousePosition;

        // Update the hover preview position so it follows the card
        Vector3 hoverPos = new(transform.position.x, transform.position.y + 1, transform.position.z);

        CardSpriteHover.Instance.Show(card, hoverPos);
    }

    private void OnMouseUp()
    {
        // Restore the wrapper and hide the hover preview when the drag ends
        wrapper.SetActive(true);
        CardSpriteHover.Instance.Hide();

        // Return the card to its original position
        transform.position = originalPosition;
    }
}
