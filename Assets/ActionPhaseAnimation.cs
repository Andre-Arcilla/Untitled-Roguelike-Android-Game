using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class ActionPhaseAnimation : MonoBehaviour
{
    public static ActionPhaseAnimation Instance { get; private set; }

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

    //take sender, target, and card to sorting group layer 2
    //show darkPanel
    public void ActionAnimationStart(Targetable sender, CardInformation card, GameObject target)
    {
        TargetingSystem.Instance.darkPanel.SetActive(true);
        sender.GetComponentInChildren<SortingGroup>().sortingOrder = 2;
        target.GetComponentInChildren<SortingGroup>().sortingOrder = 2;
        card.GetComponentInChildren<SortingGroup>().sortingOrder = 2;
    }

    public IEnumerator ActionAnimationPerform(Targetable sender, CardInformation card, GameObject target)
    {
        float speed = 5f; // Units per second
        float stopDistance = 0.1f;


        Collider2D cardCollider = card.gameObject.GetComponentInChildren<Collider2D>();
        Collider2D targetCollider = target.GetComponentInChildren<Collider2D>();

        if (cardCollider == null || targetCollider == null)
        {
            Debug.LogWarning("Missing Collider2D on card or target.");
            yield break;
        }

        card.gameObject.transform.localScale = new Vector3(0.6f, 0.6f);

        // Start tweening
        Tween moveTween = card.gameObject.transform.DOMove(target.transform.position, 1f).SetSpeedBased(true).SetEase(Ease.Linear);

        // Wait until touching
        while (!cardCollider.IsTouching(targetCollider))
        {
            yield return null;
        }

        // Stop tween when contact is made
        moveTween.Kill();

        // Optional: Trigger hit effect or cleanup
        Debug.Log("Card hit the target!");
    }

    public void ActionAnimationEnd(Targetable sender, CardInformation card, GameObject target)
    {
        TargetingSystem.Instance.darkPanel.SetActive(false);
        sender.GetComponentInChildren<SortingGroup>().sortingOrder = 0;
        target.GetComponentInChildren<SortingGroup>().sortingOrder = 0;
        card.GetComponentInChildren<SortingGroup>().sortingOrder = 0;
    }
}
