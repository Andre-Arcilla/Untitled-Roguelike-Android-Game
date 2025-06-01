using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageBurnedEffect : ICardEffect
{
    [SerializeField]
    [Range(0f, 1f)]
    private float damagePercent = 0.5f;
    [SerializeField] private string effectName;

    public void Execute(Targetable senderObj, CardInformation card, GameObject targetObj, int manaCost)
    {
        CharacterInfo sender = senderObj.GetComponent<CharacterInfo>();
        CharacterInfo target = targetObj.GetComponent<CharacterInfo>();

        int damage = Calculate(card.card.power, sender.stats.totalPWR);

        Debug.Log($"final damage {damage}");
        foreach (var effect in target.activeEffects)
        {
            if (effect.Name.ToLower() == effectName.ToLower())
            {
                damage = Mathf.FloorToInt(damage + (damage * damagePercent));
                break;
            }
        }

        int finalDamage = target.ApplyPreDamageModifiers(damage);

        bool damageNegated = target.TriggerOnHitEffects(sender, finalDamage);

        if (!damageNegated)
        {
            Debug.Log($"final damage {finalDamage}");
            target.currentHP -= finalDamage;
        }
    }

    private int Calculate(int cardPower, int characterPower)
    {
        float result = cardPower * (characterPower / 20f);
        return Mathf.FloorToInt(result);
    }
}
