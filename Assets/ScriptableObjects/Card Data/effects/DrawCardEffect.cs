using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawCardEffect : ICardEffect
{
    [SerializeField] private int amount;

    public void Execute(Targetable senderObj, CardInformation card, GameObject targetObj)
    {
        CharacterInfo sender = senderObj.GetComponent<CharacterInfo>();

        sender.GetComponent<CharacterDeck>().DrawCard(amount, card);
    }
}
