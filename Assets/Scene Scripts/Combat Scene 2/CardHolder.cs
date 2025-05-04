using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.Splines.Examples;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Splines;

public class CardHolder : MonoBehaviour
{
    [SerializeField] private SplineContainer splineContainer;
    [SerializeField] private GameObject spawnPoint;
    [SerializeField] private GameObject handParent;
    [SerializeField] private GameObject playParent;

    public void DrawHandAnimation()
    {
        int childCount = handParent.transform.childCount;
        if (childCount <= 0) return;

        float cardSpacing = 1f / childCount;
        float firstCardPos = 0.5f - (childCount - 1) * cardSpacing / 2;
        Spline spline = splineContainer.Spline;

        // Main sequence to hold all animations in order
        var mainSequence = DOTween.Sequence();

        for (int i = 0; i < childCount; i++)
        {
            Transform child = handParent.transform.GetChild(i);
            BoxCollider2D collider = child.GetComponent<BoxCollider2D>();
            if (collider != null) collider.enabled = false;

            child.transform.position = new Vector3(spawnPoint.transform.position.x, spawnPoint.transform.position.y, handParent.transform.position.z);

            float p = firstCardPos + (i * cardSpacing);
            Vector3 splinePosition = spline.EvaluatePosition(p);
            Vector3 forward = spline.EvaluateTangent(p);
            Vector3 up = spline.EvaluateUpVector(p);
            Quaternion rotation = Quaternion.LookRotation(up, Vector3.Cross(up, forward).normalized);

            // Create a sub-sequence for this card
            var cardSequence = DOTween.Sequence();
            cardSequence.Append(child.transform.DOLocalMove(splinePosition + ((i * 0.2f) * Vector3.back), 0.25f));
            cardSequence.Join(child.transform.DOLocalRotateQuaternion(rotation, 0.25f));
            cardSequence.Join(child.transform.DOScale(1, 0.25f));

            // Enable collider after animation completes
            if (collider != null)
            {
                cardSequence.OnComplete(() => collider.enabled = true);
            }

            // Append this card's animation to the main sequence with a short delay between cards
            mainSequence.Append(cardSequence);
        }
    }

    public void DisplayCard()
    {
        playParent.transform.GetChild(0);
    }

    public IEnumerator DrawCardAnimation(CardInformation card)
    {
        CardShowInfo.Instance.Hide();
        GameObject cardObj = card.gameObject;
        Vector3 startZonePos = new Vector3(playParent.transform.position.x, playParent.transform.position.y, cardObj.transform.position.z);
        cardObj.transform.position = new Vector3(spawnPoint.transform.position.x, spawnPoint.transform.position.y, handParent.transform.position.z);

        yield return SortCards();

        cardObj.transform.DOLocalMove(Vector2.zero, 0.25f);
        cardObj.transform.DOScale(Vector3.one, 0.25f);

        yield return new WaitForSeconds(0.5f);

        cardObj.transform.SetParent(handParent.transform, true);
        yield return SortCards();
    }

    public IEnumerator DrawCardAnimation(CardInformation card, GameObject parent)
    {
        CardShowInfo.Instance.Hide();
        GameObject cardObj = card.gameObject;
        Vector3 startZonePos = new Vector3(playParent.transform.position.x, playParent.transform.position.y, cardObj.transform.position.z);
        Vector3 dropZonePos = new Vector3(spawnPoint.transform.position.x, spawnPoint.transform.position.y, cardObj.transform.position.z);
        cardObj.transform.position = new Vector3(spawnPoint.transform.position.x, spawnPoint.transform.position.y, handParent.transform.position.z);

        yield return SortCards();

        cardObj.transform.DOLocalMove(Vector2.zero, 0.25f);
        cardObj.transform.DOScale(Vector3.one, 0.25f);

        yield return new WaitForSeconds(0.5f);

        var seq = DOTween.Sequence();
        seq.Join(cardObj.transform.DOLocalMove(dropZonePos, 0.25f));
        seq.Join(cardObj.transform.DOScale(Vector3.zero, 0.25f));

        yield return seq.WaitForCompletion();

        cardObj.transform.SetParent(parent.transform, true);
    }

    private IEnumerator SortCards()
    {
        int childCount = handParent.transform.childCount;
        if (childCount <= 0) yield return null;

        float cardSpacing = 1f / childCount;
        float firstCardPos = 0.5f - (childCount - 1) * cardSpacing / 2;
        Spline spline = splineContainer.Spline;
        float delay = 0f;

        for (int i = 0; i < childCount; i++)
        {
            Transform child = handParent.transform.GetChild(i);
            BoxCollider2D collider = child.GetComponent<BoxCollider2D>();
            if (collider != null) collider.enabled = false;

            float p = firstCardPos + (i * cardSpacing);
            Vector3 splinePosition = spline.EvaluatePosition(p);
            Vector3 forward = spline.EvaluateTangent(p);
            Vector3 up = spline.EvaluateUpVector(p);
            Quaternion rotation = Quaternion.LookRotation(up, Vector3.Cross(up, forward).normalized);

            // Create a sub-sequence for this card
            var cardSequence = DOTween.Sequence();
            cardSequence.SetDelay(delay);
            cardSequence.Append(child.transform.DOLocalMove(splinePosition + ((i * 0.2f) * Vector3.back), 0.25f));
            cardSequence.Join(child.transform.DOLocalRotateQuaternion(rotation, 0.25f));
            cardSequence.Join(child.transform.DOScale(1, 0.25f));
            cardSequence.OnComplete(() => collider.enabled = true);

            delay += 0.1f;
        }

        yield return new WaitForSeconds(delay);
    }
}
