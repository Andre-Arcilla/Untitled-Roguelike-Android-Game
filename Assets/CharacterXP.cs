using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEditor.U2D.Animation;
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

    public void Setup(CharacterData characterData, int kills, List<int> levelDiffs)
    {
        this.characterData = characterData;

        // Calculate total XP gained from kills and level differences
        int xpGain = XPGainCalculator(kills, levelDiffs);

        // Calculate total XP including current XP
        int totalXP = characterData.basicInfo.xp + xpGain;

        // Calculate level gained and leftover XP after leveling up
        int levelGain = totalXP / 100;
        int xpLeft = totalXP % 100;

        // Update character's XP and level
        characterData.basicInfo.xp = xpLeft;
        characterData.basicInfo.level += levelGain;

        // Calculate fill amount for the XP bar (0 to 1)
        float fillAmount = xpLeft / 100f;

        GenerateCharacterSprite();

        // Set UI text fields
        charName.text = $"Name: {characterData.basicInfo.characterName}";

        // Show level with optional gained levels
        if (levelGain > 0)
        {
            charLevel.text = $"Level: {characterData.basicInfo.level - levelGain} +{levelGain}";
        }
        else
        {
            charLevel.text = $"Level: {characterData.basicInfo.level}";
        }

        // Show XP with gained amount in parentheses
        charXP.text = $"XP: {xpLeft} / 100 (+{xpGain})";

        // Update XP bar fill
        xpBar.fillAmount = fillAmount;
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
