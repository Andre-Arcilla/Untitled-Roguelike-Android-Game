using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

public class CardHolder : MonoBehaviour
{
    [SerializeField] SplineContainer splineContainer;
    [SerializeField, Range(1f, 50f)] public float cardsPerSpline = 10f;

    private readonly List<CardSprite> cards = new();

    public IEnumerator AddCard(CardSprite card)
    {
        cards.Add(card);
        yield return UpdateCardPosition(0.15f);
    }

    private IEnumerator UpdateCardPosition(float duration)
    {
        if (cards.Count == 0)
        {
            yield break;
        }

        float cardSpacing = 1f / cardsPerSpline;

        float firstCardPos = 0.5f - (cards.Count - 1) * cardSpacing / 2;

        Spline spline = splineContainer.Spline;

        for (int i = 0; i < cards.Count; i++)
        {
            if (cards[i] == null) continue;

            float p = firstCardPos + i * cardSpacing;
            Vector3 splinePosition = spline.EvaluatePosition(p);
            Vector3 forward = spline.EvaluateTangent(p);
            Vector3 up = spline.EvaluateUpVector(p);
            Quaternion rotation = Quaternion.LookRotation(-up, Vector3.Cross(-up, forward).normalized);

            cards[i].transform.DOMove(splinePosition + transform.position + 0.01f * i * Vector3.back, duration);
            cards[i].transform.DORotate(rotation.eulerAngles, duration);
        }

        yield return new WaitForSeconds(duration);
    }

    public IEnumerator DiscardCardsToPile(Transform discardTarget, float duration = 0.15f)
    {
        if (cards.Count == 0) yield break;

        float delay = 0f;

        for (int i = cards.Count - 1; i >= 0; i--)
        {
            if (cards[i] == null) continue;

            CardSprite cardToDestroy = cards[i];
            Vector3 startPos = cards[i].transform.position;
            Vector3 targetPos = new Vector3(discardTarget.position.x, discardTarget.position.y, startPos.z);

            cardToDestroy.transform.DOMove(targetPos, duration).SetDelay(delay).SetEase(Ease.InOutQuad);
            cardToDestroy.transform.DORotate(Vector3.zero, duration).SetDelay(delay).SetEase(Ease.InOutQuad);
            cardToDestroy.transform.DOScale(Vector3.zero, duration).SetDelay(delay).SetEase(Ease.InOutQuad).OnComplete(() => Destroy(cardToDestroy.gameObject));

            delay += 0.1f;
        }

        cards.Clear();
        yield return new WaitForSeconds(duration + delay);
    }


}
