using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        public Targetable target;

        public Action(Targetable sender, CardInformation card, Targetable target)
        {
            this.sender = sender;
            this.card = card;
            this.target = target;
        }
    }

    public void AddCard(Targetable sender, CardInformation card, Targetable target)
    {
        actions.Add(new Action(sender, card, target));
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

    //do all actions after ending turn
    //add enemy actions alongside player actions
}
