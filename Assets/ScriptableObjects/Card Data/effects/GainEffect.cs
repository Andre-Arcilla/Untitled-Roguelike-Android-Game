using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GainEffect : ICardEffect
{
    public void Execute(Targetable senderObj, CardInformation card, GameObject targetObj, int manaCost)
    {
        CharacterInfo sender = senderObj.GetComponent<CharacterInfo>();
        CharacterInfo target = targetObj.GetComponent<CharacterInfo>();

        int gain = Calculate(card.card.power, sender.stats.totalPWR);

        target.currentEN += gain;
    }

    private int Calculate(int cardPower, int characterPower)
    {
        float result = cardPower * (characterPower / 20f);
        return Mathf.FloorToInt(result);
    }
}