using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EquipmentInfoDisplay : MonoBehaviour
{
    [SerializeField] private TMP_Text itemName;
    [SerializeField] private TMP_Text itemSlot;
    [SerializeField] private TMP_Text itemDesc;
    [SerializeField] private TMP_Text itemHP;
    [SerializeField] private TMP_Text itemEN;
    [SerializeField] private TMP_Text itemPWR;
    [SerializeField] private TMP_Text itemSPD;
    [SerializeField] private TMP_Text card1;
    [SerializeField] private TMP_Text card2;
    [SerializeField] private TMP_Text card3;
    [SerializeField] private TMP_Text card4;
    [SerializeField] private EquipmentDataSO equipment;

    public void Setup(EquipmentDataSO newEquipment)
    {
        equipment = newEquipment;
        itemName.text = newEquipment.equipmentName.ToString();
        itemName.ForceMeshUpdate();
        itemDesc.text = newEquipment.description.ToString();
        itemDesc.ForceMeshUpdate();
        itemSlot.text = newEquipment.slotType.ToString();
        itemHP.text = $"HP: {newEquipment.bonusHP.ToString()}";
        itemEN.text = $"EN: {newEquipment.bonusEN.ToString()}";
        itemPWR.text = $"PWR: {newEquipment.bonusPWR.ToString()}";
        itemSPD.text = $"SPD: {newEquipment.bonusSPD.ToString()}";

        SetupCards();
    }

    private void SetupCards()
    {
        // Count cards by name
        Dictionary<string, int> cardCounts = new Dictionary<string, int>();
        foreach (CardDataSO card in equipment.cards)
        {
            if (cardCounts.ContainsKey(card.cardName))
                cardCounts[card.cardName]++;
            else
                cardCounts[card.cardName] = 1;
        }

        // Store the text fields in a list
        List<TMP_Text> cardTextFields = new List<TMP_Text> { card1, card2, card3, card4 };

        int index = 0;
        foreach (var pair in cardCounts)
        {
            if (index >= 4) break;

            string cardLine = $"{pair.Key} - ×{pair.Value}";
            cardTextFields[index].text = cardLine;
            index++;
        }

        // Clear unused fields
        for (; index < 4; index++)
        {
            cardTextFields[index].text = "";
        }
    }
}
