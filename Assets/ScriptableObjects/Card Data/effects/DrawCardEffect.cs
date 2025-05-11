using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawCardEffect : ICardEffect
{
    public void Execute(Targetable senderObj, CardInformation card, GameObject targetObj)
    {
        CharacterInfo sender = senderObj.GetComponent<CharacterInfo>();

        sender.GetComponent<CharacterDeck>().DrawCard(card.card.power, card);
    }
}
