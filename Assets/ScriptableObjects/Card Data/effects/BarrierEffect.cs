using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrierEffect : ICardEffect
{
    public void Execute(Targetable senderObj, CardInformation card, GameObject targetObj, int manaCost)
    {
        CharacterInfo sender = senderObj.GetComponent<CharacterInfo>();
        CharacterInfo target = targetObj.GetComponent<CharacterInfo>();

        int barrier = Calculate(card.card.power, sender.stats.totalPWR);

        target.barrier += barrier;
    }

    private int Calculate(int cardPower, int characterPower)
    {
        float result = cardPower * (characterPower / 20f);
        return Mathf.FloorToInt(result);
    }
}