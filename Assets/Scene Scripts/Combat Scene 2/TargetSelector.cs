using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TargetSelector : MonoBehaviour
{
    public static TargetSelector Instance;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public List<GameObject> GetTargets(CardInformation card, Targetable sender)
    {
        if (card.card.target == Target.Enemy || card.card.target == Target.AllEnemies)
        {
            return EnemyTarget(sender);
        }
        else if (card.card.target == Target.Ally || card.card.target == Target.AllAllies)
        {
            return AllyTarget(sender, card);
        }
        else if (card.card.target == Target.Self)
        {
            return SelfTarget(sender);
        }
        else if (card.card.target == Target.Card)
        {
            return CardTarget(sender, card);
        }
        else if (card.card.target == Target.Draw)
        {
            return TriggerTarget(sender);
        }
        else
        {
            return null;
        }
    }

    private List<GameObject> EnemyTarget(Targetable sender)
    {
        if (sender.team == Team.Player)
        {
            //is player character
            List<Targetable> targets = TargetingSystem.Instance.enemies.members
                .Where(t => t.GetComponent<CharacterInfo>().currentHP > 0)
                .ToList();
            return targets.ConvertAll(t => t.gameObject);
        }
        else if (sender.team == Team.Enemy)
        {
            //is enemy character
            List<Targetable> targets = TargetingSystem.Instance.allies.members
                .Where(t => t.GetComponent<CharacterInfo>().currentHP > 0)
                .ToList();
            return targets.ConvertAll(t => t.gameObject);
        }
        else
        {
            return null;
        }
    }

    private List<GameObject> AllyTarget(Targetable sender, CardInformation card)
    {
        bool isRevive = card.card.effects.Any(e => e is ReviveEffect);

        if (sender.team == Team.Player)
        {
            var targets = TargetingSystem.Instance.allies.members
                .Where(t =>
                {
                    var hp = t.GetComponent<CharacterInfo>().currentHP;
                    return isRevive || hp > 0;
                })
                .ToList();

            return targets.ConvertAll(t => t.gameObject);
        }
        else if (sender.team == Team.Enemy)
        {
            var targets = TargetingSystem.Instance.enemies.members
                .Where(t =>
                {
                    var hp = t.GetComponent<CharacterInfo>().currentHP;
                    return isRevive || hp > 0;
                })
                .ToList();

            return targets.ConvertAll(t => t.gameObject);
        }
        else
        {
            return null;
        }
    }


    private List<GameObject> SelfTarget(Targetable sender)
    {
        return new List<GameObject> { sender.gameObject };
    }

    private List<GameObject> CardTarget(Targetable sender, CardInformation currentCard)
    {
        CharacterDeck deck = sender.GetComponent<CharacterDeck>();

        List<GameObject> allCards = new List<GameObject>();
        allCards.AddRange(deck.hand);
        allCards = allCards
            .Where(cardObj => cardObj != null
                && cardObj.GetComponent<CardInformation>() != currentCard
                && !cardObj.GetComponent<CardInformation>().isSelected)
            .ToList();
        return allCards;
    }

    private List<GameObject> TriggerTarget(Targetable sender)
    {
        return new List<GameObject> { TargetingSystem.Instance.center };
    }
}