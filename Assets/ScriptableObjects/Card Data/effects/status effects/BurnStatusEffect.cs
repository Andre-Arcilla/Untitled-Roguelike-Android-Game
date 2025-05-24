using UnityEngine;

public class BurnStatusEffect : IStatusEffect
{
    public string Name => "Burn";
    public int Duration { get => duration; set => duration = value; }
    public bool IsShortTerm => _IsShortTerm;

    [SerializeField] private int duration;
    [SerializeField] private int damage;
    [SerializeField] private bool _IsShortTerm = false;
    private CharacterInfo target;

    public BurnStatusEffect() { }

    public BurnStatusEffect(int duration, int damage)
    {
        this.duration = duration;
        this.damage = damage;
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