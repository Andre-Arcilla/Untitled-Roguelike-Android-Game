using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PWRUpEffect : ICardEffect
{
    [SerializeField] private int duration = 2;
    [SerializeField] private int buffAmount;
    [SerializeField] private bool isShortTerm;
    [SerializeField] private bool isStackable;

    public void Execute(Targetable senderObj, CardInformation card, GameObject targetObj, int manaCost)
    {
        CharacterInfo target = targetObj.GetComponent<CharacterInfo>();
        if (target == null) return;

        var buff = new PWRUpStatusEffect(buffAmount, duration, isShortTerm, isStackable);
        target.ApplyStatusEffect(buff);
    }
}

