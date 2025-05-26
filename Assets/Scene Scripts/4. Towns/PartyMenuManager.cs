using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PartyMenuManager : MonoBehaviour
{
    public static PartyMenuManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    [System.Serializable]
    public class CharacterButton
    {
        public Button button;
        public TMP_Text text;
        public CharacterData charData;
    }
    [System.Serializable]
    public class PanelButton
    {
        public Button button;
        public GameObject panel;
    }

    [Header("Character General Information")]
    [SerializeField] private Image charSpriteImage;
    [SerializeField] private TMP_Text charNameTxt;
    [SerializeField] private TMP_Text charRaceTxt;
    [SerializeField] private TMP_Text charLevelTxt;
    [SerializeField] private TMP_Text charENTxt;
    [SerializeField] private TMP_Text charHPTxt;

    [Header("Character Stats Panel")]
    [SerializeField] private TMP_Text charHPStatTxt;
    [SerializeField] private TMP_Text charENStatTxt;
    [SerializeField] private TMP_Text charPWRStatTxt;
    [SerializeField] private TMP_Text charSPDStatTxt;

    [Header("Character Buttons")]
    [SerializeField] private List<CharacterButton> buttonList = new List<CharacterButton>();

    [Header("Panel Buttons")]
    [SerializeField] private List<PanelButton> panelList = new List<PanelButton>();

    [Header("Others")]
    [SerializeField] private Color selectedButton;
    [SerializeField] private Color listColorA;
    [SerializeField] private Color listColorB;
    [SerializeField] private GameObject listObjPrefab;
    [SerializeField] private Transform classHolder;
    [SerializeField] private Transform cardHolder;
    [SerializeField] private List<CharacterData> characterList;
    [SerializeField] private TMP_Text goldTxt1;
    [SerializeField] private TMP_Text goldTxt2;

    [Header("Current Character")]
    [SerializeField] private CharacterData currentCharacter;
    [SerializeField] private Dictionary<CardDataSO, int> deck;
    [SerializeField] private int HP;
    [SerializeField] private int EN;

    [Header("Databases")]
    [SerializeField] private ClassDatabase _classDatabase;
    [SerializeField] private RaceDatabase _raceDatabase;
    [SerializeField] private EquipmentDatabase _equipmentDatabase;

    public ClassDatabase classDatabase => _classDatabase;
    public RaceDatabase raceDatabase => _raceDatabase;
    public EquipmentDatabase equipmentDatabase => _equipmentDatabase;

    private void Start()
    {
        currentCharacter = new CharacterData();
        deck = new Dictionary<CardDataSO, int>();

        characterList = PlayerDataHolder.Instance.partyMembers;

        SetCharacterButtons();

        //sets the displayed information to be 1st character and their deck
        SelectedCharacter(buttonList[0]);
        SelectedPanel(panelList[0]);
        SetInventory();
        UpdateGoldAmount(0);
    }

    // == BUTTON METHODS ======================================================

    //button method characters
    public void CharacterButtonAction()
    {
        GameObject sender = EventSystem.current.currentSelectedGameObject;
        if (sender == null)
        {
            return;
        }

        CharacterButton selectedBtn = buttonList.FirstOrDefault(btn => btn.button.gameObject == sender);

        if (selectedBtn == null || selectedBtn.charData == currentCharacter)
        {
            return;
        }

        SelectedCharacter(selectedBtn);
    }

    //displays the selected character's data
    private void SelectedCharacter(CharacterButton characterButton)
    {
        foreach (CharacterButton charButton in buttonList)
        {
            ColorBlock cb = charButton.button.colors;
            cb.selectedColor = selectedButton;
            if (charButton == characterButton)
            {
                charButton.button.image.color = cb.selectedColor;
                currentCharacter = characterButton.charData;
                SetCharacterInfo();
            }
            else
            {
                charButton.button.image.color = cb.normalColor;
            }
        }
    }

    //button method for panels
    public void PanelButtonAction()
    {
        GameObject sender = EventSystem.current.currentSelectedGameObject;
        if (sender == null)
        {
            return;
        }

        PanelButton selectedPanel = panelList.FirstOrDefault(panel => panel.button.gameObject == sender);

        if (selectedPanel == null)
        {
            return;
        }

        SelectedPanel(selectedPanel);
    }

    //displays the selected panel data
    private void SelectedPanel(PanelButton panelButton)
    {
        foreach (PanelButton panelBtn in panelList)
        {
            ColorBlock cb = panelBtn.button.colors;
            cb.selectedColor = selectedButton;
            if (panelBtn == panelButton)
            {
                panelBtn.button.image.color = cb.selectedColor;
                panelBtn.panel.SetActive(true);
            }
            else
            {
                panelBtn.button.image.color = cb.normalColor;
                panelBtn.panel.SetActive(false);
            }
        }
    }

    // == DISPLAY METHODS =====================================================

    //generates equipment in player inventory
    private void SetInventory()
    {
        List<string> tempInventory = new List<string>();

        foreach (string itemString in PlayerDataHolder.Instance.partyInventory)
        {
            EquipmentDataSO selectedEquipment = equipmentDatabase.allEquipments.Find(e => e.equipmentName == itemString);

            if (selectedEquipment == null)
            {
                Debug.LogWarning($"No match found for: {itemString}");
                continue;
            }

            InventoryManager.Instance.AddItem(selectedEquipment);
            tempInventory.Add(selectedEquipment.equipmentName);
        }

        PlayerDataHolder.Instance.partyInventory.Clear();
        PlayerDataHolder.Instance.partyInventory.AddRange(tempInventory);
    }

    //generates the character buttons based on the characterList
    private void SetCharacterButtons()
    {
        for (int i = 0; i < buttonList.Count; i++)
        {
            if (i < characterList.Count && characterList[i] != null)
            {
                buttonList[i].button.interactable = true;
                buttonList[i].text.text = characterList[i].basicInfo.characterName.ToString();
                buttonList[i].charData = characterList[i];
            }
            else
            {
                buttonList[i].button.interactable = false;
                buttonList[i].text.text = "";
                buttonList[i].charData = null;
            }
        }
    }

    //changes the shown character info based on the selected characterButton
    private void SetCharacterInfo()
    {
        deck.Clear();

        //make sure classHolder is empty
        foreach (Transform child in classHolder)
        {
            Destroy(child.gameObject);
        }
        //make sure cardHolder is empty
        foreach (Transform child in cardHolder)
        {
            Destroy(child.gameObject);
        }
        //make sure equipment slot is empty
        InventoryManager.Instance.ClearEquipmentSlots();

        //loops through all the classes
        for (int i = 0; i < currentCharacter.classes.Count; i++)
        {
            ClassDataSO selectedClass = classDatabase.allClasses.Find(c => c.className == currentCharacter.classes[i]);

            if (selectedClass == null)
            {
                continue;
            }

            //sets the character sprite
            if (i == 0)
            {
                string playerGender = currentCharacter.basicInfo.gender.ToLower();
                charSpriteImage.sprite = SetCharacterSprite(selectedClass, playerGender);
            }

            //generates a text for the class
            GameObject classObj = Instantiate(listObjPrefab, classHolder);
            classObj.name = currentCharacter.classes[i].ToString();
            classObj.GetComponentInChildren<TextMeshProUGUI>().text = currentCharacter.classes[i].ToString();

            //alternating colors for each class text
            if (i % 2 == 1)
            {
                classObj.GetComponent<Image>().color = listColorA;
            }
            else
            {
                classObj.GetComponent<Image>().color = listColorB;
            }

            foreach (CardDataSO card in selectedClass.startingDeck)
            {
                if (deck.ContainsKey(card))
                {
                    deck[card]++;
                }
                else
                {
                    deck[card] = 1;
                }
            }
        }

        //generate character specific things
        GenerateCards();
        SetCharacterStats();
        GenerateEquipment();

        //input text on screen
        charNameTxt.text = currentCharacter.basicInfo.characterName.ToString();
        charRaceTxt.text = currentCharacter.basicInfo.raceName.ToString();
        charLevelTxt.text = currentCharacter.basicInfo.level.ToString();
        charHPTxt.text = "×" + HP.ToString();
        charENTxt.text = "×" + EN.ToString();
    }

    //set character sprite to the current character
    private Sprite SetCharacterSprite(ClassDataSO selectedClass, string playerGender)
    {
        if (playerGender == "male")
        {
            return selectedClass.imageMale;
        }
        else if (playerGender == "female")
        {
            return selectedClass.imageFemale;
        }
        else
        {
            return selectedClass.imageMale;
        }
    }

    //generates cards for the current character
    private void GenerateCards()
    {
        foreach (var entry in deck)
        {
            CardDataSO cardData = entry.Key;
            int count = entry.Value;

            Card card = new Card(cardData, null);
            InventoryCardInformation cardInfo = CardSpriteGenerator.Instance.GenerateCardSprite(card, cardHolder, count);
            cardInfo.transform.localScale = new Vector2(0.8f, 0.8f);
            cardInfo.name = $"Card_{card.cardName} (×{count})";
        }
    }

    //sets the stats to match the current character's
    private void SetCharacterStats()
    {
        RaceDataSO selectedRace = raceDatabase.allRaces.Find(r => r.raceName == currentCharacter.basicInfo.raceName);
        charHPStatTxt.text = $"{selectedRace.HP.ToString("D2")}+{currentCharacter.allocatedStats.allocatedHP.ToString("D2")}";
        charENStatTxt.text = $"{selectedRace.EN.ToString("D2")}+{currentCharacter.allocatedStats.allocatedEN.ToString("D2")}";
        charPWRStatTxt.text = $"{selectedRace.PWR.ToString("D2")}+{currentCharacter.allocatedStats.allocatedPWR.ToString("D2")}";
        charSPDStatTxt.text = $"{selectedRace.SPD.ToString("D2")}+{currentCharacter.allocatedStats.allocatedSPD.ToString("D2")}";

        HP = Mathf.FloorToInt(1.5f * (selectedRace.HP + currentCharacter.allocatedStats.allocatedHP));
        EN = Mathf.FloorToInt((selectedRace.EN + currentCharacter.allocatedStats.allocatedEN) / 5);
    }

    //method to increase stats
    public void IncStatButtonAction()
    {
        HandleStatChange("increase");
    }

    //method to decrease stats
    public void DecStatButtonAction()
    {
        HandleStatChange("decrease");
    }

    //finds what stat should change
    private void HandleStatChange(string change)
    {
        //returns the 2nd gameobject of the stat, "HP", "EN", "PWR", "SPD"
        GameObject sender = EventSystem.current.currentSelectedGameObject.transform.parent.GetChild(1).gameObject;

        string statName = sender.name.ToUpper();

        Debug.Log($"{change.ToUpper()} sender: {statName}");

        ChangeStat(statName, change);
    }

    //handles stat changes
    private void ChangeStat(string statName, string change)
    {
        int statChange = (change.ToLower() == "increase") ? 1 : -1;

        switch (statName)
        {
            case "HP":
                currentCharacter.allocatedStats.allocatedHP += statChange;
                break;
            case "EN":
                currentCharacter.allocatedStats.allocatedEN += statChange;
                break;
            case "PWR":
                currentCharacter.allocatedStats.allocatedPWR += statChange;
                break;
            case "SPD":
                currentCharacter.allocatedStats.allocatedSPD += statChange;
                break;
            default:
                Debug.LogWarning($"Unrecognized stat: {statName}");
                break;
        }

        SetCharacterStats();
    }

    //changes the items in the equipment slots to match the current character's
    private void GenerateEquipment()
    {
        EquipmentDataSO armor = equipmentDatabase.allEquipments.Find(e => e.equipmentName == currentCharacter.equipment.armor);
        EquipmentDataSO weapon = equipmentDatabase.allEquipments.Find(e => e.equipmentName == currentCharacter.equipment.weapon);
        EquipmentDataSO accessory1 = equipmentDatabase.allEquipments.Find(e => e.equipmentName == currentCharacter.equipment.accessory1);
        EquipmentDataSO accessory2 = equipmentDatabase.allEquipments.Find(e => e.equipmentName == currentCharacter.equipment.accessory2);
        EquipmentDataSO accessory3 = equipmentDatabase.allEquipments.Find(e => e.equipmentName == currentCharacter.equipment.accessory3);

        if (armor != null) InventoryManager.Instance.AddEquippedItem(armor);
        if (weapon != null) InventoryManager.Instance.AddEquippedItem(weapon);
        if (accessory1 != null) InventoryManager.Instance.AddEquippedItem(accessory1);
        if (accessory2 != null) InventoryManager.Instance.AddEquippedItem(accessory2);
        if (accessory3 != null) InventoryManager.Instance.AddEquippedItem(accessory3);
    }

    //updates the current character's data and party's inventory to match what is shown in UI
    public void UpdateCharacterItems()
    {
        currentCharacter.equipment.armor = InventoryManager.Instance.equipmentSlots[0].transform.GetComponentInChildren<EquipmentInfo>()?.equipment?.equipmentName ?? "";
        currentCharacter.equipment.weapon = InventoryManager.Instance.equipmentSlots[1].transform.GetComponentInChildren<EquipmentInfo>()?.equipment?.equipmentName ?? "";
        currentCharacter.equipment.accessory1 = InventoryManager.Instance.equipmentSlots[2].transform.GetComponentInChildren<EquipmentInfo>()?.equipment?.equipmentName ?? "";
        currentCharacter.equipment.accessory2 = InventoryManager.Instance.equipmentSlots[3].transform.GetComponentInChildren<EquipmentInfo>()?.equipment?.equipmentName ?? "";
        currentCharacter.equipment.accessory3 = InventoryManager.Instance.equipmentSlots[4].transform.GetComponentInChildren<EquipmentInfo>()?.equipment?.equipmentName ?? "";

        PlayerDataHolder.Instance.partyInventory.Clear();
        foreach (InventorySlot slot in InventoryManager.Instance.inventorySlots)
        {
            if (slot.transform.childCount > 0)
            {
                PlayerDataHolder.Instance.partyInventory.Add(slot.transform.GetComponentInChildren<EquipmentInfo>().equipment.equipmentName);
            }
        }
    }

    //method to update gold amount text on merchant screen
    public void UpdateGoldAmount(int amount)
    {
        PlayerDataHolder.Instance.partyGold += amount;
        goldTxt1.text = $"Gold: {PlayerDataHolder.Instance.partyGold.ToString("N0")}";
        goldTxt2.text = $"Gold: {PlayerDataHolder.Instance.partyGold.ToString("N0")}";
    }

    public void KickPartyMember()
    {
        if (currentCharacter == null)
        {
            Debug.LogWarning("No character selected to kick.");
            return;
        }

        if (PlayerDataHolder.Instance.partyMembers.Count <= 1)
        {
            Debug.LogWarning("Cannot kick the last party member.");
            return;
        }

        Debug.Log($"{currentCharacter.basicInfo.characterName} has been kicked from the party.");
        PlayerDataHolder.Instance.partyMembers.Remove(currentCharacter);

        // Update the character list and buttons
        characterList = PlayerDataHolder.Instance.partyMembers;
        SetCharacterButtons();

        // Select the first available character
        var firstValidButton = buttonList.FirstOrDefault(btn => btn.charData != null);
        if (firstValidButton != null)
        {
            SelectedCharacter(firstValidButton);
        }
        TownPartySystem.Instance.UpdatePartyDisplay();
    }

    public void RefreshCharacterList()
    {
        // Update reference (not strictly necessary, but explicit)
        characterList = PlayerDataHolder.Instance.partyMembers;

        // Refresh character buttons
        SetCharacterButtons();

        // Auto-select the first valid character (optional)
        var firstValidButton = buttonList.FirstOrDefault(btn => btn.charData != null);
        if (firstValidButton != null)
        {
            SelectedCharacter(firstValidButton);
        }
    }
}
