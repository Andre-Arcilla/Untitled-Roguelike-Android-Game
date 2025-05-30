using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterXP : MonoBehaviour
{
    private CharacterData characterData;
    [SerializeField] private Image charIcon;
    [SerializeField] private TMP_Text charName;
    [SerializeField] private TMP_Text charLevel;
    [SerializeField] private TMP_Text charXP;
    [SerializeField] Image xpBar;
    [SerializeField] private ClassDatabase classDatabase;

    public void Setup(CharacterData characterData, int kills, List<int> levelDiffs, int bonusLevels)
    {
        this.characterData = characterData;

        // Calculate total XP gained from kills and level differences
        int xpGain = XPGainCalculator(kills, levelDiffs);

        // Calculate total XP including current XP
        int totalXP = characterData.basicInfo.xp + xpGain;

        // Calculate level gained from XP and leftover XP
        int levelGainFromXP = totalXP / 100;
        int xpLeft = totalXP % 100;

        // Final level gain includes XP levels and bonus levels
        int totalLevelGain = levelGainFromXP + bonusLevels;

        // Update character data
        characterData.basicInfo.xp = xpLeft;
        characterData.basicInfo.level += totalLevelGain;

        // Update UI
        GenerateCharacterSprite();
        charName.text = $"Name: {characterData.basicInfo.characterName}";

        if (totalLevelGain > 0)
        {
            charLevel.text = $"Level: {characterData.basicInfo.level - totalLevelGain} +{totalLevelGain}";
        }
        else
        {
            charLevel.text = $"Level: {characterData.basicInfo.level}";
        }

        charXP.text = $"XP: {xpLeft} / 100 (+{xpGain})";
        xpBar.fillAmount = xpLeft / 100f;
    }

    private int XPGainCalculator(int kills, List<int> levelDiffs)
    {
        int totalXPFromKills = 0;
        for (int i = 0; i < kills; i++)
        {
            totalXPFromKills += levelDiffs[i] * 7;
        }
        return totalXPFromKills;
    }

    private void GenerateCharacterSprite()
    {
        string targetClassName = characterData.classes[0];
        string playerGender = characterData.basicInfo.gender.ToLower();

        ClassDataSO selectedClass = classDatabase.allClasses.Find(c => c.className == targetClassName);

        if (selectedClass == null)
        {
            return;
        }

        if (playerGender == "male")
        {
            charIcon.sprite = selectedClass.imageMale;
        }
        else if (playerGender == "female")
        {
            charIcon.sprite = selectedClass.imageFemale;
        }
        else
        {
            charIcon.sprite = selectedClass.imageMale;
        }
    }
}
