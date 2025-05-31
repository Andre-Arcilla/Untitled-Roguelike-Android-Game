using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockDownEffect : ICardEffect
{
    public void Execute(Targetable senderObj, CardInformation card, GameObject targetObj, int manaCost)
    {
        CharacterInfo target = targetObj.GetComponent<CharacterInfo>();
        if (target == null) return;

        var lockDown = new LockDownStatusEffect();
        target.ApplyStatusEffect(lockDown);
    }
}
