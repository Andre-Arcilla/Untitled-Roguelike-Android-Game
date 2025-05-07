using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class ActionSystem : MonoBehaviour
{
    public static ActionSystem Instance { get; private set; }

    private void Awake()
    {
        // Check if an instance already exists
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // Only one allowed
            return;
        }

        Instance = this;
    }

    [SerializeField] private List<Action> actions = new List<Action>();

    [System.Serializable]
    public struct Action
    {
        public Targetable sender;
        public CardInformation card;
        public GameObject target;

        public Action(Targetable sender, CardInformation card, GameObject target)
        {
            this.sender = sender;
            this.card = card;
            this.target = target;
        }
    }

    public void AddCard(Targetable sender, CardInformation card, GameObject target)
    {
        Debug.Log(card.card.cardName + ", " + card.card.mana);
        actions.Add(new (sender, card, target));
    }

    public void RemoveCard(CardInformation card)
    {
        int index = actions.FindIndex(a => a.card == card);
        if (index != -1)
        {
            actions.RemoveAt(index);
        }
    }

    public void EndTurn()
    {
        SortActions();
        DoActions();
        DoStatusEffects();
    }

    private void SortActions()
    {
        actions.Sort((a, b) =>
        {
            // Handle null cases gracefully
            if (a.card == null || b.card == null) return 0;

            var aCharSPD = a.card.GetComponentInParent<CharacterInfo>().stats.totalSPD;
            var bCharSPD = b.card.GetComponentInParent<CharacterInfo>().stats.totalSPD;

            if (aCharSPD == null || bCharSPD == null) return 0;

            return bCharSPD.CompareTo(aCharSPD); // Descending
        });
    }

    private void DoActions()
    {
        foreach (Action action in actions)
        {
            foreach (ICardEffect effect in action.card.card.effects)
            {
                effect.Execute(action.sender, action.card, action.target);
            }
        }
    }

    private void DoStatusEffects()
    {
        List<Targetable> characters = new List<Targetable>();
        characters.AddRange(TargetingSystem.Instance.allies.members);
        characters.AddRange(TargetingSystem.Instance.enemies.members);

        List<CharacterInfo> infoList = characters
            .Select(c => c.GetComponent<CharacterInfo>())
            .ToList();

        foreach (CharacterInfo character in infoList)
        {
            character.OnTurnStart();
        }

        foreach (CharacterInfo character in infoList)
        {
            character.OnTurnEnd();
        }
    }

    public void TriggerAction(Targetable sender, CardInformation card)
    {
        foreach (ICardEffect effect in card.card.effects)
        {
            effect.Execute(sender, card, null);
        }

        Debug.Log("sender: " + sender.GetComponent<CharacterInfo>().characterData.basicInfo.characterName + "; card: " + card.card.cardName);
    }

    //do all actions after ending turn
    //add enemy actions alongside player actions
}
