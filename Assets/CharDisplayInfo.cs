using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharDisplayInfo : MonoBehaviour
{
    private CharacterData characterData;
    [SerializeField] private TMP_Text charName;
    [SerializeField] private Image charIcon;
    [SerializeField] private TMP_Text charLevelRace;
    [SerializeField] private TMP_Text charHP;
    [SerializeField] private TMP_Text charEN;
    [SerializeField] private TMP_Text charPWR;
    [SerializeField] private TMP_Text charSPD;
    [SerializeField] private TMP_Text charClass1;
    [SerializeField] private TMP_Text charClass2;
    [SerializeField] private TMP_Text charClass3;
    [SerializeField] private TMP_Text charClass4;
    [SerializeField] private ClassDatabase classDatabase;
    [SerializeField] private RaceDatabase raceDatabase;
    [SerializeField] private GameObject gameObj;
    [SerializeField] private TMP_Text buyButtonTxt;
    [SerializeField] private int price;

    public void Setup(CharacterData characterData, GameObject gameObj)
    {
        this.gameObj = gameObj;
        this.characterData = characterData;

        ClassDataSO selectedClass = classDatabase.allClasses.Find(c => c.className == characterData.classes[0]);
        RaceDataSO selectedRace = raceDatabase.allRaces.Find(r => r.raceName == characterData.basicInfo.raceName);
        price = characterData.basicInfo.level * 30;

        charName.text = $"{characterData.basicInfo.characterName}";
        charIcon.sprite = selectedClass.imageMale;
        charLevelRace.text = $"{characterData.basicInfo.level} - {characterData.basicInfo.raceName}";
        charHP.text = $"HP: {characterData.allocatedStats.allocatedHP + selectedRace.HP}";
        charEN.text = $"EN: {characterData.allocatedStats.allocatedEN + selectedRace.EN}";
        charPWR.text = $"PWR: {characterData.allocatedStats.allocatedPWR + selectedRace.PWR}";
        charSPD.text = $"SPD: {characterData.allocatedStats.allocatedSPD + selectedRace.SPD}";
        charClass1.text = characterData.classes.Count > 0 ? characterData.classes[0] : "";
        charClass2.text = characterData.classes.Count > 1 ? characterData.classes[1] : "";
        charClass3.text = characterData.classes.Count > 2 ? characterData.classes[2] : "";
        charClass4.text = characterData.classes.Count > 3 ? characterData.classes[3] : "";
        buyButtonTxt.text = $"RECRUIT ({price.ToString("N0")}g)";
    }
    public void BuyButtonAction()
    {
        if (TavernManager.Instance.parentHolder.childCount <= 0)
        {
            return;
        }

        if (PlayerDataHolder.Instance.partyGold < price)
        {
            return;
        }

        if (PlayerDataHolder.Instance.partyMembers.Count >= 4)
        {
            return;
        }

        if (PlayerDataHolder.Instance.partyMembers.Count < 4)
        {
            PartyMenuManager.Instance.UpdateGoldAmount(-price);
            PartyMenuManager.Instance.RefreshCharacterList();
            PlayerDataHolder.Instance.partyMembers.Add(characterData);
            TavernManager.Instance.RemoveItemFromList(gameObj);

            // Add this line to update the visual display
            TownPartySystem.Instance.UpdatePartyDisplay();
        }
    }
}
