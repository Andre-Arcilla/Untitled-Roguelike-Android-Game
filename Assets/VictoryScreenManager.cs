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
        var killList = CombatSystem.Instance.GetKillCounter(); // Access the list

        foreach (var entry in killList)
        {
            GameObject xpUI = Instantiate(xpUIPrefab, xpUIParent);
            xpUI.GetComponent<CharacterXP>().Setup(entry.character, entry.levelDiffs.Count, entry.levelDiffs);
        }
    }

    private void GenerateMisc()
    {
        goldText.text = $"Gold: {CombatSystem.Instance.goldAmount.ToString("N0")}";
        townText.text = $"Next Town: {TownManager.Instance.townTo.townName}";
        nextButton.onClick.AddListener(NextButton);
    }

    public void NextButton()
    {
        SceneManager.LoadScene(TownManager.Instance.townTo.sceneName);
    }
}
