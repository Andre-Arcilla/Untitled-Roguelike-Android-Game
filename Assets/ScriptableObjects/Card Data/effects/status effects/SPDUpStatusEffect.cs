using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SPDUpStatusEffect : IStatusEffect
{
    public string Name => "Speed Up";
    public int Duration { get => duration; set => duration = value; }
    public bool IsShortTerm => _IsShortTerm;
    public bool AllowsStacking => _AllowsStacking;
    public bool Override => false;
    public bool IsDebuff => _IsDebuff;

    [SerializeField] private int duration = 2;
    [SerializeField] private bool _IsShortTerm = true;
    [SerializeField] private bool _AllowsStacking = false;
    [SerializeField] private bool _IsDebuff = false;
    [SerializeField] public int buffAmount = 2;

    private CharacterInfo target;

    public SPDUpStatusEffect() { }

    public SPDUpStatusEffect(int buffAmount, int duration, bool isShortTerm, bool isStackable, bool isDebuff)
    {
        this.buffAmount = buffAmount;
        this.duration = duration;
        _IsShortTerm = isShortTerm;
        _AllowsStacking = isStackable;
        _IsDebuff = isDebuff;
    }

    public void OnApply(CharacterInfo target)
    {
        this.target = target;
        target.stats.totalSPD += buffAmount;
    }

    public void OnTurnStart()
    {
        duration--;
    }

    public void OnRemove()
    {
        if (target != null)
        {
            target.stats.totalSPD -= buffAmount;
        }
    }

    public void OnTrigger() { }
}