using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyActionsManager : MonoBehaviour
{
    public static EnemyActionsManager Instance;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    [System.Serializable]
    public class EnemyDeck
    {
        public CharacterInfo enemy;
        public List<GameObject> cards;

        public EnemyDeck(CharacterInfo enemy, List<GameObject> cards)
        {
            this.enemy = enemy;
            this.cards = cards;
        }
    }

    [SerializeField] public List<EnemyDeck> enemyList;

    public void AddEnemyDeck(CharacterInfo enemy)
    {
        List<GameObject> cards = enemy.GetComponent<CharacterDeck>().hand;

        enemyList.Add(new (enemy, cards));

    }

    public void SetEnemyAction()
    {
        foreach (EnemyDeck enemyChar in enemyList)
        {
            if (enemyChar.enemy.currentHP <= 0)
            { 
                continue; 
            }

            // Check if enemy has LockDownStatusEffect active
            bool isLockedDown = false;
            foreach (var effect in enemyChar.enemy.activeEffects)
            {
                if (effect is LockDownStatusEffect)
                {
                    isLockedDown = true;
                    break;
                }
            }

            if (isLockedDown)
            {
                continue;
            }

            //if taunted, check if sender is alive, if alive, make the only target

            Targetable sender = enemyChar.enemy.GetComponent<Targetable>();

            //pick a random card
            int randonCard = Random.Range(0, enemyChar.cards.Count);
            CardInformation card = enemyChar.cards[randonCard].GetComponent<CardInformation>();

            //pick a random target
            List<GameObject> targetList = TargetSelector.Instance.GetTargets(card, sender);

            // check for any target with TauntStatusEffect
            GameObject tauntTarget = targetList.Find(t =>
            {
                CharacterInfo info = t.GetComponent<CharacterInfo>();
                return info != null && info.activeEffects.Exists(e => e is TauntStatusEffect);
            });

            GameObject chosenTarget;

            if (tauntTarget != null)
            {
                // force target with TauntStatusEffect
                chosenTarget = tauntTarget;
            }
            else
            {
                // pick a random target as fallback
                int randomTarget = Random.Range(0, targetList.Count);
                chosenTarget = targetList[randomTarget];
            }

            ActionSystem.Instance.AddCard(sender, card, chosenTarget, card.card.mana);
        }
    }
}