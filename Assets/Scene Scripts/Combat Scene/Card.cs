using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Card
{
    public string cardName => data.cardName;

    public string description => data.description;

    public Target target => data.target;

    public Sprite sprite => data.cardSprite;
    public List<ICardEffect> effects => data.effects;

    public bool isInstantUse => data.isInstantUse;

    public bool isCharged = false;

    public int mana {  get; private set; }
    public int power {  get; private set; }
    public int powerDisplay {  get; private set; }

    private readonly CardDataSO data;
    private readonly CharacterInfo owner;

    public Card(CardDataSO cardData, CharacterInfo ownerData)
    {
        data = cardData;
        owner = ownerData;
        power = data.power;

        bool hasCharge = data.effects.Any(e => e is ChargeDamageEffect);
        mana = data.cost;
        if (hasCharge && owner != null)
        {
            mana = owner.currentEN;
            isCharged = true;
        }
        if (ownerData != null)
        {
            Debug.Log("yes calc");
            powerDisplay = Calculate(data.power, ownerData.stats.totalPWR);
        }
        else
        {
            Debug.Log("no calc");
            powerDisplay = data.power;
        }
    }

    public void UpdateInfo()
    {
        bool hasCharge = data.effects.Any(e => e is ChargeDamageEffect);
        mana = data.cost;
        if (hasCharge && owner != null)
        {
            mana = owner.currentEN;
            isCharged = true;
        }

        if (owner != null)
        {
            powerDisplay = Calculate(power, owner.stats.totalPWR);
        }
        else
        {
            powerDisplay = power;
        }
    }

    public void UpdateManaCost(int cost)
    {
        bool hasCharge = data.effects.Any(e => e is ChargeDamageEffect);
        if (hasCharge)
        {
            mana = cost;
        }

        if (owner != null)
        {
            powerDisplay = Calculate(power, owner.stats.totalPWR);
        }
        else
        {
            powerDisplay = power;
        }
    }

    private int Calculate(int cardPower, int characterPower)
    {
        float result = cardPower * (characterPower / 20f);
        return Mathf.FloorToInt(result);
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
