using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TauntEffect : ICardEffect
{
    public void Execute(Targetable senderObj, CardInformation card, GameObject targetObj, int manaCost)
    {
        CharacterInfo target = targetObj.GetComponent<CharacterInfo>();
        if (target == null) return;

        var taunt = new TauntStatusEffect();
        target.ApplyStatusEffect(taunt);
    }
}

