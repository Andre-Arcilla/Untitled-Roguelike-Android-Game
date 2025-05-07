using UnityEngine;

public class CounterEffect : ICardEffect
{
    [SerializeField] private int damage = 0;
    [SerializeField] private bool negateDamage;
    [SerializeField] private bool expiresOnHit;

    public void Execute(Targetable senderObj, CardInformation card, GameObject targetObj)
    {
        CharacterInfo target = targetObj.GetComponent<CharacterInfo>();
        if (target == null) return;

        var counter = new CounterStatusEffect(damage, negateDamage, expiresOnHit);
        target.ApplyStatusEffect(counter);
    }
}