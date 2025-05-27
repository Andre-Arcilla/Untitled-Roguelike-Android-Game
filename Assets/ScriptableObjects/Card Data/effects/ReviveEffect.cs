using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReviveEffect : ICardEffect
{
    public void Execute(Targetable senderObj, CardInformation card, GameObject targetObj, int manaCost)
    {
        CharacterInfo sender = senderObj.GetComponent<CharacterInfo>();
        CharacterInfo target = targetObj.GetComponent<CharacterInfo>();

        int revive = Calculate(card.card.power, sender.stats.totalPWR);

        if (target.currentHP <= 0)
        {
            target.currentHP += revive;
        }

        if (target.currentHP > target.maxHP)
        {
            target.currentHP = target.maxHP;
        }
    }

    private int Calculate(int cardPower, int characterPower)
    {
        float result = cardPower * (characterPower / 20f);
        return Mathf.FloorToInt(result);
    }
}