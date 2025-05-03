using UnityEngine;

public class DamageEffect : ICardEffect
{
    public void Execute(Targetable senderObj, CardInformation card, GameObject targetObj)
    {
        CharacterInfo sender = senderObj.GetComponent<CharacterInfo>();
        CharacterInfo target = targetObj.GetComponent<CharacterInfo>();

        int damage = CalculateDamage(card.card.power, sender.stats.totalPWR);

        target.currentHP -= damage;

        if (target.currentHP <= 0)
        {
            target.gameObject.SetActive(false);
        }
    }

    private int CalculateDamage(int cardPower, int characterPower)
    {
        float result = cardPower * (characterPower / 20f);
        return Mathf.FloorToInt(result);
    }
}