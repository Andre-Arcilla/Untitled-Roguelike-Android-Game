using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConsumeBurnEffect : ICardEffect
{
    [SerializeField] private int damagePerBurn;
    [SerializeField] private string effectName;

    public void Execute(Targetable senderObj, CardInformation card, GameObject targetObj, int manaCost)
    {
        CharacterInfo sender = senderObj.GetComponent<CharacterInfo>();
        CharacterInfo target = targetObj.GetComponent<CharacterInfo>();

        int totalBurn = 0;
        List<IStatusEffect> effectsToRemove = new List<IStatusEffect>();

        foreach (var effect in target.activeEffects)
        {
            if (effect.Name.ToLower() == effectName.ToLower())
            {
                totalBurn += effect.Duration;
                effectsToRemove.Add(effect);
            }
        }

        // Remove the burn effects
        foreach (var burn in effectsToRemove)
        {
            burn.OnRemove(); // Handle stat reversal if needed
            target.activeEffects.Remove(burn);
        }

        int damage = Calculate(totalBurn * damagePerBurn, sender.stats.totalPWR);

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