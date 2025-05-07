using UnityEngine;

public class CounterStatusEffect : IStatusEffect
{
    public string Name => "Counter";
    public int Duration { get => duration; set => duration = value; }
    public bool IsShortTerm => true;
    public bool NegatesDamage => negateDamage;
    public bool ExpiresOnHit => expiresOnHit;

    [SerializeField] private int duration = 1;
    [SerializeField] private int damage;
    [SerializeField] private bool negateDamage;
    [SerializeField] private bool expiresOnHit;
    [SerializeField] private CharacterInfo target;

    public CounterStatusEffect(int damage, bool negateDamage, bool expiresOnHit)
    {
        this.damage = damage;
        this.negateDamage = negateDamage;
        this.expiresOnHit = expiresOnHit;
    }

    public void OnApply(CharacterInfo target)
    {
        this.target = target;
        Debug.Log(target.characterData.basicInfo.characterName + " has gained counter");
    }

    public void OnTurnStart() { }

    public void OnRemove() { Debug.Log(target.characterData.basicInfo.characterName + " has lost counter"); }

    public void OnTrigger(CharacterInfo attacker)
    {
        attacker.currentHP -= damage;
        Debug.Log(attacker.characterData.basicInfo.characterName + " got hit with a counter");
    }
}
