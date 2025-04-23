using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;

public class CardSprite : MonoBehaviour
{
    [Header("Card Display")]
    [SerializeField] private string cardName; //removeable
    [SerializeField] private TMP_Text cardMana;
    [SerializeField] private SpriteRenderer sprite;
    [SerializeField] private int HP; //removeable

    //holds the data of this instance of the card
    public Card card {  get; private set; }

    public void Setup(Card card)
    {
        this.card = card;
        cardName = card.cardName;
        cardMana.text = card.mana.ToString();
        sprite.sprite = card.sprite;
        CharacterInfo characterInfo = GetComponentInParent<CharacterInfo>();
        //HP = characterInfo.stats.totalHP;
    }

    private void OnMouseEnter()
    {
        CardSpriteHover.Instance.Show(card);
    }

    private void OnMouseExit()
    {
        CardSpriteHover.Instance.Hide();
    }

    //drag and drop ===============================================================================================
    [Header("Card Position")]
    private Vector3 originalPosition; // Store the original position
    private Quaternion originalRotation; // Store the original rotation
    private Vector3 originalScale; // Store the original rotation
    [SerializeField] private float returnAnimationDelay = 0.15f;

    void OnMouseDown()
    {
        //stops the cursor from changing the hovered card 
        CardSpriteHover.Instance.Drag(true);

        // Store the original position and rotation
        originalPosition = transform.position;
        originalRotation = transform.rotation;
        originalScale = transform.localScale;
        GetComponent<SortingGroup>().sortingOrder = 1;
    }

    void OnMouseDrag()
    {
        // Update the object's position as the mouse is dragged
        Vector3 mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 mousePosition = new Vector3(mouse.x, mouse.y, transform.position.z);
        transform.DOMove(mousePosition, 0.15f);
        transform.DORotateQuaternion(Quaternion.identity, 0.15f);
        transform.DOScale(Vector3.one * 0.5f, 0.15f);
    }

    void OnMouseUp()
    {
        CardSpriteDrag.Instance.CheckIfInDropZone(this);

        //stops the cursor from changing the hovered card 
        CardSpriteHover.Instance.Drag(false);

        //disable the collider to avoid catching the card mid animation
        BoxCollider2D collider = GetComponent<BoxCollider2D>();
        collider.enabled = false;

        // Return the object to its original position and rotation
        var seq = DOTween.Sequence();
        seq.Append(transform.DOMove(originalPosition, returnAnimationDelay));
        seq.Join(transform.DORotateQuaternion(originalRotation, returnAnimationDelay));
        seq.Join(transform.DOScale(originalScale, returnAnimationDelay));
        seq.OnComplete(() => collider.enabled = true);

        GetComponent<SortingGroup>().sortingOrder = 0;
    }

    //stats and actions ===========================================================================================
}
