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
    [SerializeField] public bool isSelected = false;
    [SerializeField] public bool isDragging = false;
    [SerializeField] public bool isDeselecting = false;
    [SerializeField] public bool isUsing = false;
    private Vector3 originalPosition;
    private Quaternion originalRotation;
    private Vector3 originalScale;

    public void NewPos(float newPos)
    {
        originalPosition.y += newPos;
    }

    void OnMouseDown()
    {
        if (card.target == Target.Card)
        {
            CardHolder holder = transform.parent.parent.GetComponent<CardHolder>();
            holder.StartCoroutine(holder.FanCardsAction(this));
        }

        // Store the original position and rotation
        originalPosition = transform.position;
        originalRotation = transform.rotation;
        originalScale = transform.localScale;

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

        //put valid targets in front of a panel
        GetComponent<SortingGroup>().sortingOrder = 3;
        TargetingSystem.Instance.darkPanel.SetActive(true);
        Targetable sender = GetComponentInParent<Targetable>();
        List<GameObject> potentialTargets = TargetSelector.Instance.GetTargets(this, sender);
        foreach (GameObject target in potentialTargets)
        {
            target.GetComponentInChildren<SortingGroup>().sortingOrder = 2;
            if (target.name == TargetingSystem.Instance.center.name)
            {
                target.GetComponent<SpriteRenderer>().enabled = true;
            }
        }


        // Update the object's position as the mouse is dragged
        Vector3 mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 mousePosition = new Vector3(mouse.x, mouse.y, transform.position.z);

        var sequence = DOTween.Sequence();
        sequence.Append(transform.DOMove(mousePosition, 0.15f));
        sequence.Join(transform.DORotateQuaternion(Quaternion.identity, 0.15f));
        sequence.Join(transform.DOScale(Vector3.one * 0.5f, 0.15f));
        sequence.SetLink(gameObject).SetAutoKill(true);
    }

    void OnMouseUp()
    {
        //return valid targets to original state
        bool hasEnoughMana = GetComponentInParent<CharacterInfo>().currentEN >= card.mana;
        bool hasValidTarget = TargetingSystem.Instance.TryGetValidTarget(transform.position, this, out GameObject t);
        bool willPlay = !isDeselecting && !isSelected && hasEnoughMana && hasValidTarget;

        CardHolder holder = transform.parent.parent.GetComponent<CardHolder>();

        if (card.target == Target.Card)
        {
            if (willPlay)
            {
                holder.StartCoroutine(holder.SortCards(this));
            }
            else
            {
                holder.StartCoroutine(holder.SortCards());
            }
        }

        GetComponent<SortingGroup>().sortingOrder = 0;
        TargetingSystem.Instance.darkPanel.SetActive(false);
        Targetable sender = GetComponentInParent<Targetable>();
        List<GameObject> potentialTargets = TargetSelector.Instance.GetTargets(this, sender);
        foreach (GameObject target in potentialTargets)
        {
            target.GetComponentInChildren<SortingGroup>().sortingOrder = 0;
            if (target.name == TargetingSystem.Instance.center.name)
            {
                target.GetComponent<SpriteRenderer>().enabled = false;
            }
        }

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
        var sequence = DOTween.Sequence();
        sequence.Append(transform.DOMove(originalPosition, 0.15f));
        sequence.Join(transform.DORotateQuaternion(originalRotation, 0.15f));
        sequence.Join(transform.DOScale(originalScale, 0.15f));
        sequence.SetLink(gameObject).SetAutoKill(true);
        sequence.OnComplete(() => collider.enabled = true);
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
