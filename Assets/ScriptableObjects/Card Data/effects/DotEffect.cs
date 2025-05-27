using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DotEffect : ICardEffect
{
    [SerializeField] private int duration = 0;
    [SerializeField] private int damage = 0;

    public void Execute(Targetable senderObj, CardInformation card, GameObject targetObj, int manaCost)
    {
        CharacterInfo target = targetObj.GetComponent<CharacterInfo>();
        if (target == null) return;

        var DOT = new BurnStatusEffect(duration, damage);
        target.ApplyStatusEffect(DOT);
    }
}