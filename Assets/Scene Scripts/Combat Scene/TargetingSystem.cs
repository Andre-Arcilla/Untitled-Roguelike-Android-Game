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
    [SerializeField] public GameObject center;

    private bool TryGetValidTarget(Vector2 cardPosition, CardInformation card, out GameObject validTarget)
    {
        validTarget = null;
        if (card == null)
            return false;

        // Get all potential targets based on the card's target type
        List<GameObject> potentialTargets = TargetSelector.Instance.GetTargets(card, card.GetComponentInParent<Targetable>());

        if (potentialTargets == null || potentialTargets.Count == 0)
            return false;

        // Find the closest valid target under the card's position
        foreach (GameObject targetObj in potentialTargets)
        {
            if (targetObj == null) continue;

            Collider2D targetCollider = targetObj.GetComponentInChildren<Collider2D>();
            if (targetCollider != null && targetCollider.OverlapPoint(cardPosition))
            {
                validTarget = targetObj;
                return true;
            }
        }

        return false;
    }


    public void AttemptPlayCard(CardInformation card, Vector2 cardPosition)
    {
        Targetable sender = card.GetComponentInParent<Targetable>();

        if (!TryGetValidTarget(cardPosition, card, out GameObject target)) return;

        CharacterInfo info = card.GetComponentInParent<CharacterInfo>();
        CharacterDeck deck = card.GetComponentInParent<CharacterDeck>();
        if (info.currentEN < card.card.mana) return;

        // Pay the cost
        info.currentEN -= card.card.mana;

        // Check if this is an instant card
        if (card.card.target == Target.Draw)
        {
            card.isUsing = true;
            ActionSystem.Instance.TriggerAction(sender, card);
        }
        else if (card.card.isInstantUse == true)
        {
            card.isUsing = true;
            StartCoroutine(ActionSystem.Instance.TriggerAction(sender, card, target));
        }
        else
        {
            card.isSelected = true;
            card.NewPos(0.5f);
            ActionSystem.Instance.AddCard(sender, card, target);
        }

        sender.GetComponent<CharacterInfo>().UpdateResourcesView();
    }

    public void DeselectCard(CardInformation card)
    {
        card.isSelected = false;
        card.NewPos(-0.5f);
        card.GetComponentInParent<CharacterInfo>().currentEN += card.card.mana;
        ActionSystem.Instance.RemoveCard(card);
        CharacterDeck deck = card.GetComponentInParent<CharacterDeck>();
        card.GetComponentInParent<CharacterInfo>().UpdateResourcesView();
    }
}