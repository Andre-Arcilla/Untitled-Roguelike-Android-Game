using UnityEngine;

public class DodgeStatusEffect : IStatusEffect
{
    public string Name => "Dodge";
    public int Duration { get => duration; set => duration = value; }
    public bool IsShortTerm => _IsShortTerm;
    public bool ExpiresOnHit => _ExpiresOnHit;
    public bool AllowsStacking => _AllowsStacking;

    [SerializeField] private int duration = 1;
    [SerializeField] private bool _ExpiresOnHit;
    [SerializeField] private bool _IsShortTerm = true;
    [SerializeField] private bool _AllowsStacking = false;
    private CharacterInfo target;

    public DodgeStatusEffect() { }

    public DodgeStatusEffect(bool expiresOnHit)
    {
        _ExpiresOnHit = expiresOnHit;
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

    public void OnTrigger(CharacterInfo attacker) { }
}
