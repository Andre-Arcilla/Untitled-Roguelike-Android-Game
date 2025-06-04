using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class CounterStatusEffect : IStatusEffect
{
    public string Name => "Counter";
    public int Duration { get => duration; set => duration = value; }
    public bool NegatesDamage => _NegateDamage;
    public bool ExpiresOnHit => _ExpiresOnHit;
    public bool IsShortTerm => _IsShortTerm;
    public bool AllowsStacking => _AllowsStacking;
    public bool Override => _Override;
    public bool IsDebuff => _IsDebuff;

    [SerializeField] private int duration = 1;
    [SerializeField] private bool _NegateDamage = false;
    [SerializeField] private bool _ExpiresOnHit = false;
    [SerializeField] private bool _IsShortTerm = true;
    [SerializeField] private bool _AllowsStacking = false;
    [SerializeField] private bool _Override = false;
    [SerializeField] private bool _IsDebuff = false;
    private CharacterInfo target;

    public CounterStatusEffect() { }

    public CounterStatusEffect(bool negateDamage, bool expiresOnHit, bool _Override)
    {
        this._NegateDamage = negateDamage;
        this._ExpiresOnHit = expiresOnHit;
        this._Override = _Override;
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

        if (attacker.currentHP <= 0)
        {
            attacker.UpdateResourcesView();
            attacker.currentHP = 0;

            Targetable attackerTargetable = attacker.GetComponent<Targetable>();

            if (TargetingSystem.Instance.enemies.members.Contains(attackerTargetable))
            {
                int levelDiff = Mathf.Max(attacker.characterData.basicInfo.level - target.characterData.basicInfo.level, 1);
                int XPGain = CalculateXP(target.characterData.basicInfo.level, levelDiff);

                float goldMultiplier = Mathf.Max(levelDiff * 0.75f, 1.25f);
                int goldFound = Mathf.RoundToInt(UnityEngine.Random.Range(50, 101) * goldMultiplier);

                CombatSystem.Instance.AddGoldFound(goldFound);

                foreach (Targetable ally in TargetingSystem.Instance.allies.members)
                {
                    CharacterData allyData = ally.GetComponent<CharacterInfo>().characterData;
                    if (allyData == target.characterData)
                    {
                        CombatSystem.Instance.AddXP(allyData, XPGain);
                    }
                    else
                    {
                        CombatSystem.Instance.AddXP(allyData, XPGain / 2);
                    }
                }
            }
        }
    }

    private int CalculateXP(int charLevel, int enemyLevel)
    {
        int levelDiff = Mathf.Max(enemyLevel, 1);
        int totalXP = 0;

        totalXP += levelDiff * 10;

        return totalXP;
    }
}