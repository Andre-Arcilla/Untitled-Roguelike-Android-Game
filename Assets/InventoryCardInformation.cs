using SerializeReferenceEditor;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryCardInformation : MonoBehaviour
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
    [SerializeField] private TMP_Text cardAmountTxt;
    [SerializeField] private Image image;

    //holds the data of this instance of the card
    public Card card { get; private set; }

    public void Setup(Card card, int amount)
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
        cardAmountTxt.text = "×" + amount.ToString();
        ReplaceSprite(image, card.sprite);
    }

    void ReplaceSprite(Image renderer, Sprite newSprite)
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
}
