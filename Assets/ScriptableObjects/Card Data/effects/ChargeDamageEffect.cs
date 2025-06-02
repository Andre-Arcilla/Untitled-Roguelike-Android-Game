using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargeDamageEffect : ICardEffect
{
    [SerializeField] private int multiplier;
    public void Execute(Targetable senderObj, CardInformation card, GameObject targetObj, int manaCost)
    {
        CharacterInfo sender = senderObj.GetComponent<CharacterInfo>();
        CharacterInfo target = targetObj.GetComponent<CharacterInfo>();

        float cardPower = manaCost * multiplier;
        int damage = Calculate(cardPower, sender.stats.totalPWR);

        int finalDamage = target.ApplyPreDamageModifiers(damage);

        bool damageNegated = target.TriggerOnHitEffects(sender, finalDamage);

        if (!damageNegated)
        {
            target.currentHP -= finalDamage;
        }
    }

    private int Calculate(float cardPower, int characterPower)
    {
        float result = cardPower * (characterPower / 20f);
        return Mathf.FloorToInt(result);
    }
}