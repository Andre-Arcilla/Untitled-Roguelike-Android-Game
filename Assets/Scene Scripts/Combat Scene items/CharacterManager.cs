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

        allCardViews[0].SetActive(true);
    }

    public void DisplayCardView(GameObject targetView)
    {
        foreach (var cardView in allCardViews)
        {
            cardView.SetActive(cardView == targetView);
        }
    }
}