using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ClassChoiceDisplay : MonoBehaviour
{
    [SerializeField] private Image classIcon;
    [SerializeField] private TMP_Text className;
    [SerializeField] private Transform cardListHolder;
    [SerializeField] private ClassDataSO classData;
    [SerializeField] private GameObject gameObj;

    public void Setup(ClassDataSO classData)
    {
        this.classData = classData;

        classIcon.sprite = classData.className.Contains("Mage") ? classData.imageFemale : classData.imageMale;

        className.text = classData.className;

        if (cardListHolder.childCount > 1)
        {
            for (int i = cardListHolder.childCount - 1; i > 0; i--)
            {
                Destroy(cardListHolder.GetChild(i).gameObject);
            }
        }

        TMP_Text template = cardListHolder.GetChild(0).GetComponent<TMP_Text>();

        // Count occurrences of each card
        Dictionary<string, int> cardCounts = new Dictionary<string, int>();
        foreach (var card in classData.startingDeck)
        {
            if (cardCounts.ContainsKey(card.cardName))
                cardCounts[card.cardName]++;
            else
                cardCounts[card.cardName] = 1;
        }

        int index = 0;
        foreach (var entry in cardCounts)
        {
            TMP_Text cardText;

            if (index == 0)
            {
                cardText = template;
            }
            else
            {
                GameObject newTextObj = Instantiate(template.gameObject, cardListHolder);
                cardText = newTextObj.GetComponent<TMP_Text>();
            }

            cardText.text = $"Card_{entry.Key} (×{entry.Value})";
            index++;
        }
    }

    public void ButtonAction()
    {
        SelectClass.Instance.SetClassChoice(classData);
    }
}
