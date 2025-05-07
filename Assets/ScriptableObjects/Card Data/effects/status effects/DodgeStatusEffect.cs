using UnityEngine;

public class DodgeStatusEffect : IStatusEffect
{
    public string Name => "Dodge";
    public int Duration { get => duration; set => duration = value; }
    public bool IsShortTerm => true;
    public bool ExpiresOnHit => expiresOnHit;

    [SerializeField] private int duration = 1;
    [SerializeField] private bool expiresOnHit;
    [SerializeField] private CharacterInfo target;

    public DodgeStatusEffect(bool expiresOnHit)
    {
        this.expiresOnHit = expiresOnHit;
    }

    public void OnApply(CharacterInfo target)
    {
        this.target = target;
        Debug.Log(target.characterData.basicInfo.characterName + " has gained dodge");
    }

    public void OnTurnStart() { }

    public void OnRemove() { Debug.Log(target.characterData.basicInfo.characterName + " has lost dodge"); }

    public void OnTrigger(CharacterInfo attacker)
    {
        Debug.Log(attacker.characterData.basicInfo.characterName + " missed");
    }
}
