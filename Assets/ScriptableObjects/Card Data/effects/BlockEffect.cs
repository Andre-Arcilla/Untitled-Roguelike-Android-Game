using UnityEngine;

public class BlockEffect : ICardEffect
{
    [SerializeField] private bool expiresOnHit;
    public void Execute(Targetable senderObj, CardInformation card, GameObject targetObj, int manaCost)
    {
        CharacterInfo target = targetObj.GetComponent<CharacterInfo>();
        if (target == null) return;

        var block = new BlockStatusEffect(expiresOnHit);
        target.ApplyStatusEffect(block);
    }
}
