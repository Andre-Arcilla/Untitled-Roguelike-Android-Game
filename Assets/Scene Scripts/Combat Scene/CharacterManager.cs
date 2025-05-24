using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterManager : MonoBehaviour
{
    public static CharacterManager Instance;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    [SerializeField] public List<GameObject> characterList;
    [SerializeField] private List<GameObject> allCardViews;

    public void SetCardViews()
    {
        foreach (var character in characterList)
        {
            // assumes "Card View" is always a child
            Transform cardViewTransform = character.transform.Find("Card View");
            if (cardViewTransform != null)
            {
                GameObject cardView = cardViewTransform.gameObject;
                allCardViews.Add(cardView);
            }
        }
    }

    public void DisplayCardView(GameObject targetView)
    {
        foreach (var cardView in allCardViews)
        {
            bool isTarget = cardView == targetView;
            cardView.SetActive(isTarget);

            Transform characterSprite = cardView.transform.parent.Find("Character Sprite");

            //stop ongoing tweens
            characterSprite.DOKill();

            if (isTarget)
            {
                //apply pulse and move character
                characterSprite.DOLocalMove(new Vector3(characterSprite.localPosition.x, 0.3f), 0.25f);
                characterSprite.DOScale(0.8f, 0.5f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutSine);
            }
            else
            {
                //reset pos and size
                characterSprite.DOLocalMove(new Vector3(characterSprite.localPosition.x, 0.15f), 0.25f);
                characterSprite.localScale = new Vector3(0.750f, 0.750f);
            }
        }
    }

    public void DisplayCardView(CharacterInfo characterInfo)
    {
        Transform cardViewTransform = characterInfo.transform.Find("Card View");
        if (cardViewTransform == null) return;

        GameObject targetView = cardViewTransform.gameObject;

        foreach (var cardView in allCardViews)
        {
            bool isTarget = cardView == targetView;
            cardView.SetActive(isTarget);

            Transform characterSprite = cardView.transform.parent.Find("Character Sprite");

            //stop ongoing tweens and reset size and pos
            characterSprite.DOKill();
            characterSprite.DOLocalMove(new Vector3(characterSprite.localPosition.x, 0.15f), 0.25f);
            characterSprite.localScale = new Vector3(0.750f, 0.750f);
            Debug.Log(transform.parent.name);
        }
    }

    public void DisplayCardView()
    {
        foreach (var cardView in allCardViews)
        {
            Transform characterSprite = cardView.transform.parent.Find("Character Sprite");

            //stop ongoing tweens and reset size and pos
            characterSprite.DOKill();
            characterSprite.DOLocalMove(new Vector3(characterSprite.localPosition.x, 0.15f), 0.25f);
            characterSprite.localScale = new Vector3(0.750f, 0.750f);
        }
    }

    public void SelectFirstCharacter()
    {
        foreach (var card in allCardViews)
        {
            if (card.GetComponentInParent<CharacterInfo>().currentHP > 0)
            {
                DisplayCardView(card);
                break;
            }
        }
    }

    public void DeselectCharacters()
    {
        foreach (var cardView in allCardViews)
        {
            cardView.SetActive(false);
        }
    }
}