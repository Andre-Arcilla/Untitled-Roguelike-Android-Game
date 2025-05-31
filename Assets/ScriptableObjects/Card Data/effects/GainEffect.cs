using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GainEffect : ICardEffect
{
    [SerializeField] private int gainAmount;

    public void Execute(Targetable senderObj, CardInformation card, GameObject targetObj, int manaCost)
    {
        CharacterInfo sender = senderObj.GetComponent<CharacterInfo>();
        CharacterInfo target = targetObj.GetComponent<CharacterInfo>();

        target.currentEN += gainAmount;
    }
}