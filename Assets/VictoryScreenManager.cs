using System.Collections.Generic;
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

    private void Start()
    {
        GenerateXP();
        GenerateMisc();
    }

    private void GenerateXP()
    {
        int extraLevels = 0;
        if (TownManager.Instance.isLabyrinth)
        {
            extraLevels = 5;
        }

        var killList = CombatSystem.Instance.GetKillCounter(); // Access the list

        foreach (var entry in killList)
        {
            GameObject xpUI = Instantiate(xpUIPrefab, xpUIParent);
            xpUI.GetComponent<CharacterXP>().Setup(entry.character, entry.levelDiffs.Count, entry.levelDiffs, extraLevels);
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

        nextButton.onClick.AddListener(NextButton);
    }

    public void NextButton()
    {
        SceneManager.LoadScene(TownManager.Instance.townTo.sceneName);
    }
}
