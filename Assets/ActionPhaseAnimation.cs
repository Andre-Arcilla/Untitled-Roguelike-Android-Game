using DG.Tweening;
using System.Collections;
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

    [SerializeField] private int speed;
    [SerializeField] private float animationSpeed;

    //take sender, target, and card to sorting group layer 2
    //show darkPanel
    public void ActionAnimationStart(Targetable sender, CardInformation card, GameObject target)
    {
        (sender.transform.Find("Character Sprite")?.GetComponentInChildren<SortingGroup>() ?? sender.GetComponentInChildren<SortingGroup>()).sortingOrder = 2;
        card.GetComponentInChildren<SortingGroup>().sortingOrder = 2;
        (target.transform.Find("Character Sprite")?.GetComponentInChildren<SortingGroup>() ?? card.GetComponentInChildren<SortingGroup>()).sortingOrder = 2;

    }

    public IEnumerator ActionAnimationPerform(Targetable sender, CardInformation card, GameObject target, string audioSFX)
    {
        // Get necessary components
        Collider2D senderCollider = sender.transform.Find("Character Sprite")?.GetComponentInChildren<Collider2D>() ?? target.GetComponentInChildren<Collider2D>(); //should get the one from character sprite
        Collider2D cardCollider = card.GetComponentInChildren<Collider2D>();
        Collider2D targetCollider = target.transform.Find("Character Sprite")?.GetComponentInChildren<Collider2D>() ?? target.GetComponentInChildren<Collider2D>(); //should get the one from character sprite
        Rigidbody2D cardRB = card.gameObject.GetComponentInChildren<Rigidbody2D>();
        Animator senderController = sender.GetComponentInChildren<Animator>();
        Animator targetController = target.GetComponentInChildren<Animator>();

        // Validate components
        if (senderCollider == null || cardCollider == null || targetCollider == null || cardRB == null)
        {
            Debug.LogWarning("Missing required Collider2D or Rigidbody2D components.");
            yield break;
        }

        //set the sender and target's object
        GameObject senderHolder = senderCollider.transform.parent.gameObject;
        GameObject targetHolder = targetCollider.transform.parent.name == "Character Sprite" ? targetCollider.transform.parent.gameObject : targetCollider.gameObject;

        // Save original parents
        Transform senderParent = senderHolder.transform.parent;
        Transform cardParent = cardCollider.transform.parent;
        Transform targetParent = targetHolder.transform.parent;

        // Move all objects to a common parent to force same Z plane
        Transform sharedParent = TargetingSystem.Instance.darkPanel.transform.parent;
        senderHolder.transform.SetParent(sharedParent);
        cardCollider.transform.SetParent(sharedParent);
        targetHolder.transform.SetParent(sharedParent);

        //save original pos and scale of target/sender
        Vector3 senderOrigPos = senderHolder.transform.position;
        Vector3 targetOrigPos = targetHolder.transform.position;
        Vector3 senderOrigScale = senderHolder.transform.localScale;
        Vector3 targetOrigScale = targetHolder.transform.localScale;

        //pass the senderHolder/targetHolder to targetanimation methods
        yield return ChooseAnimation(senderHolder, targetHolder, sender.gameObject, target);
        senderHolder.transform.DOKill();

        yield return new WaitForSeconds(0.15f);

        // Scale the card and determine target position
        card.transform.localScale = new Vector3(0.6f, 0.6f);
        Vector2 targetPos = targetCollider.transform.position;

        if (senderController != null)
        {
            senderController.SetTrigger("doAttack");
            yield return WaitForAnimationToFinishSimple(senderController, "attack");
        }

        // Move the card toward the target
        while (!cardCollider.IsTouching(targetCollider))
        {
            Vector2 direction = (targetPos - cardRB.position).normalized;
            Vector2 newPos = Vector2.MoveTowards(cardRB.position, targetPos, speed * Time.fixedDeltaTime);
            cardRB.MovePosition(newPos);
            yield return new WaitForFixedUpdate();
        }

        switch (audioSFX)
        {
            case "hit":
                AudioManager.Instance.PlayHitSFX();
                break;
            case "miss":
                AudioManager.Instance.PlayMissSFX();
                break;
            case "kill":
                AudioManager.Instance.PlayKillSFX();
                break;
            default:
                AudioManager.Instance.PlayHitSFX();
                break;
        }

        // Update resources immediately on impact
        sender.GetComponentInParent<CharacterInfo>()?.UpdateResourcesView();
        CharacterInfo charInfo = target.GetComponentInParent<CharacterInfo>();
        if (charInfo != null)
        {
            charInfo.UpdateResourcesView();

            if (charInfo.currentHP <= 0)
            {
                targetController.SetTrigger("doDeath");
            }
        }
        else
        {
            CardInformation cardInfo = target.GetComponent<CardInformation>();
            if (cardInfo != null)
                cardInfo.UpdateCard();
        }

        foreach (var t in TargetingSystem.Instance.enemies.members)
        {
            if (t.TryGetComponent<CharacterInfo>(out var character))
            {
                character.UpdateResourcesView();
            }
        }

        foreach (var t in TargetingSystem.Instance.allies.members)
        {
            if (t.TryGetComponent<CharacterInfo>(out var character))
            {
                character.UpdateResourcesView();
            }
        }

        float knockbackDistance = 1f;
        float shakeStrength = 0.2f;

        // Directions
        Vector2 knockbackDirectionCard = (cardRB.position - (Vector2)targetCollider.transform.position).normalized;
        Vector2 knockbackDirectionTarget = ((Vector2)targetCollider.transform.position - cardRB.position).normalized;

        //shake and knockback effects
        var sequence = DOTween.Sequence();
        sequence.Append(cardRB.transform.DOMove((Vector2)cardRB.transform.position + knockbackDirectionCard * knockbackDistance, animationSpeed).SetEase(Ease.OutQuad));
        sequence.Join(cardRB.transform.DOShakeRotation(animationSpeed, shakeStrength, vibrato: 10, randomness: 90, fadeOut: true));
        sequence.Join(cardRB.transform.DOShakeScale(animationSpeed, shakeStrength, vibrato: 10, randomness: 90, fadeOut: true));

        if (targetCollider.GetComponent<CardInformation>() == null)
        {
            sequence.Join(targetHolder.transform.DOMove((Vector2)targetHolder.transform.position + knockbackDirectionTarget * knockbackDistance, animationSpeed).SetEase(Ease.OutQuad));
        }
        sequence.Join(targetHolder.transform.DOShakeRotation(animationSpeed, shakeStrength, vibrato: 10, randomness: 90, fadeOut: true));
        sequence.Join(targetHolder.transform.DOShakeScale(animationSpeed, shakeStrength, vibrato: 10, randomness: 90, fadeOut: true));

        yield return sequence.WaitForCompletion();

        //restore changes made to position and scale of sender and target

        sequence = DOTween.Sequence();
        if (targetCollider.GetComponent<CardInformation>() == null)
        {
            sequence.Append(senderHolder.transform.DOLocalMove(senderOrigPos, animationSpeed));
            sequence.Join(senderHolder.transform.DOScale(senderOrigScale, animationSpeed));
            sequence.Join(targetHolder.transform.DOLocalMove(targetOrigPos, animationSpeed));
            sequence.Join(targetHolder.transform.DOScale(targetOrigScale, animationSpeed));
        }
        else
        {
            sequence.Append(senderHolder.transform.DOLocalMove(senderOrigPos, animationSpeed));
            sequence.Join(senderHolder.transform.DOScale(senderOrigScale, animationSpeed));
            sequence.Append(targetHolder.transform.DOLocalMove(senderHolder.transform.position, animationSpeed));
            sequence.Join(targetHolder.transform.DOScale(Vector3.zero, animationSpeed));
        }

        yield return sequence.WaitForCompletion();

        //restore original parent hierarchy
        senderHolder.transform.SetParent(senderParent);
        cardCollider.transform.SetParent(cardParent);
        targetHolder.transform.SetParent(targetParent);

        if (targetCollider.GetComponent<CardInformation>() != null)
        {
            target.transform.localScale = Vector3.one;
            target.transform.Find("Card Front").gameObject.SetActive(false);
            target.transform.Find("Card Back").gameObject.SetActive(true);
            target.GetComponentInChildren<SortingGroup>().sortingOrder = 0;
        }

        yield return null;
    }

    public void ActionAnimationEnd(Targetable sender, CardInformation card, GameObject target)
    {
        (sender.transform.Find("Character Sprite")?.GetComponentInChildren<SortingGroup>() ?? sender.GetComponentInChildren<SortingGroup>()).sortingOrder = 0;
        card.GetComponentInChildren<SortingGroup>().sortingOrder = 0;
        (target.transform.Find("Character Sprite")?.GetComponentInChildren<SortingGroup>() ?? card.GetComponentInChildren<SortingGroup>()).sortingOrder = 0;
    }

    //make a new method that just determines which animation to use
    private IEnumerator ChooseAnimation(GameObject senderObj, GameObject targetObj, GameObject sender, GameObject target)
    {
        Targetable senderInfo = sender.GetComponent<Targetable>();
        Targetable targetInfo = target.GetComponent<Targetable>();
        CardInformation targetCard = targetInfo == null ? target.GetComponent<CardInformation>() : null;

        if (targetInfo == null && targetCard != null)
        {
            yield return CardTargetAnimation(senderObj, targetObj, senderInfo, targetCard);
        }
        else if (senderInfo == targetInfo)
        {
            yield return SelfTargetAnimation(senderObj, targetObj, senderInfo, targetInfo);
        }
        else if (senderInfo.team != targetInfo.team)
        {
            yield return EnemyTargetAnimation(senderObj, targetObj, senderInfo, targetInfo);
        }
        else if (senderInfo.team == targetInfo.team)
        {
            yield return AllyTargetAnimation(senderObj, targetObj, senderInfo, targetInfo);
        }
    }

    private IEnumerator WaitForAnimationToFinishSimple(Animator animator, string stateName)
    {
        // Wait until the animator is playing the desired state
        while (!animator.GetCurrentAnimatorStateInfo(0).IsName(stateName))
            yield return null;

        // Then wait until it's done (normalizedTime >= 1)
        while (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 0.55f)
            yield return null;
    }

    //positions depending on sender and target
    private IEnumerator CardTargetAnimation(GameObject senderObj, GameObject targetObj, Targetable sender, CardInformation target)
    {
        if (sender == null)
        {
            yield break;
        }

        target.transform.localScale = Vector3.zero;
        target.transform.localPosition = senderObj.transform.position;
        target.transform.Find("Card Front").gameObject.SetActive(true);
        target.transform.Find("Card Back").gameObject.SetActive(false);
        target.GetComponentInChildren<SortingGroup>().sortingOrder = 2;

        //checks for sender's team
        if (sender.team == Team.Player)
        {
            var sequence = DOTween.Sequence();
            sequence.Append(senderObj.transform.DOMove(new Vector3(-6f, 1f), animationSpeed));
            sequence.Join(senderObj.transform.DOScale(new Vector3(2.5f, 2.5f), animationSpeed));

            sequence.Join(targetObj.transform.DOMove(new Vector3(-3f, 1f), animationSpeed));
            sequence.Join(targetObj.transform.DOScale(new Vector3(1f, 1f), animationSpeed));
            sequence.SetLink(gameObject).SetAutoKill(true);

            yield return sequence.WaitForCompletion();
        }

        //checks for sender's team
        if (sender.team == Team.Enemy)
        {
            var sequence = DOTween.Sequence();
            sequence.Append(senderObj.transform.DOMove(new Vector3(6f, 1f), animationSpeed));
            sequence.Join(senderObj.transform.DOScale(new Vector3(2.5f, 2.5f), animationSpeed));

            sequence.Join(targetObj.transform.DOMove(new Vector3(3.5f, 1f), animationSpeed));
            sequence.Join(targetObj.transform.DOScale(new Vector3(0.85f, 0.85f), animationSpeed));
            sequence.SetLink(gameObject).SetAutoKill(true);

            yield return sequence.WaitForCompletion();
        }
    }

    private IEnumerator SelfTargetAnimation(GameObject senderObj, GameObject targetObj, Targetable sender, Targetable target)
    {
        if (sender == null)
        {
            yield break;
        }

        //checks for sender's team
        if (sender.team == Team.Player)
        {
            var sequence = DOTween.Sequence();
            sequence.Append(targetObj.transform.DOMove(new Vector3(-4.5f, 1f), animationSpeed));
            sequence.Join(targetObj.transform.DOScale(new Vector3(2.5f, 2.5f), animationSpeed));

            sequence.SetLink(gameObject).SetAutoKill(true);

            yield return sequence.WaitForCompletion();
        }

        //checks for sender's team
        if (sender.team == Team.Enemy)
        {
            var sequence = DOTween.Sequence();
            sequence.Append(targetObj.transform.DOMove(new Vector3(4.5f, 1f), animationSpeed));
            sequence.Join(targetObj.transform.DOScale(new Vector3(2.5f, 2.5f), animationSpeed));

            sequence.SetLink(gameObject).SetAutoKill(true);

            yield return sequence.WaitForCompletion();
        }
    }

    private IEnumerator EnemyTargetAnimation(GameObject senderObj, GameObject targetObj, Targetable sender, Targetable target)
    {
        if (sender == null)
        {
            yield break;
        }

        //checks for sender's team
        if (sender.team == Team.Player)
        {
            var sequence = DOTween.Sequence();
            sequence.Append(senderObj.transform.DOMove(new Vector3(-4.5f, 1f), animationSpeed));
            sequence.Join(senderObj.transform.DOScale(new Vector3(2.5f, 2.5f), animationSpeed));
            sequence.Join(targetObj.transform.DOMove(new Vector3(4.5f, 1f), animationSpeed));
            sequence.Join(targetObj.transform.DOScale(new Vector3(2.5f, 2.5f), animationSpeed));
            sequence.SetLink(gameObject).SetAutoKill(true);

            yield return sequence.WaitForCompletion();
        }

        //checks for sender's team
        if (sender.team == Team.Enemy)
        {
            var sequence = DOTween.Sequence();
            sequence.Append(senderObj.transform.DOMove(new Vector3(4.5f, 1f), animationSpeed));
            sequence.Join(senderObj.transform.DOScale(new Vector3(2.5f, 2.5f), animationSpeed));
            sequence.Join(targetObj.transform.DOMove(new Vector3(-4.5f, 1f), animationSpeed));
            sequence.Join(targetObj.transform.DOScale(new Vector3(2.5f, 2.5f), animationSpeed));
            sequence.SetLink(gameObject).SetAutoKill(true);

            yield return sequence.WaitForCompletion();
        }
    }

    private IEnumerator AllyTargetAnimation(GameObject senderObj, GameObject targetObj, Targetable sender, Targetable target)
    {
        if (sender == null)
        {
            yield break;
        }

        //checks for sender's team
        if (sender.team == Team.Player)
        {
            var sequence = DOTween.Sequence();
            sequence.Append(senderObj.transform.DOMove(new Vector3(-6.5f, 1f), animationSpeed));
            sequence.Join(senderObj.transform.DOScale(new Vector3(2.5f, 2.5f), animationSpeed));

            sequence.Join(targetObj.transform.DOMove(new Vector3(-3.5f, 3f), animationSpeed));
            sequence.Join(targetObj.transform.DOScale(new Vector3(2.5f, 2.5f), animationSpeed));

            sequence.SetLink(gameObject).SetAutoKill(true);

            yield return sequence.WaitForCompletion();
        }

        //checks for sender's team
        if (sender.team == Team.Enemy)
        {
            var sequence = DOTween.Sequence();
            sequence.Append(senderObj.transform.DOMove(new Vector3(6.5f, 1f), animationSpeed));
            sequence.Join(senderObj.transform.DOScale(new Vector3(2.5f, 2.5f), animationSpeed));

            sequence.Join(targetObj.transform.DOMove(new Vector3(3.5f, 2.5f), animationSpeed));
            sequence.Join(targetObj.transform.DOScale(new Vector3(2.5f, 2.5f), animationSpeed));

            sequence.SetLink(gameObject).SetAutoKill(true);

            yield return sequence.WaitForCompletion();
        }
    }
}
