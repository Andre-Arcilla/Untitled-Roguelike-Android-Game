using UnityEngine;

public class BurnStatusEffect : IStatusEffect
{
    public string Name => "Burn";
    public int Duration { get => duration; set => duration = value; }
    public bool IsShortTerm => _IsShortTerm;
    public bool AllowsStacking => _AllowsStacking;
    public bool Override => false;
    public bool IsDebuff => _IsDebuff;
    public CharacterInfo Sender => _Sender;

    [SerializeField] private int duration;
    [SerializeField] private int damage;
    [SerializeField] private bool _IsShortTerm = false;
    [SerializeField] private bool _AllowsStacking = false;
    [SerializeField] private bool _IsDebuff = true;
    private CharacterInfo target;
    private CharacterInfo _Sender;

    public BurnStatusEffect() { }

    public BurnStatusEffect(int duration, int damage, CharacterInfo sender)
    {
        this.duration = duration;
        this.damage = damage;
        _Sender = sender;
    }

    public void OnApply(CharacterInfo target)
    {
        this.target = target;
    }

    public void OnTurnStart()
    {
        if (target == null) return;

        target.currentHP -= damage;
        duration--;
    }

    public void OnRemove() { }
}