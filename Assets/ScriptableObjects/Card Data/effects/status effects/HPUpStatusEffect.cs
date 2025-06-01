using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HPUpStatusEffect : IStatusEffect
{
    public string Name => "Energy Up";
    public int Duration { get => duration; set => duration = value; }
    public bool IsShortTerm => _IsShortTerm;
    public bool AllowsStacking => _AllowsStacking;
    public bool IsDebuff => _IsDebuff;

    [SerializeField] private int duration = 2;
    [SerializeField] private bool _IsShortTerm = true;
    [SerializeField] private bool _AllowsStacking = false;
    [SerializeField] private bool _IsDebuff = false;
    [SerializeField] public int buffAmount = 2;

    private CharacterInfo target;

    public HPUpStatusEffect() { }

    public HPUpStatusEffect(int buffAmount, int duration, bool isShortTerm, bool isStackable, bool isDebuff)
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
        target.maxHP += buffAmount;
    }

    public void OnTurnStart()
    {
        duration--;
    }

    public void OnRemove()
    {
        if (target != null)
        {
            target.maxHP -= buffAmount;
        }
    }

    public void OnTrigger() { }
}