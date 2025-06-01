using System.Collections;
using UnityEngine;

public class DonationEffect : ICardEffect
{
    [SerializeField]
    [Range(0f, 1f)]
    private float donationAmount = 0.25f;

    [SerializeField] private int duration = 0;
    [SerializeField] private int regen = 0;

    public void Execute(Targetable senderObj, CardInformation card, GameObject targetObj, int manaCost)
    {
        CharacterInfo sender = senderObj.GetComponent<CharacterInfo>();
        CharacterInfo target = targetObj.GetComponent<CharacterInfo>();

        if (target == null || sender == null || target.currentHP <= 0)
            return;

        if (sender.characterData.basicInfo.characterName == target.characterData.basicInfo.characterName)
        {
            var DOT = new RegenStatusEffect(duration, regen);
            target.ApplyStatusEffect(DOT);
            return;
        }

        int donationValue = Mathf.FloorToInt(sender.currentHP * donationAmount);

        donationValue = Mathf.Min(donationValue, sender.currentHP - 1);

        int maxHealable = target.maxHP - target.currentHP;
        int actualHeal = Mathf.Min(donationValue, maxHealable);

        sender.currentHP -= actualHeal;
        target.currentHP += actualHeal;
    }
}
