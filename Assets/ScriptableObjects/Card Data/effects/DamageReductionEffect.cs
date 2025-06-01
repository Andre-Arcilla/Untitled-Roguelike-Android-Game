using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageReductionEffect : ICardEffect
{
    [SerializeField]
    [Range(0f, 1f)]
    private float percentReduction = 0.5f;
    [SerializeField] private int duration = 0;

    public void Execute(Targetable senderObj, CardInformation card, GameObject targetObj, int manaCost)
    {
        CharacterInfo target = targetObj.GetComponent<CharacterInfo>();
        if (target == null) return;

        var dmgReduce = new DamageReductionStatusEffect(percentReduction, duration);
        target.ApplyStatusEffect(dmgReduce);
    }
}
