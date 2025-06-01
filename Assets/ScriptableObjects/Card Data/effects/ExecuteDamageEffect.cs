using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExecuteDamageEffect : ICardEffect
{
    [SerializeField]
    [Range(0f, 1f)]
    private float hpThreshold = 0.5f;
    [SerializeField]
    [Range(0f, 1f)]
    private float executeDamage = 0.5f;

    public void Execute(Targetable senderObj, CardInformation card, GameObject targetObj, int manaCost)
    {
        CharacterInfo sender = senderObj.GetComponent<CharacterInfo>();
        CharacterInfo target = targetObj.GetComponent<CharacterInfo>();

        int damage = Calculate(card.card.power, sender.stats.totalPWR);

        if (target.currentHP < target.maxHP * hpThreshold)
        {
            damage += (int)(damage * executeDamage);
        }

        int finalDamage = target.ApplyPreDamageModifiers(damage);

        bool damageNegated = target.TriggerOnHitEffects(sender, finalDamage);

        if (!damageNegated)
        {
            target.currentHP -= finalDamage;
        }
    }

    private int Calculate(int cardPower, int characterPower)
    {
        float result = cardPower * (characterPower / 20f);
        return Mathf.FloorToInt(result);
    }
}
