using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardEnDownEffect : ICardEffect
{
    [SerializeField] private int amount;
    [SerializeField] private Change change;

    public void Execute(Targetable senderObj, CardInformation card, GameObject targetObj, int manaCost)
    {
        CharacterInfo sender = senderObj.GetComponent<CharacterInfo>();
        CardInformation target = targetObj.GetComponent<CardInformation>();

        target.card.ChangeMana(amount, change);
    }
}