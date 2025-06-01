using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ENUpEffect : ICardEffect
{
    [SerializeField] private int duration = 2;
    [SerializeField] private int buffAmount;
    [SerializeField] private bool isShortTerm;
    [SerializeField] private bool isStackable;
    [SerializeField] private bool isDebuff;

    public void Execute(Targetable senderObj, CardInformation card, GameObject targetObj, int manaCost)
    {
        CharacterInfo target = targetObj.GetComponent<CharacterInfo>();
        if (target == null) return;

        var buff = new ENUpStatusEffect(buffAmount, duration, isShortTerm, isStackable, isDebuff);
        target.ApplyStatusEffect(buff);
    }
}

