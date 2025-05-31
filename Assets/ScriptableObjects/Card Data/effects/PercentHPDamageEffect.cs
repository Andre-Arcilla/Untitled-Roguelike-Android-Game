using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PercentHPDamageEffect : ICardEffect
{
    [SerializeField] private float percentDamage = 0.1f;

    public void Execute(Targetable senderObj, CardInformation card, GameObject targetObj, int manaCost)
    {
        CharacterInfo sender = senderObj.GetComponent<CharacterInfo>();
        CharacterInfo target = targetObj.GetComponent<CharacterInfo>();

        int damage = Mathf.FloorToInt(target.currentHP * percentDamage);

        int finalDamage = target.ApplyPreDamageModifiers(damage);

        bool damageNegated = target.TriggerOnHitEffects(sender, finalDamage);

        if (!damageNegated)
        {
            target.currentHP -= finalDamage;
        }
    }
}
