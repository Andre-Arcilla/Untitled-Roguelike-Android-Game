using UnityEngine;

public interface ICardEffect
{
    void Execute(Targetable sender, CardInformation card, GameObject target, int manaCost);
}

