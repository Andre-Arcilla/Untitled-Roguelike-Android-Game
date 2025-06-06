using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockDownStatusEffect : IStatusEffect
{
    public string Name => "LockDown";
    public int Duration { get => duration; set => duration = value; }
    public bool IsShortTerm => _IsShortTerm;
    public bool AllowsStacking => _AllowsStacking;
    public bool Override => false;
    public bool IsDebuff => _IsDebuff;

    [SerializeField] private int duration = 1;
    [SerializeField] private bool _IsShortTerm = true;
    [SerializeField] private bool _AllowsStacking = false;
    [SerializeField] private bool _IsDebuff = true;
    private CharacterInfo target;

    public LockDownStatusEffect() { }

    public void OnApply(CharacterInfo target)
    {
        this.target = target;
    }

    public void OnTurnStart()
    {
        duration--;
    }

    public void OnRemove() { }

    public void OnTrigger()
    {

    }
}
