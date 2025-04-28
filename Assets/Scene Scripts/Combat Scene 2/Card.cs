using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card
{
    public string cardName => data.cardName;

    public string description => data.description;

    public Target target => data.target;

    public Sprite sprite => data.cardSprite;

    public int mana {  get; private set; }
    public int power {  get; private set; }

    private readonly CardDataSO data;

    public Card(CardDataSO cardData)
    {
        data = cardData;
        mana = cardData.cost;
        power = cardData.power;
    }
}
