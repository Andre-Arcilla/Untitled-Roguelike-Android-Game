using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static UnityEditor.Progress;

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
    [SerializeField] Color selectedButton;
    [SerializeField] Color listColorA;
    [SerializeField] Color listColorB;
    [SerializeField] GameObject listObjPrefab;
    [SerializeField] Transform classHolder;
    [SerializeField] Transform cardHolder;
    [SerializeField] private ClassDatabase classDatabase;
    [SerializeField] private RaceDatabase raceDatabase;
    [SerializeField] private EquipmentDatabase equipmentDatabase;
    [SerializeField] public GameObject inventoryCanvas;
    private List<CharacterData> characterList;
    private Dictionary<CardDataSO, int> deck;
    private CharacterData currentCharacter;
    private List<EquipmentDataSO> inventory;
    private int HP;
    private int EN;

    private void Start()
    {
        currentCharacter = new CharacterData();
        deck = new Dictionary<CardDataSO, int>();
        inventory = new List<EquipmentDataSO>();

        characterList = PlayerDataHolder.Instance.partyMembers;

        SetCharacterButtons();

        //sets the displayed information to be 1st character and their deck
        SelectedCharacter(buttonList[0]);
        SelectedPanel(panelList[0]);
        SetInventory();
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
        for (int i = 0; i < PlayerDataHolder.Instance.inventoryItems.Count; i++)
        {
            string jsonItemName = PlayerDataHolder.Instance.inventoryItems[i].equipmentName;
            Debug.Log($"Looking for: {jsonItemName}");

            // Print all names in the equipment database
            Debug.Log("Available in database:");
            foreach (var item in equipmentDatabase.allEquipments)
            {
                Debug.Log("- " + item.equipmentName);
            }

            EquipmentDataSO selectedEquipment = equipmentDatabase.allEquipments
                .Find(e => e.equipmentName == jsonItemName);

            if (selectedEquipment == null)
            {
                Debug.LogWarning($"No match found for: {jsonItemName}");
                continue;
            }

            InventoryManager.Instance.AddItem(selectedEquipment);
            Debug.Log($"Added to inventory: {selectedEquipment.equipmentName}");
        }

    }

    //generates the character buttons based on the characterList
    private void SetCharacterButtons()
    {
        for (int i = 0; i < buttonList.Count; i++)
        {
            if (characterList[i] == null)
            {
                buttonList[i].button.interactable = false;
                buttonList[i].text.text = "";
                continue;
            }
            else
            {
                buttonList[i].button.interactable = true;
                buttonList[i].text.text = characterList[i].basicInfo.characterName.ToString();
                buttonList[i].charData = characterList[i];
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

        //generate card sprites
        GenerateCards();
        SetCharacterStats();

        //input text on screen
        charNameTxt.text = currentCharacter.basicInfo.characterName.ToString();
        charRaceTxt.text = currentCharacter.basicInfo.raceName.ToString();
        charLevelTxt.text = currentCharacter.basicInfo.level.ToString();
        charHPTxt.text = "×" + HP.ToString();
        charENTxt.text = "×" + EN.ToString();
    }

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

    private void GenerateCards()
    {
        foreach (var entry in deck)
        {
            CardDataSO cardData = entry.Key;
            int count = entry.Value;

            Card card = new Card(cardData, null);
            InventoryCardInformation cardInfo = CardSpriteGenerator.Instance.GenerateCardSprite(card, cardHolder, count);
            cardInfo.name = $"Card_{card.cardName} (×{count})";
        }
    }

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

    public void IncStatButtonAction()
    {
        HandleStatChange("increase");
    }

    public void DecStatButtonAction()
    {
        HandleStatChange("decrease");
    }

    private void HandleStatChange(string change)
    {
        //returns the 2nd gameobject of the stat, "HP", "EN", "PWR", "SPD"
        GameObject sender = EventSystem.current.currentSelectedGameObject.transform.parent.GetChild(1).gameObject;

        string statName = sender.name.ToUpper();

        Debug.Log($"{change.ToUpper()} sender: {statName}");

        ChangeStat(statName, change);
    }

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
}
