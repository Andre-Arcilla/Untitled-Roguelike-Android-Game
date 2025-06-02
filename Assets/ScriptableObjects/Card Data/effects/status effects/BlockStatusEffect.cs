using UnityEngine;

public class BlockStatusEffect : IStatusEffect
{
    public string Name => "Block";
    public int Duration { get => duration; set => duration = value; }
    public bool IsShortTerm => _IsShortTerm;
    public bool ExpiresOnHit => _ExpiresOnHit;
    public bool AllowsStacking => _AllowsStacking;
    public bool Override => false;
    public bool IsDebuff => _IsDebuff;

    [SerializeField] private int duration = 1;
    [SerializeField] private bool _ExpiresOnHit;
    [SerializeField] private bool _IsShortTerm = true;
    [SerializeField] private bool _AllowsStacking = false;
    [SerializeField] private bool _IsDebuff = false;
    private CharacterInfo target;

    public BlockStatusEffect() { }

    public BlockStatusEffect(bool expiresOnHit)
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
