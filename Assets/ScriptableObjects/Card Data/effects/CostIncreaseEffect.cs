using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CostIncreaseEffect : ICardEffect
{
    [SerializeField]
    [Range(0f, 1f)]
    private float increasePercentage = 0.5f;

    public void Execute(Targetable senderObj, CardInformation card, GameObject targetObj, int manaCost)
    {
        card.card.ChangeMana(Calculate(card.card.mana), Change.Increase);
        card.UpdateCard();
    }

    private int Calculate(int cardCost)
    {
        float result = cardCost + (cardCost * increasePercentage);
        return Mathf.FloorToInt(result);
    }
}
