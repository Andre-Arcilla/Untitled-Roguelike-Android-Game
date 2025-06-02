using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RegenStatusEffect : IStatusEffect
{
    public string Name => "Regeneration";
    public int Duration { get => duration; set => duration = value; }
    public bool IsShortTerm => false;
    public bool AllowsStacking => false;
    public bool Override => false;
    public bool IsDebuff => false;

    [SerializeField] private int duration;
    [SerializeField] private int healAmount;
    private CharacterInfo target;

    public RegenStatusEffect() { }

    public RegenStatusEffect(int duration, int healAmount)
    {
        this.duration = duration;
        this.healAmount = healAmount;
    }

    public void OnApply(CharacterInfo target)
    {
        this.target = target;
    }

    public void OnTurnStart()
    {
        if (target == null) return;

        target.currentHP += healAmount;
        target.currentHP = Mathf.Min(target.maxHP, target.currentHP); // Prevent overheal
        duration--;
    }

    public void OnRemove() { }
}
