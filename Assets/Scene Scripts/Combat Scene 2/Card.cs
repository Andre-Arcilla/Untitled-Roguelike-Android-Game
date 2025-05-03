using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card
{
    public string cardName => data.cardName;

    public string description => data.description;

    public Target target => data.target;

    public Sprite sprite => data.cardSprite;
    public List<ICardEffect> effects => data.effects;

    public int mana {  get; private set; }
    public int power {  get; private set; }

    private readonly CardDataSO data;

    public Card(CardDataSO cardData)
    {
        data = cardData;
        mana = cardData.cost;
        power = cardData.power;
    }

    public void ChangeMana(int amount, Change change)
    {
        switch (change)
        {
            case Change.Increase:
                mana += amount;
                break;
            case Change.Decrease:
                mana = Mathf.Max(1, mana - amount);
                break;
            default:
                break;
        }
    }

    public void ChangePower(int amount, Change change)
    {
        switch (change)
        {
            case Change.Increase:
                power += amount;
                break;
            case Change.Decrease:
                power = Mathf.Max(1, power - amount);
                break;
            default:
                break;
        }
    }
}

public enum Change
{
    Increase,
    Decrease
}
