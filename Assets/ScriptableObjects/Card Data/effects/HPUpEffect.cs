using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HPUpEffect : ICardEffect
{
    [SerializeField] private int duration = 2;
    [SerializeField] private int flatBuff;
    [SerializeField]
    [Range(0f, 1f)]
    private float percBuff = 0.5f;
    [SerializeField] private bool isShortTerm;
    [SerializeField] private bool isStackable;
    [SerializeField] private bool isDebuff;

    public void Execute(Targetable senderObj, CardInformation card, GameObject targetObj, int manaCost)
    {
        CharacterInfo target = targetObj.GetComponent<CharacterInfo>();
        if (target == null) return;

        int buffAmount = 0;

        if (flatBuff > 0 && percBuff <= 0)
        {
            buffAmount = flatBuff;
        }
        else if (percBuff > 0 && flatBuff <= 0)
        {
            buffAmount = Mathf.FloorToInt(percBuff * target.maxHP);
        }
        else
        {
            buffAmount = 0;
        }

        var buff = new HPUpStatusEffect(buffAmount, duration, isShortTerm, isStackable, isDebuff);
        target.ApplyStatusEffect(buff);
    }
}

