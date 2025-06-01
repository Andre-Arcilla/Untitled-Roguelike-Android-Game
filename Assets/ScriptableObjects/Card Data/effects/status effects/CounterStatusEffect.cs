using UnityEngine;

public class CounterStatusEffect : IStatusEffect
{
    public string Name => "Counter";
    public int Duration { get => duration; set => duration = value; }
    public bool NegatesDamage => _NegateDamage;
    public bool ExpiresOnHit => _ExpiresOnHit;
    public bool IsShortTerm => _IsShortTerm;
    public bool AllowsStacking => _AllowsStacking;
    public bool IsDebuff => _IsDebuff;

    [SerializeField] private int duration = 1;
    [SerializeField] private bool _NegateDamage = false;
    [SerializeField] private bool _ExpiresOnHit = false;
    [SerializeField] private bool _IsShortTerm = true;
    [SerializeField] private bool _AllowsStacking = false;
    [SerializeField] private bool _IsDebuff = false;
    private CharacterInfo target;

    public CounterStatusEffect() { }

    public CounterStatusEffect(bool negateDamage, bool expiresOnHit)
    {
        this._NegateDamage = negateDamage;
        this._ExpiresOnHit = expiresOnHit;
    }

    public void OnApply(CharacterInfo target)
    {
        this.target = target;
    }

    public void OnTurnStart()
    {
        duration--;
    }

    public void OnRemove() { }

    public void OnTrigger(CharacterInfo attacker, int damage)
    {
        int finalDamage = target.ApplyPreDamageModifiers(damage);
        attacker.currentHP -= damage;
    }
}
