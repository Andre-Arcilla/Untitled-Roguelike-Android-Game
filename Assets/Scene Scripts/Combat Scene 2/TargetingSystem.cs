using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;
using static UnityEngine.GraphicsBuffer;

public class TargetingSystem : MonoBehaviour
{
    public static TargetingSystem Instance { get; private set; }

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

    [System.Serializable]
    public class TeamContainer
    {
        public List<Targetable> members = new List<Targetable>();
    }

    [Header("Team Configuration")]
    [SerializeField] public TeamContainer allies;
    [SerializeField] public TeamContainer enemies;

    private bool TryGetValidTarget(Vector2 cardPosition, CardInformation card, out Targetable validTarget)
    {
        validTarget = null;
        if (card == null) return false;

        // Get all potential targets based on card type
        List<Targetable> potentialTargets = GetEligibleTargets(card.card.target);

        // Find closest valid target
        foreach (Targetable target in potentialTargets)
        {
            if (target == null) continue;
            if (!target.IsActive)
            {
                Debug.Log("unit is dead");
                continue;
            }

            // Check if the card's position overlaps the target's collider
            if (target.targetCollider != null && target.targetCollider.OverlapPoint(cardPosition))
            {
                validTarget = target;
                return true;
            }
        }

        if (validTarget != null)
        {
            return true;
        }
        return false;
    }

    private List<Targetable> GetEligibleTargets(Target cardTarget)
    {
        if (cardTarget == Target.Ally)
        {
            return allies.members;
        }
        else
        {
            return enemies.members;
        }
    }

    public void AttemptPlayCard(CardInformation card, Vector2 cardPosition)
    {
        Targetable sender = card.GetComponentInParent<Targetable>();

        if (TryGetValidTarget(cardPosition, card, out Targetable target) &&
            card.GetComponentInParent<CharacterInfo>().currentMana >= card.card.mana)
        {
            card.isSelected = true;
            card.NewPos(0.5f);
            card.GetComponentInParent<CharacterInfo>().currentMana -= card.card.mana;
            ActionSystem.Instance.AddCard(sender, card, target);
        }
    }

    public void DeselectCard(CardInformation card)
    {
        card.isSelected = false;
        card.NewPos(-0.5f);
        card.GetComponentInParent<CharacterInfo>().currentMana += card.card.mana;
        ActionSystem.Instance.RemoveCard(card);
    }
}