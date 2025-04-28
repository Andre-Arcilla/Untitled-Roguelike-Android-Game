using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

public class CardHolder : MonoBehaviour
{
    [SerializeField] SplineContainer splineContainer;
    [SerializeField, Range(1f, 50f)] public float cardsPerSpline = 10f;

    private readonly List<CardInformation> cards = new();

    public IEnumerator AddCards(Transform drawTarget, List<CardInformation> newCards)
    {
        cards.AddRange(newCards);
        yield return UpdateCardPosition(drawTarget, 0.15f);
    }

    private IEnumerator UpdateCardPosition(Transform drawTarget, float duration)
    {
        if (cards.Count == 0) yield break;

        float cardSpacing = 1f / cardsPerSpline;
        float firstCardPos = 0.5f - (cards.Count - 1) * cardSpacing / 2;
        Spline spline = splineContainer.Spline;
        float delay = 0f;

        for (int i = 0; i < cards.Count; i++)
        {
            if (cards[i] == null) continue;

            float p = firstCardPos + i * cardSpacing;
            Vector3 splinePosition = spline.EvaluatePosition(p);
            Vector3 forward = spline.EvaluateTangent(p);
            Vector3 up = spline.EvaluateUpVector(p);
            Quaternion rotation = Quaternion.LookRotation(-up, Vector3.Cross(-up, forward).normalized);
            Vector3 targetPos = new(drawTarget.position.x, drawTarget.position.y);

            cards[i].transform.position = targetPos;
            BoxCollider2D collider = cards[i].gameObject.GetComponent<BoxCollider2D>();
            if (collider != null) collider.enabled = false; // Disable at start

            // Create a sequence for all animations
            Sequence cardSequence = DOTween.Sequence();
            cardSequence
                .SetDelay(delay)
                .Append(cards[i].transform.DOMove(splinePosition + transform.position + 0.01f * i * Vector3.back, duration).SetEase(Ease.InOutQuad))
                .Join(cards[i].transform.DORotate(rotation.eulerAngles, duration).SetEase(Ease.InOutQuad))
                .Join(cards[i].transform.DOScale(Vector3.one, duration).SetEase(Ease.InOutQuad))
                .OnComplete(() => {
                    if (collider != null) collider.enabled = true;
                    Debug.Log($"Card {i} animations complete, collider enabled");
                });

            delay += 0.1f;
        }

        yield return new WaitForSeconds(duration);
    }

    public IEnumerator DiscardCardsToPile(Transform discardTarget, float duration = 0.15f)
    {
        if (cards.Count == 0) yield break;

        float delay = 0f;
        var cardsCopy = new List<CardInformation>(cards); // Create a safe copy

        // Disable all colliders first
        foreach (var card in cardsCopy)
        {
            if (card != null && card.TryGetComponent<BoxCollider2D>(out var collider))
                collider.enabled = false;
        }

        // Animate each card
        for (int i = cardsCopy.Count - 1; i >= 0; i--)
        {
            if (cardsCopy[i] == null) continue;

            int index = i; // Capture current index for closure
            Vector3 targetPos = discardTarget.position;

            Sequence s = DOTween.Sequence()
                .SetDelay(delay)
                .Append(cardsCopy[index].transform.DOMove(targetPos, duration))
                .Join(cardsCopy[index].transform.DORotate(Vector3.zero, duration))
                .Join(cardsCopy[index].transform.DOScale(Vector3.zero, duration))
                .OnComplete(() => {
                    // Safely re-enable collider on the actual card (not from list)
                    if (cardsCopy[index] != null && cardsCopy[index].TryGetComponent<BoxCollider2D>(out var col))
                        col.enabled = true;
                });

            delay += 0.1f;
        }

        // Wait for animations then clear
        yield return new WaitForSeconds(duration + delay);
        cards.Clear(); // Now safe to clear
    }
}
