using UnityEngine;

public class CounterStatusEffect : IStatusEffect
{
    public string Name => "Counter";
    public int Duration { get => duration; set => duration = value; }
    public bool NegatesDamage => _NegateDamage;
    public bool ExpiresOnHit => _ExpiresOnHit;
    public bool IsShortTerm => _IsShortTerm;

    [SerializeField] private int duration = 1;
    [SerializeField] private int damage;
    [SerializeField] private bool _NegateDamage = false;
    [SerializeField] private bool _ExpiresOnHit = false;
    [SerializeField] private bool _IsShortTerm = true;
    private CharacterInfo target;

    public CounterStatusEffect() { }

    public CounterStatusEffect(int damage, bool negateDamage, bool expiresOnHit)
    {
        this.damage = damage;
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

    public void OnTrigger(CharacterInfo attacker)
    {
        attacker.currentHP -= damage;
    }
}
