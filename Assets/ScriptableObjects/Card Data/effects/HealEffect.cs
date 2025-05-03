using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealEffect : ICardEffect
{
    public void Execute(Targetable senderObj, CardInformation card, GameObject targetObj)
    {
        CharacterInfo sender = senderObj.GetComponent<CharacterInfo>();
        CharacterInfo target = targetObj.GetComponent<CharacterInfo>();

        int heal = CalculateDamage(card.card.power, sender.stats.totalPWR);

        target.currentHP += heal;

        if (target.currentHP > target.maxHP)
        {
            target.currentHP = target.maxHP;
        }
    }

    private int CalculateDamage(int cardPower, int characterPower)
    {
        float result = cardPower * (characterPower / 20f);
        return Mathf.FloorToInt(result);
    }
}