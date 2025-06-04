using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class VictoryScreenManager : MonoBehaviour
{
    [SerializeField] private GameObject xpUIPrefab;
    [SerializeField] private Transform xpUIParent;
    [SerializeField] private TMP_Text goldText;
    [SerializeField] private TMP_Text townText;
    [SerializeField] private Button nextButton;
    [SerializeField] private GameObject gameClearScreen;

    private void Start()
    {
        GenerateXP();
        GenerateMisc();
        CheckIfAllTownsCleared();
    }

    private void CheckIfAllTownsCleared()
    {
        // Only show game clear screen if the player has cleared ALL listed towns
        bool allCleared = PlayerDataHolder.Instance.partyClearedTowns.All(t => t.hasCleared);

        if (allCleared && PlayerDataHolder.Instance.partyClearedTowns.Count > 0)
        {
            gameClearScreen.SetActive(true);
        }
    }

    private void GenerateXP()
    {
        int extraLevels = 0;
        if (TownManager.Instance.isLabyrinth)
        {
            extraLevels = 5;
        }

        var XPGainList = CombatSystem.Instance.GetXPGainList(); // Access the list

        foreach (var entry in XPGainList)
        {
            GameObject xpUI = Instantiate(xpUIPrefab, xpUIParent);
            xpUI.GetComponent<CharacterXP>().Setup(entry.character, entry.XP, extraLevels);
        }
    }

    private void GenerateMisc()
    {
        goldText.text = $"Gold: {CombatSystem.Instance.goldAmount.ToString("N0")}";
        PlayerDataHolder.Instance.partyGold = CombatSystem.Instance.goldAmount;

        if (TownManager.Instance.isLabyrinth)
        {
            townText.text = $"{TownManager.Instance.townTo.townName} labyrinth cleared!";
        }
        else
        {
            townText.text = $"Next Town: {TownManager.Instance.townTo.townName}";
        }
    }

    public void NextButton()
    {
        foreach (Targetable characterObj in TargetingSystem.Instance.allies.members)
        {
            CharacterInfo characterInfo = characterObj.GetComponent<CharacterInfo>();

            if (characterInfo == null)
            {
                continue;
            }

            if (characterInfo.currentHP <= 0)
            {
                PlayerDataHolder.Instance.partyMembers.Remove(characterInfo.characterData);
            }
        }

        SceneManager.LoadScene(TownManager.Instance.townTo.sceneName);
    }

    public void QuitGame()
    {
        SceneManager.LoadScene("Main Menu");
    }
}
