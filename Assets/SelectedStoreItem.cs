using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SelectedStoreItem : MonoBehaviour
{
    [SerializeField] private Image itemImage;
    [SerializeField] private TMP_Text itemName;
    [SerializeField] private TMP_Text itemDescription;
    [SerializeField] private TMP_Text itemRequirement;
    [SerializeField] private TMP_Text itemHP;
    [SerializeField] private TMP_Text itemEN;
    [SerializeField] private TMP_Text itemPWR;
    [SerializeField] private TMP_Text itemSPD;
    [SerializeField] private TMP_Text itemCardA;
    [SerializeField] private TMP_Text itemCardB;
    [SerializeField] private TMP_Text itemCardC;
    [SerializeField] private TMP_Text itemCardD;
    [SerializeField] private TMP_Text buttonTxt;
    [SerializeField] public EquipmentDataSO equipment;
    [SerializeField] private GameObject currentItemGO;

    public void Setup(EquipmentDataSO newEquipment, GameObject listObject)
    {
        currentItemGO = listObject;
        equipment = newEquipment;
        itemImage.sprite = newEquipment.sprite;
        itemName.text = newEquipment.equipmentName;
        itemDescription.text = newEquipment.description;
        itemRequirement.text = $"{newEquipment.slotType} - {ToReadableString(newEquipment.classType.ToString())}";
        itemHP.text = $"HP:  {newEquipment.bonusHP}";
        itemEN.text = $"HP:  {newEquipment.bonusEN}";
        itemPWR.text = $"HP:  {newEquipment.bonusPWR}";
        itemSPD.text = $"HP:  {newEquipment.bonusSPD}";
        buttonTxt.text = $"BUY ({newEquipment.price.ToString()}g)";

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
        List<TMP_Text> cardTextFields = new List<TMP_Text> { itemCardA, itemCardB, itemCardC, itemCardD };

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

    private string ToReadableString(string input)
    {
        return input.Replace("_", " ");
    }

    public void BuyButtonAction()
    {
        if (PlayerDataHolder.Instance.partyGold < equipment.price)
        {
            return;
        }

        if (InventoryManager.Instance.AddItem(equipment))
        {
            PartyMenuManager.Instance.UpdateGoldAmount(equipment.price);
            PartyMenuManager.Instance.UpdateCharacterItems();
            MerchantManager.Instance.RemoveItemFromList(currentItemGO);
        }
    }
}
