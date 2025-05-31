using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PWRUpStatusEffect : IStatusEffect
{
    public string Name => "Power Up";
    public int Duration { get => duration; set => duration = value; }
    public bool IsShortTerm => _IsShortTerm;
    public bool AllowsStacking => _AllowsStacking;

    [SerializeField] private int duration = 2;
    [SerializeField] private bool _IsShortTerm = true;
    [SerializeField] private bool _AllowsStacking = false;
    [SerializeField] public int buffAmount = 2;

    private CharacterInfo target;

    public PWRUpStatusEffect() { }

    public PWRUpStatusEffect(int buffAmount, int duration, bool isShortTerm, bool isStackable)
    {
        this.buffAmount = buffAmount;
        this.duration = duration;
        _IsShortTerm = isShortTerm;
        _AllowsStacking = isStackable;
    }

    public void OnApply(CharacterInfo target)
    {
        this.target = target;
        target.stats.totalPWR += buffAmount;
    }

    public void OnTurnStart()
    {
        duration--;
    }

    public void OnRemove()
    {
        if (target != null)
        {
            target.stats.totalPWR -= buffAmount;
        }
    }

    public void OnTrigger() { }
}