using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TavernCharacterInfo : MonoBehaviour
{
    [SerializeField] private TMP_Text charName;
    [SerializeField] private TMP_Text charLevel;
    [SerializeField] private TMP_Text charRace;
    [SerializeField] private CharacterData character;

    public void Setup(CharacterData character)
    {
        this.character = character;
        charName.text = character.basicInfo.characterName;
        charLevel.text = character.basicInfo.level.ToString();
        charRace.text = character.basicInfo.raceName;
    }

    public void SendCharacterData()
    {
        TavernManager.Instance.SendCharacterData(character, this.gameObject);
    }
}
