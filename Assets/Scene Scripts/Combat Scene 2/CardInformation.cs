using DG.Tweening;
using SerializeReferenceEditor;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;

public class CardInformation : MonoBehaviour
{
    [Header("Card Display")] //temp
    [SerializeField] private string cardName; //temp
    [SerializeField] private int mana; //temp
    [SerializeField] private int power; //temp
    [SerializeReference, SR] private List<ICardEffect> effects = new List<ICardEffect>(); //temp

    [Header("References")]
    [SerializeField] private TMP_Text cardManaTxt;
    [SerializeField] private TMP_Text cardNameTxt;
    [SerializeField] private TMP_Text cardDescTxt;
    [SerializeField] private SpriteRenderer sprite;

    //holds the data of this instance of the card
    public Card card {  get; private set; }

    public void Setup(Card card)
    {
        this.card = card;
        cardName = card.cardName;
        mana = card.mana;
        power = card.power;
        effects.Clear();
        effects.AddRange(card.effects);

        cardManaTxt.text = card.mana.ToString();
        cardNameTxt.text = card.cardName.ToString();
        cardDescTxt.text = card.description.Replace("X", card.power.ToString());
        ReplaceSprite(sprite, card.sprite);
    }

    public void UpdateCard()
    {
        cardName = card.cardName;
        mana = card.mana;
        power = card.power;
        effects.Clear();
        effects.AddRange(card.effects);

        cardManaTxt.text = card.mana.ToString();
        cardNameTxt.text = card.cardName.ToString();
        cardDescTxt.text = card.description.Replace("X", card.power.ToString());

    }

    void ReplaceSprite(SpriteRenderer renderer, Sprite newSprite)
    {
        Sprite oldSprite = renderer.sprite;

        if (oldSprite == null || newSprite == null)
            return;

        Vector2 oldSize = oldSprite.bounds.size;
        Vector2 newSize = newSprite.bounds.size;

        // Calculate scale ratios for both axes
        float xRatio = oldSize.x / newSize.x;
        float yRatio = oldSize.y / newSize.y;

        // Choose the smaller ratio to ensure the new sprite fits within old bounds
        float scaleRatio = Mathf.Min(xRatio, yRatio);

        // Apply uniform scale
        renderer.transform.localScale *= scaleRatio;

        // Assign new sprite
        renderer.sprite = newSprite;
    }

    //drag and drop ===============================================================================================
    [Header("Card Position")]
    [SerializeField] private float returnAnimationDelay = 0.15f;
    [SerializeField] public bool isSelected = false;
    [SerializeField] public bool isDragging = false;
    [SerializeField] public bool isDeselecting = false;
    [SerializeField] public bool isUsing = false;
    [SerializeField] private Vector3 originalPosition; // Store the original position
    private Quaternion originalRotation; // Store the original rotation
    private Vector3 originalScale; // Store the original rotation

    public void NewPos(float newPos)
    {
        originalPosition.y += newPos;
    }

    void OnMouseDown()
    {
        // Store the original position and rotation
        originalPosition = transform.position;
        originalRotation = transform.rotation;
        originalScale = transform.localScale;
        GetComponent<SortingGroup>().sortingOrder = 1;

        if (isSelected == true)
        {
            TargetingSystem.Instance.DeselectCard(this);
            isDeselecting = true;
            return;
        }

        isDragging = true;

        //stops the cursor from changing the hovered card 
        CardShowInfo.Instance.Drag(true);
    }

    void OnMouseDrag()
    {
        if (isDragging == false)
        {
            return;
        }

        if (isSelected == true)
        {
            return;
        }

        // Update the object's position as the mouse is dragged
        Vector3 mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 mousePosition = new Vector3(mouse.x, mouse.y, transform.position.z);
        transform.DOMove(mousePosition, 0.15f);
        transform.DORotateQuaternion(Quaternion.identity, 0.15f);
        transform.DOScale(Vector3.one * 0.5f, 0.15f);
    }

    void OnMouseUp()
    {
        if (isSelected == true)
        {
            return;
        }

        if (isDeselecting != true)
        {
            TargetingSystem.Instance.AttemptPlayCard(this, transform.position);
        }

        if (isUsing == true)
        {
            CardShowInfo.Instance.Drag(false);
            return;
        }


        isDragging = false;
        isDeselecting = false;

        //stops the cursor from changing the hovered card 
        CardShowInfo.Instance.Drag(false);

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

    private void OnMouseEnter()
    {
        CardShowInfo.Instance.Show(card);
    }

    private void OnMouseExit()
    {
        CardShowInfo.Instance.Hide();
    }

    //stats and actions ===========================================================================================
}
