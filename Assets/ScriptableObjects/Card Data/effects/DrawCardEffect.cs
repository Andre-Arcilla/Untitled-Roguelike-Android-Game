using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawCardEffect : ICardEffect
{
    [SerializeField] private int drawAmount;
    [SerializeField] private bool drawOnly = true;

    public void Execute(Targetable senderObj, CardInformation card, GameObject targetObj, int manaCost)
    {
        CharacterInfo sender = senderObj.GetComponent<CharacterInfo>();

        sender.GetComponent<CharacterDeck>().DrawCard(drawAmount, card, drawOnly);
    }
}
