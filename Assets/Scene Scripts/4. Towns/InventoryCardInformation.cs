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
        if (renderer == null || newSprite == null)
            return;

        RectTransform rectTransform = renderer.rectTransform;

        // Get the current UI size (in pixels)
        Vector2 targetSize = rectTransform.sizeDelta;

        // Get the new sprite's pixel size
        Vector2 newSpriteSize = newSprite.rect.size;

        // Compute aspect-fit scaling ratio
        float xRatio = targetSize.x / newSpriteSize.x;
        float yRatio = targetSize.y / newSpriteSize.y;
        float scaleRatio = Mathf.Min(xRatio, yRatio);

        // Apply scaled size while preserving aspect ratio
        Vector2 adjustedSize = newSpriteSize * scaleRatio;
        rectTransform.sizeDelta = adjustedSize;

        // Assign the new sprite
        renderer.sprite = newSprite;
    }

}
