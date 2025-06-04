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
    [SerializeField] private RectTransform fillImage; // Reference to FillImage
    [SerializeField] private RectTransform fillMask;  // Reference to FillMask container
    [SerializeField] private ClassDatabase classDatabase;

    public void Setup(CharacterData characterData, int XPGain, int bonusLevels)
    {
        this.characterData = characterData;

        // Calculate total XP including current XP
        int totalXP = characterData.basicInfo.xp + XPGain;

        // Calculate level gained from XP and leftover XP
        int levelGainFromXP = totalXP / 100;
        int xpLeft = totalXP % 100;

        // Final level gain includes XP levels and bonus levels
        int totalLevelGain = levelGainFromXP + bonusLevels;

        // Update character data
        characterData.basicInfo.xp = xpLeft;
        characterData.basicInfo.level += totalLevelGain;
    
        //find the matching characterInfo here
        CharacterInfo matchedInfo = null;

        foreach (Targetable ally in TargetingSystem.Instance.allies.members)
        {
            CharacterInfo info = ally.GetComponent<CharacterInfo>();

            if (info != null && info.characterData.basicInfo.characterName == characterData.basicInfo.characterName)
            {
                matchedInfo = info;
                break;
            }
        }

        // Update UI
        GenerateCharacterSprite(matchedInfo);

        if (matchedInfo != null && matchedInfo.currentHP <= 0)
        {
            charName.text = $"Name: {characterData.basicInfo.characterName} (DEAD)";
        }
        else
        {
            charName.text = $"Name: {characterData.basicInfo.characterName}";
        }

        if (totalLevelGain > 0)
        {
            charLevel.text = $"Level: {characterData.basicInfo.level - totalLevelGain} +{totalLevelGain}";
        }
        else
        {
            charLevel.text = $"Level: {characterData.basicInfo.level}";
        }

        charXP.text = $"XP: {xpLeft} / 100 (+{XPGain})";

        float fillRatio = xpLeft / 100f;
        Canvas.ForceUpdateCanvases();
        float maskWidth = fillMask.rect.width;
        float minWidth = fillImage.rect.height;

        float targetWidth = Mathf.Max(maskWidth * fillRatio, minWidth);

        fillImage.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, targetWidth);
    }

    private void GenerateCharacterSprite(CharacterInfo info)
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
