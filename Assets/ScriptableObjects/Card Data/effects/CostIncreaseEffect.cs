using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CostIncreaseEffect : ICardEffect
{
    [SerializeField]
    [Range(0f, 1f)]
    private float increasePercentage = 0.5f;

    public void Execute(Targetable senderObj, CardInformation cardInfo, GameObject targetObj, int manaCost)
    {
        var card = cardInfo.card;
        int newCost = Calculate(card.mana);

        card.ChangeMana(newCost, Change.Increase);
    }

    private int Calculate(int cardCost)
    {
        float result = (cardCost * increasePercentage);
        return Mathf.CeilToInt(result);
    }
}
