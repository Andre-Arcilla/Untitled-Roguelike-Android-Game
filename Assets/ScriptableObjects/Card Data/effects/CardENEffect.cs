using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardEnDownEffect : ICardEffect
{
    [SerializeField] private Change change;

    public void Execute(Targetable senderObj, CardInformation card, GameObject targetObj)
    {
        CharacterInfo sender = senderObj.GetComponent<CharacterInfo>();
        CardInformation target = targetObj.GetComponent<CardInformation>();

        target.card.ChangeMana(card.card.power, change);
        target.UpdateCard();
    }
}