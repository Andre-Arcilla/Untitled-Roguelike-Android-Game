using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LifeStealEffect : ICardEffect
{
    [SerializeField]
    [Range(0f, 1f)]
    private float lifeStealPercent = 0.5f;

    public void Execute(Targetable senderObj, CardInformation card, GameObject targetObj, int manaCost)
    {
        CharacterInfo sender = senderObj.GetComponent<CharacterInfo>();
        CharacterInfo target = targetObj.GetComponent<CharacterInfo>();

        if (sender == null || target == null || target.currentHP <= 0) return;

        int damage = Calculate(card.card.power, sender.stats.totalPWR);
        int finalDamage = target.ApplyPreDamageModifiers(damage);

        bool damageNegated = target.TriggerOnHitEffects(sender, finalDamage);

        if (!damageNegated)
        {
            int actualDamage = Mathf.Min(finalDamage, target.currentHP);
            target.currentHP -= actualDamage;

            int healAmount = Mathf.FloorToInt(actualDamage * lifeStealPercent);
            sender.currentHP = Mathf.Min(sender.maxHP, sender.currentHP + healAmount);
        }
    }

    private int Calculate(int cardPower, int characterPower)
    {
        float result = cardPower * (characterPower / 20f);
        return Mathf.FloorToInt(result);
    }
}
