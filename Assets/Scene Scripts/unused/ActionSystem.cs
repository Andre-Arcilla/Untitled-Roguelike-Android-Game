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
        public CardInformation card;
        public Targetable target;

        public Action(CardInformation card, Targetable target)
        {
            this.card = card;
            this.target = target;
        }
    }

    public void AddCard(CardInformation card, Targetable target)
    {
        actions.Add(new Action(card, target));
    }

    public void RemoveCard(CardInformation card)
    {
        int index = actions.FindIndex(a => a.card == card);
        if (index != -1)
        {
            actions.RemoveAt(index);
        }
    }

    //do all actions after ending turn
    //add enemy actions alongside player actions
}
