using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterManager : MonoBehaviour
{
    public static CharacterManager Instance;

    // Start is called before the first frame update
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    [SerializeField] private List<GameObject> characterList;
    private List<GameObject> allCardViews = new List<GameObject>();

    private void Start()
    {
        foreach (var character in characterList)
        {
            // assumes "Card View" is always a child
            Transform cardViewTransform = character.transform.Find("Card View");
            if (cardViewTransform != null)
            {
                GameObject cardView = cardViewTransform.gameObject;
                allCardViews.Add(cardView);
                cardView.SetActive(false); // optional: hide all at start
            }
        }
    }

    public void DisplayCardView(GameObject targetView)
    {
        foreach (var cardView in allCardViews)
        {
            cardView.SetActive(cardView == targetView);
        }
    }
}