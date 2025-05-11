using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.Splines.Examples;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Splines;

public class CardHolder : MonoBehaviour
{
    [SerializeField] private SplineContainer splineContainer;
    [SerializeField] private GameObject spawnPoint;
    [SerializeField] private GameObject despawnPoint;
    [SerializeField] private GameObject handParent;
    [SerializeField] private GameObject playParent;

    //method for drawing a new hand
    public IEnumerator DrawHandAnimation()
    {
        int childCount = handParent.transform.childCount;
        if (childCount <= 0) yield return null;

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

            //set starting position of card
            child.transform.position = new Vector3(spawnPoint.transform.position.x, spawnPoint.transform.position.y, handParent.transform.position.z);

            //calculate the positions on the spline
            float p = firstCardPos + (i * cardSpacing);
            Vector3 splinePosition = spline.EvaluatePosition(p);
            Vector3 forward = spline.EvaluateTangent(p);
            Vector3 up = spline.EvaluateUpVector(p);
            Quaternion rotation = Quaternion.LookRotation(up, Vector3.Cross(up, forward).normalized);

            // sequence to animate cards moving from deck to hand
            var cardSequence = DOTween.Sequence();
            cardSequence.Append(child.transform.DOLocalMove(splinePosition + ((i * 0.2f) * Vector3.back), 0.1f));
            cardSequence.Join(child.transform.DOLocalRotateQuaternion(rotation, 0.1f));
            cardSequence.Join(child.transform.DOScale(1, 0.1f));

            // Enable collider after animation completes
            if (collider != null)
            {
                cardSequence.OnComplete(() => collider.enabled = true);
            }

            // Append this card's animation to the main sequence with a short delay between cards
            mainSequence.Append(cardSequence);
        }

        yield return mainSequence.WaitForCompletion();
    }

    //method for discarding current hand
    public IEnumerator DiscardHandAnimation()
    {
        int childCount = handParent.transform.childCount;
        Vector3 despawnPos = despawnPoint.transform.position;

        // Main sequence to hold all animations in order
        var mainSequence = DOTween.Sequence();

        //animate cards moving from hand to discard
        for (int i = childCount; i > 0; i--)
        {
            Transform child = handParent.transform.GetChild(i - 1);

            var cardSequence = DOTween.Sequence();
            cardSequence.Append(child.transform.DOMove(despawnPos, 0.1f));
            cardSequence.Join(child.transform.DOScale(Vector3.zero, 0.1f));

            // Append this card's animation to the main sequence with a short delay between cards
            mainSequence.Append(cardSequence);
        }

        yield return mainSequence.WaitForCompletion();
    }

    //method for when using draw cards and adding to hand
    public IEnumerator DrawCardAnimation(CardInformation card)
    {
        GameObject cardObj = card.gameObject;
        Vector3 startZonePos = new Vector3(playParent.transform.position.x, playParent.transform.position.y, cardObj.transform.position.z);
        cardObj.transform.position = new Vector3(spawnPoint.transform.position.x, spawnPoint.transform.position.y, handParent.transform.position.z);

        yield return SortCards();

        cardObj.transform.DOLocalMove(Vector2.zero, 0.1f);
        cardObj.transform.DOScale(Vector3.one, 0.1f);

        yield return new WaitForSeconds(0.1f);

        cardObj.transform.SetParent(handParent.transform, true);
        yield return SortCards();
    }

    //method for when using draw cards and hand is full
    public IEnumerator DrawCardAnimation(CardInformation card, GameObject parent)
    {
        GameObject cardObj = card.gameObject;
        Vector3 startZonePos = new Vector3(playParent.transform.position.x, playParent.transform.position.y, cardObj.transform.position.z);
        Vector3 dropZonePos = new Vector3(spawnPoint.transform.position.x, spawnPoint.transform.position.y, cardObj.transform.position.z);
        cardObj.transform.position = new Vector3(spawnPoint.transform.position.x, spawnPoint.transform.position.y, handParent.transform.position.z);

        yield return SortCards();

        cardObj.transform.DOLocalMove(Vector2.zero, 0.1f);
        cardObj.transform.DOScale(Vector3.one, 0.1f);

        yield return new WaitForSeconds(0.1f);

        var seq = DOTween.Sequence();
        seq.Join(cardObj.transform.DOLocalMove(dropZonePos, 0.1f));
        seq.Join(cardObj.transform.DOScale(Vector3.zero, 0.1f));

        yield return seq.WaitForCompletion();

        cardObj.transform.SetParent(parent.transform, true);
    }

    //method to sort the cards in hand
    public IEnumerator SortCards()
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
            child.GetComponent<SortingGroup>().sortingOrder = 0;

            float p = firstCardPos + (i * cardSpacing);
            Vector3 splinePosition = spline.EvaluatePosition(p);
            Vector3 forward = spline.EvaluateTangent(p);
            Vector3 up = spline.EvaluateUpVector(p);
            Quaternion rotation = Quaternion.LookRotation(up, Vector3.Cross(up, forward).normalized);

            // Create a sub-sequence for this card
            var cardSequence = DOTween.Sequence();
            cardSequence.SetDelay(delay);
            cardSequence.Append(child.transform.DOLocalMove(splinePosition + ((i * 0.2f) * Vector3.back), 0.1f));
            cardSequence.Join(child.transform.DOLocalRotateQuaternion(rotation, 0.1f));
            cardSequence.Join(child.transform.DOScale(1, 0.1f));

            CardInformation card = child.GetComponent<CardInformation>();

            delay += 0.1f;
        }

        yield return new WaitForSeconds(delay);
    }

    public IEnumerator SelectedCardsPosition()
    {
        foreach (Transform child in handParent.transform)
        {
            CardInformation card = child.GetComponent<CardInformation>();
            if (card != null && card.isSelected == true)
            {
                yield return child.DOMove(child.position + new Vector3(0, 0.5f, 0), 0.1f).WaitForCompletion();
            }
        }
    }
}
