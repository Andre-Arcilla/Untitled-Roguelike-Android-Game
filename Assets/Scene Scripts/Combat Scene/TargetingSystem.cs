using System.Collections;
using System.Collections.Generic;
using Unity.Splines.Examples;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.TextCore.Text;
using static Cinemachine.CinemachineTargetGroup;
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
    [SerializeField] public GameObject darkPanel;
    [SerializeField] public List<GameObject> potentialTargets;
    [SerializeField] private List<Color> allyColors;
    [SerializeField] private List<Color> enemyColors;

    public bool TryGetValidTarget(Vector2 cardPosition, CardInformation card, out GameObject validTarget)
    {
        validTarget = null;
        if (card == null)
            return false;

        // Get all potential targets based on the card's target type
        potentialTargets = TargetSelector.Instance.GetTargets(card, card.GetComponentInParent<Targetable>());

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
            if (target != null && target.transform != null && target.gameObject.activeInHierarchy)
            {
                StartCoroutine(DrawLineCoroutine(sender.gameObject, card.gameObject, target));
            }
            else
            {
                Debug.LogWarning("Target is null or inactive!");
            }
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
        DrawLine.Instance.RemoveLineForCard(card.gameObject);
        CharacterDeck deck = card.GetComponentInParent<CharacterDeck>();
        card.GetComponentInParent<CharacterInfo>().UpdateResourcesView();
    }
    private IEnumerator DrawLineCoroutine(GameObject sender, GameObject card, GameObject target)
    {
        yield return new WaitForSeconds(0.1f);

        Vector3 start = card.GetComponentInChildren<Collider2D>().transform.position;
        Transform lineHolder = card.transform.parent.parent.Find("Line Holder");
        Transform hpTransform = target.transform.Find("Character Sprite").Find("Resources").Find("HP Object").Find("HP icon");

        SpriteRenderer sr = hpTransform.GetComponent<SpriteRenderer>();
        Vector3 end;

        if (sr != null)
        {
            end = sr.bounds.center;
        }
        else
        {
            end = hpTransform.position; // Fallback if no SpriteRenderer
        }

        start.y += 1f;
        start.z -= 0.5f;
        end.z += 0.5f;

        Debug.Log(sender);
        Debug.Log(target);

        Color senderColor = GetSenderColor(sender);
        Color targetColor = GetSenderColor(target);

        Debug.Log(senderColor);
        Debug.Log(targetColor);

        DrawLine.Instance.CreateCurvedLine(lineHolder, card, start, end, senderColor, targetColor);
    }

    private Color GetSenderColor(GameObject sender)
    {
        int allyIndex = allies.members.FindIndex(x => x.gameObject == sender);
        if (allyIndex != -1 && allyIndex < allyColors.Count)
        {
            return allyColors[allyIndex];
        }

        int enemyIndex = enemies.members.FindIndex(x => x.gameObject == sender);
        if (enemyIndex != -1 && enemyIndex < enemyColors.Count)
        {
            return enemyColors[enemyIndex];
        }

        return Color.gray;
    }
}