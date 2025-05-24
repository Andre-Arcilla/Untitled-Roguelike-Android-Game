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
    [SerializeField] private SplineContainer splineContainer1;
    [SerializeField] private SplineContainer splineContainer2;
    [SerializeField] private GameObject handParent;
    [SerializeField] private GameObject playParent;
    [SerializeField] private float animationSpeed = 0.1f;

    //method for drawing a new hand
    public IEnumerator DrawHandAnimation()
    {
        GameObject spawnPoint = GetComponentInParent<CharacterDeck>().deckPos.gameObject;
        GameObject despawnPoint = GetComponentInParent<CharacterDeck>().discardPos.gameObject;
        if (GetComponentInParent<Targetable>().team == Team.Enemy)
        {
            yield break;
        }

        int childCount = handParent.transform.childCount;
        if (childCount <= 0)
        {
            yield break;
        }

        float cardSpacing = 1f / childCount;
        float firstCardPos = 0.5f - (childCount - 1) * cardSpacing / 2;
        Spline spline = splineContainer1.Spline;

        // Main sequence to hold all animations in order
        var mainSequence = DOTween.Sequence();
        mainSequence.SetLink(gameObject).SetAutoKill(true);

        for (int i = 0; i < childCount; i++)
        {
            Transform child = handParent.transform.GetChild(i);
            BoxCollider2D collider = child.GetComponent<BoxCollider2D>();
            if (collider != null) collider.enabled = false;

            //set starting position of card
            child.transform.position = new Vector3(spawnPoint.transform.position.x, spawnPoint.transform.position.y, spawnPoint.transform.position.z);
            child.transform.rotation = Quaternion.identity;
            child.transform.localScale = Vector3.one;
            child.Find("Card Front").gameObject.SetActive(false);
            child.Find("Card Back").gameObject.SetActive(true);

            //calculate the positions on the spline
            float p = firstCardPos + (i * cardSpacing);
            Vector3 splinePosition = spline.EvaluatePosition(p);
            Vector3 forward = spline.EvaluateTangent(p);
            Vector3 up = spline.EvaluateUpVector(p);
            Quaternion rotation = Quaternion.LookRotation(up, Vector3.Cross(up, forward).normalized);

            // sequence to animate cards moving from deck to hand
            var cardSequence = DOTween.Sequence();
            cardSequence.AppendCallback(() =>
            {
                child.GetComponent<SortingGroup>().sortingOrder = 1;
                child.transform.position = new Vector3(spawnPoint.transform.position.x, spawnPoint.transform.position.y, handParent.transform.position.z);
            });
            cardSequence.Append(child.transform.DOLocalRotateQuaternion(Quaternion.Euler(0f, 90f, 0f), 0.15f).SetEase(Ease.InOutQuad));
            cardSequence.AppendCallback(() =>
            {
                child.Find("Card Front").gameObject.SetActive(true);
                child.Find("Card Back").gameObject.SetActive(false);
            });
            cardSequence.Append(child.transform.DOLocalRotateQuaternion(Quaternion.Euler(0f, 0f, 0f), 0.15f).SetEase(Ease.InOutQuad));
            cardSequence.Append(child.transform.DOLocalMove(splinePosition + ((i * 0.2f) * Vector3.back), animationSpeed));
            cardSequence.Join(child.transform.DOLocalRotateQuaternion(rotation, animationSpeed));
            cardSequence.SetLink(gameObject).SetAutoKill(true);
            cardSequence.OnComplete(() => {
                collider.enabled = true;
                child.GetComponent<SortingGroup>().sortingOrder = 0;
            });

            // Append this card's animation to the main sequence with a short delay between cards
            mainSequence.Append(cardSequence);

        }

        yield return mainSequence.WaitForCompletion();
    }

    //method for discarding current hand
    public IEnumerator DiscardHandAnimation()
    {
        GameObject despawnPoint = GetComponentInParent<CharacterDeck>().discardPos.gameObject;
        if (GetComponentInParent<Targetable>().team == Team.Enemy)
        {
            yield break;
        }

        int childCount = handParent.transform.childCount;
        Vector3 despawnPos = despawnPoint.transform.position;

        // Main sequence to hold all animations in order
        var mainSequence = DOTween.Sequence();
        mainSequence.SetLink(gameObject).SetAutoKill(true);

        //animate cards moving from hand to discard
        for (int i = childCount; i > 0; i--)
        {
            Transform child = handParent.transform.GetChild(i - 1);

            var cardSequence = DOTween.Sequence();
            cardSequence.AppendCallback(() =>
            {
                child.GetComponent<SortingGroup>().sortingOrder = 1;
            });
            cardSequence.Append(child.transform.DOMove(new Vector3(despawnPos.x, despawnPos.y, child.transform.position.z), animationSpeed));
            cardSequence.Join(child.transform.DOLocalRotateQuaternion(Quaternion.identity, animationSpeed));
            cardSequence.Append(child.transform.DOLocalRotateQuaternion(Quaternion.Euler(0f, 90f, 0f), 0.15f).SetEase(Ease.InOutQuad));
            cardSequence.AppendCallback(() =>
            {
                child.Find("Card Front").gameObject.SetActive(false);
                child.Find("Card Back").gameObject.SetActive(true);
            });
            cardSequence.Append(child.transform.DOLocalRotateQuaternion(Quaternion.Euler(0f, 0f, 0f), 0.15f).SetEase(Ease.InOutQuad));
            cardSequence.SetLink(gameObject).SetAutoKill(true);
            cardSequence.OnComplete(() => {
                child.transform.position += new Vector3(despawnPos.x, despawnPos.y, despawnPos.z);
                child.GetComponent<SortingGroup>().sortingOrder = 0;
            });

            // Append this card's animation to the main sequence with a short delay between cards
            mainSequence.Append(cardSequence);
        }

        yield return mainSequence.WaitForCompletion();
    }

    //method for when using draw cards and adding to hand
    public IEnumerator DrawCardAnimation(CardInformation card)
    {
        GameObject spawnPoint = GetComponentInParent<CharacterDeck>().deckPos.gameObject;
        if (GetComponentInParent<Targetable>().team == Team.Enemy)
        {
            yield break;
        }

        card.transform.position = new Vector3(spawnPoint.transform.position.x, spawnPoint.transform.position.y, handParent.transform.position.z);
        card.transform.rotation = Quaternion.identity;
        card.transform.localScale = Vector3.one;
        Vector3 dropZone = new Vector3(0, 1, card.transform.position.z);

        yield return SortCards();

        var sequence = DOTween.Sequence();
        sequence.AppendCallback(() =>
        {
            card.GetComponent<SortingGroup>().sortingOrder = 1;
            card.transform.position = new Vector3(spawnPoint.transform.position.x, spawnPoint.transform.position.y, handParent.transform.position.z);
        });
        sequence.Append(card.transform.DOLocalRotateQuaternion(Quaternion.Euler(0f, 90f, 0f), 0.15f).SetEase(Ease.InOutQuad));
        sequence.AppendCallback(() =>
        {
            card.transform.Find("Card Front").gameObject.SetActive(true);
            card.transform.Find("Card Back").gameObject.SetActive(false);
        });
        sequence.Append(card.transform.DOLocalRotateQuaternion(Quaternion.Euler(0f, 0f, 0f), 0.15f).SetEase(Ease.InOutQuad));
        sequence.Append(card.transform.DOMove(dropZone, 0.15f));
        sequence.SetLink(gameObject).SetAutoKill(true);
        sequence.OnComplete(() => {
            card.GetComponent<SortingGroup>().sortingOrder = 0;
        });

        yield return sequence.WaitForCompletion();

        yield return new WaitForSeconds(animationSpeed);

        card.transform.SetParent(handParent.transform, true);
        yield return SortCards();
    }

    //method for when using draw cards and hand is full
    public IEnumerator DrawCardAnimation(CardInformation card, GameObject parent)
    {
        GameObject spawnPoint = GetComponentInParent<CharacterDeck>().deckPos.gameObject;
        GameObject despawnPoint = GetComponentInParent<CharacterDeck>().discardPos.gameObject;
        if (GetComponentInParent<Targetable>().team == Team.Enemy)
        {
            yield break;
        }

        card.transform.position = new Vector3(spawnPoint.transform.position.x, spawnPoint.transform.position.y, handParent.transform.position.z);
        card.transform.rotation = Quaternion.identity;
        card.transform.localScale = Vector3.one;
        Vector3 dropZoneA = new Vector3(0, 1, card.transform.position.z);
        Vector3 dropZoneB = new Vector3(despawnPoint.transform.position.x, despawnPoint.transform.position.y, card.transform.position.z);

        yield return SortCards();

        var sequenceA = DOTween.Sequence();
        sequenceA.AppendCallback(() =>
        {
            card.GetComponent<SortingGroup>().sortingOrder = 1;
            card.transform.position = new Vector3(spawnPoint.transform.position.x, spawnPoint.transform.position.y, handParent.transform.position.z);
        });
        sequenceA.Append(card.transform.DOLocalRotateQuaternion(Quaternion.Euler(0f, 90f, 0f), 0.15f).SetEase(Ease.InOutQuad));
        sequenceA.AppendCallback(() =>
        {
            card.transform.Find("Card Front").gameObject.SetActive(true);
            card.transform.Find("Card Back").gameObject.SetActive(false);
        });
        sequenceA.Append(card.transform.DOLocalRotateQuaternion(Quaternion.Euler(0f, 0f, 0f), 0.15f).SetEase(Ease.InOutQuad));
        sequenceA.Append(card.transform.DOMove(dropZoneA, 0.15f));
        sequenceA.SetLink(gameObject).SetAutoKill(true);
        yield return sequenceA.WaitForCompletion();

        yield return new WaitForSeconds(animationSpeed);

        var sequenceB = DOTween.Sequence();
        sequenceB.Join(card.transform.DOMove(dropZoneB, 0.15f));
        sequenceB.Append(card.transform.DOLocalRotateQuaternion(Quaternion.Euler(0f, 90f, 0f), 0.15f).SetEase(Ease.InOutQuad));
        sequenceB.AppendCallback(() =>
        {
            card.transform.Find("Card Front").gameObject.SetActive(false);
            card.transform.Find("Card Back").gameObject.SetActive(true);
        });
        sequenceB.Append(card.transform.DOLocalRotateQuaternion(Quaternion.Euler(0f, 0f, 0f), 0.15f).SetEase(Ease.InOutQuad));
        sequenceB.OnComplete(() => card.transform.position += new Vector3(0, 0, 1));
        sequenceB.SetLink(gameObject).SetAutoKill(true);
        sequenceB.OnComplete(() => {
            card.GetComponent<SortingGroup>().sortingOrder = 0;
        });
        yield return sequenceB.WaitForCompletion();

        card.transform.SetParent(parent.transform, true);
    }

    //method to sort the cards in hand
    public IEnumerator SortCards()
    {
        yield return new WaitForSeconds(0.1f); // wait for mouse-up logic to finish

        if (GetComponentInParent<Targetable>().team == Team.Enemy)
        {
            yield break;
        }

        int childCount = handParent.transform.childCount;
        if (childCount <= 0)
        {
            yield break;
        }

        float cardSpacing = 1f / childCount;
        float firstCardPos = 0.5f - (childCount - 1) * cardSpacing / 2;
        Spline spline = splineContainer1.Spline;

        for (int i = 0; i < childCount; i++)
        {
            Transform child = handParent.transform.GetChild(i);
            bool isSelected = child.GetComponent<CardInformation>().isSelected;
            BoxCollider2D collider = child.GetComponent<BoxCollider2D>();
            if (collider != null) collider.enabled = false;
            child.GetComponent<SortingGroup>().sortingOrder = 0;

            float p = firstCardPos + (i * cardSpacing);
            Vector3 splinePosition = spline.EvaluatePosition(p);
            float extraYOffset = isSelected ? 0.6f : 0f;
            Vector3 targetPosition = splinePosition + ((i * 0.2f) * Vector3.back) + new Vector3(0, extraYOffset, 0);
            Vector3 forward = spline.EvaluateTangent(p);
            Vector3 up = spline.EvaluateUpVector(p);
            Quaternion rotation = Quaternion.LookRotation(up, Vector3.Cross(up, forward).normalized);

            // Create a sub-sequence for this card
            var cardSequence = DOTween.Sequence();
            cardSequence.Append(child.transform.DOLocalMove(targetPosition, animationSpeed));
            cardSequence.Join(child.transform.DOLocalRotateQuaternion(rotation, animationSpeed));
            cardSequence.SetLink(gameObject).SetAutoKill(true);
            cardSequence.OnComplete(() => collider.enabled = true);
        }
        yield return new WaitForSeconds(animationSpeed);
    }

    //sort cards with ignored card
    public IEnumerator SortCards(CardInformation cardToIgnore)
    {
        yield return new WaitForSeconds(0.01f); // wait for mouse-up logic to finish

        if (GetComponentInParent<Targetable>().team == Team.Enemy)
            yield break;

        List<Transform> validCards = new List<Transform>();
        for (int i = 0; i < handParent.transform.childCount; i++)
        {
            Transform child = handParent.transform.GetChild(i);
            CardInformation info = child.GetComponent<CardInformation>();
            if (info != null && info != cardToIgnore)
                validCards.Add(child);
        }

        int cardCount = validCards.Count;
        if (cardCount <= 0)
            yield break;

        float cardSpacing = 1f / cardCount;
        float firstCardPos = 0.5f - (cardCount - 1) * cardSpacing / 2;
        Spline spline = splineContainer1.Spline;

        for (int i = 0; i < cardCount; i++)
        {
            Transform child = validCards[i];
            CardInformation info = child.GetComponent<CardInformation>();
            bool isSelected = info.isSelected;
            BoxCollider2D collider = child.GetComponent<BoxCollider2D>();
            if (collider != null) collider.enabled = false;
            child.GetComponent<SortingGroup>().sortingOrder = 0;

            float p = firstCardPos + (i * cardSpacing);
            Vector3 splinePosition = spline.EvaluatePosition(p);
            float extraYOffset = isSelected ? 0.6f : 0f;
            Vector3 targetPosition = splinePosition + ((i * 0.2f) * Vector3.back) + new Vector3(0, extraYOffset, 0);
            Vector3 forward = spline.EvaluateTangent(p);
            Vector3 up = spline.EvaluateUpVector(p);
            Quaternion rotation = Quaternion.LookRotation(up, Vector3.Cross(up, forward).normalized);

            // Tween card into position
            var cardSequence = DOTween.Sequence();
            cardSequence.Append(child.transform.DOLocalMove(targetPosition, animationSpeed));
            cardSequence.Join(child.transform.DOLocalRotateQuaternion(rotation, animationSpeed));
            cardSequence.SetLink(gameObject).SetAutoKill(true);
            cardSequence.OnComplete(() => collider.enabled = true);
        }
        yield return new WaitForSeconds(animationSpeed);
    }


    //fan out the highlighted cards
    public IEnumerator FanCardsAction(CardInformation cardToIgnore = null)
    {
        if (GetComponentInParent<Targetable>().team == Team.Enemy)
            yield break;

        // Build list of cards to include
        List<Transform> validCards = new List<Transform>();
        for (int i = 0; i < handParent.transform.childCount; i++)
        {
            Transform child = handParent.transform.GetChild(i);
            CardInformation info = child.GetComponent<CardInformation>();
            if (info != null && info != cardToIgnore)
                validCards.Add(child);
        }

        int validCount = validCards.Count;
        if (validCount <= 0)
            yield break;

        float cardSpacing = 1f / validCount;
        float firstCardPos = 0.5f - (validCount - 1) * cardSpacing / 2;
        Spline spline = splineContainer2.Spline;

        for (int i = 0; i < validCount; i++)
        {
            Transform child = validCards[i];
            CardInformation info = child.GetComponent<CardInformation>();

            bool isSelected = info.isSelected;

            float p = firstCardPos + (i * cardSpacing);
            Vector3 splinePosition = spline.EvaluatePosition(p);
            float extraYOffset = isSelected ? 0.6f : 0f;
            Vector3 targetPosition = splinePosition + ((i * 0.2f) * Vector3.back) + new Vector3(0, extraYOffset, 0);
            Vector3 forward = spline.EvaluateTangent(p);
            Vector3 up = spline.EvaluateUpVector(p);
            Quaternion rotation = Quaternion.LookRotation(up, Vector3.Cross(up, forward).normalized);

            var cardSequence = DOTween.Sequence();
            cardSequence.Append(child.transform.DOMove(targetPosition, animationSpeed));
            cardSequence.Join(child.transform.DOLocalRotateQuaternion(rotation, animationSpeed));
            cardSequence.SetLink(gameObject).SetAutoKill(true);
        }
        yield return new WaitForSeconds(animationSpeed);
    }
}
