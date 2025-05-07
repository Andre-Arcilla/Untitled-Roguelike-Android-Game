using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargeDamageEffect : ICardEffect
{
    [SerializeField] private int multiplier;

    public void Execute(Targetable senderObj, CardInformation card, GameObject targetObj)
    {
        CharacterInfo sender = senderObj.GetComponent<CharacterInfo>();
        CharacterInfo target = targetObj.GetComponent<CharacterInfo>();

        int cardPower = card.card.mana * multiplier;
        int damage = Calculate(cardPower, sender.stats.totalPWR);

        target.currentHP -= damage;

        if (target.currentHP <= 0)
        {
            target.gameObject.SetActive(false);
        }
    }

    private int Calculate(int cardPower, int characterPower)
    {
        float result = cardPower * (characterPower / 20f);
        return Mathf.FloorToInt(result);
    }
}