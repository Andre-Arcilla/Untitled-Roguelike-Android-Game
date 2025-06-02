using UnityEngine;

public class DamageReductionStatusEffect : IStatusEffect
{
    public string Name => "Damage Reduction";
    public int Duration { get => duration; set => duration = value; }
    public bool IsShortTerm => _IsShortTerm;
    public bool AllowsStacking => _AllowsStacking;
    public bool Override => false;
    public bool IsDebuff => _IsDebuff;

    private CharacterInfo target;

    [SerializeField] private int duration = 2;
    [SerializeField] private float percentReduction = 0.5f; // Percent damage reduction
    [SerializeField] private bool _IsShortTerm = false;
    [SerializeField] private bool _AllowsStacking = false;
    [SerializeField] private bool _IsDebuff = false;

    public DamageReductionStatusEffect() { }

    public DamageReductionStatusEffect(float percentReduction, int duration = 2)
    {
        this.percentReduction = percentReduction;
        this.duration = duration;
    }

    public void OnApply(CharacterInfo target)
    {
        this.target = target;
    }

    public void OnTurnStart()
    {
        duration--;
        if (duration <= 0)
        {
            OnRemove();
        }
    }

    public void OnRemove() { }

    public int ModifyIncomingDamage(int incomingDamage)
    {
        int reduced = incomingDamage - (int)(incomingDamage * percentReduction);
        return Mathf.Max(0, reduced); // Don't go below 0
    }
}

