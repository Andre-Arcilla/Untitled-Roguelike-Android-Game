using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TauntStatusEffect : IStatusEffect
{
    public string Name => "Taunt";
    public int Duration { get => duration; set => duration = value; }
    public bool IsShortTerm => _IsShortTerm;
    public bool AllowsStacking => _AllowsStacking;

    [SerializeField] private int duration = 1;
    [SerializeField] private bool _IsShortTerm = true;
    [SerializeField] private bool _AllowsStacking = false;
    private CharacterInfo target;

    public TauntStatusEffect() { }

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