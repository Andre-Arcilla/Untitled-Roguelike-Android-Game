using UnityEngine;

public class HealEffect : ICardEffect
{
    public void Execute(Targetable senderObj, CardInformation card, GameObject targetObj)
    {
        CharacterInfo sender = senderObj.GetComponent<CharacterInfo>();
        CharacterInfo target = targetObj.GetComponent<CharacterInfo>();

        int heal = Calculate(card.card.power, sender.stats.totalPWR);

        if (target.currentHP > 0)
        {
            target.currentHP += heal;
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