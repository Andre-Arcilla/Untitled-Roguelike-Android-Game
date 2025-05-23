using UnityEngine;

public class DamageEffect : ICardEffect
{
    public void Execute(Targetable senderObj, CardInformation card, GameObject targetObj)
    {
        CharacterInfo sender = senderObj.GetComponent<CharacterInfo>();
        CharacterInfo target = targetObj.GetComponent<CharacterInfo>();

        int damage = Calculate(card.card.power, sender.stats.totalPWR);

        bool damageNegated = target.TriggerOnHitEffects(sender);

        if (!damageNegated)
        {
            target.currentHP -= damage;
        }
    }

    private int Calculate(int cardPower, int characterPower)
    {
        float result = cardPower * (characterPower / 20f);
        return Mathf.FloorToInt(result);
    }
}
