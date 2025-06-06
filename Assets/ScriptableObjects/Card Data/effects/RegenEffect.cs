using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RegenEffect : ICardEffect
{
    [SerializeField] private int duration = 0;
    [SerializeField] private int regen = 0;

    public void Execute(Targetable senderObj, CardInformation card, GameObject targetObj, int manaCost)
    {
        CharacterInfo target = targetObj.GetComponent<CharacterInfo>();
        if (target == null) return;

        var DOT = new RegenStatusEffect(duration, regen);
        target.ApplyStatusEffect(DOT);
    }
}