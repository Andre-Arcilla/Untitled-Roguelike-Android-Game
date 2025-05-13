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
    [SerializeField] private Image characterSprite;
    [SerializeField] private TMP_Text characterName;
    [SerializeField] private TMP_Text characterRace;
    [SerializeField] private TMP_Text characterLevel;
    [SerializeField] private TMP_Text characterEN;
    [SerializeField] private TMP_Text characterHP;

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
    [SerializeField] public GameObject inventoryCanvas;
    private bool inventoryShown;
    private List<CharacterData> characterList;
    private Dictionary<CardDataSO, int> deck;

    private void Start()
    {
        characterList = PlayerDataHolder.Instance.partyMembers;

        if (!inventoryCanvas.activeSelf)
        {
            inventoryShown = false;
        }
        else if (inventoryCanvas.activeSelf)
        {
            inventoryShown = true;
        }

        SetCharacterButtons();

        //sets the displayed information to be 1st character and their deck
        SelectedCharacter(buttonList[0]);
        SelectedPanel(panelList[0]);
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

        if (selectedBtn == null)
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
                SetCharacterInfo(characterButton.charData);
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

    //button method for opening inventory
    public void OpenInventory()
    {
        if (inventoryShown == false)
        {
            inventoryCanvas.SetActive(true);
            inventoryShown = true;
        }
        else if (inventoryShown == true)
        {
            inventoryCanvas.SetActive(false);
            inventoryShown = false;
        }
    }

    // == DISPLAY METHODS =====================================================

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
    private void SetCharacterInfo(CharacterData character)
    {
        deck = new Dictionary<CardDataSO, int>();

        // == GENERAL INFORMATION =============================================
        characterName.text = character.basicInfo.characterName.ToString();
        characterRace.text = character.basicInfo.raceName.ToString();
        characterLevel.text = character.basicInfo.level.ToString();

        // == Class INFORMATION ===============================================
        //probably put this in another method, and also destroy all objects in classHolder making sure its empty
        foreach (Transform child in classHolder)
        {
            Destroy(child.gameObject);
        }

        foreach (Transform child in cardHolder)
        {
            Destroy(child.gameObject);
        }

        //loops through all the classes
        for (int i = 0; i < character.classes.Count; i++)
        {
            ClassDataSO selectedClass = classDatabase.allClasses.Find(c => c.className == character.classes[i]);

            if (selectedClass == null)
            {
                continue;
            }

            //sets the character sprite
            if (i == 0)
            {
                string playerGender = character.basicInfo.gender.ToLower();
                characterSprite.sprite = SetCharacterSprite(selectedClass, playerGender);
            }

            //generates a text for the class
            GameObject classObj = Instantiate(listObjPrefab, classHolder);
            classObj.name = character.classes[i].ToString();
            classObj.GetComponentInChildren<TextMeshProUGUI>().text = character.classes[i].ToString();

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

        // == CARD GENERATION =================================================
        GenerateCards();
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
            cardInfo.name = $"Card_{card.cardName} (x{count})";
        }
    }
}
