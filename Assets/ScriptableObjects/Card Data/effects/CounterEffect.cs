using UnityEngine;

public class CounterEffect : ICardEffect
{
    [SerializeField] private bool negateDamage;
    [SerializeField] private bool expiresOnHit;

    public void Execute(Targetable senderObj, CardInformation card, GameObject targetObj, int manaCost)
    {
        CharacterInfo target = targetObj.GetComponent<CharacterInfo>();
        if (target == null) return;

        var counter = new CounterStatusEffect(negateDamage, expiresOnHit);
        target.ApplyStatusEffect(counter);
    }
}