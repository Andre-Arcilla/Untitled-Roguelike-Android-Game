using UnityEngine;

public class DodgeEffect : ICardEffect
{
    [SerializeField] private bool expiresOnHit;

    public void Execute(Targetable senderObj, CardInformation card, GameObject targetObj)
    {
        CharacterInfo target = targetObj.GetComponent<CharacterInfo>();
        if (target == null) return;

        var dodge = new DodgeStatusEffect(expiresOnHit);
        target.ApplyStatusEffect(dodge);
    }
}
