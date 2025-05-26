using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEngine.UIElements.UxmlAttributeDescription;
using UnityEngine.InputSystem;
using Unity.VisualScripting;

public class GenerateRandomClass : MonoBehaviour
{
    //let a player add one to themself
    public static GenerateRandomClass Instance { get; private set; }

    private void Awake()
    {
        // Check if an instance already exists
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // Only one allowed
            return;
        }

        Instance = this;
    }

    private Dictionary<CharacterData, List<ClassDataSO>> characterClassChoices = new Dictionary<CharacterData, List<ClassDataSO>>();

    [SerializeField] private ClassChoiceDisplay class1;
    [SerializeField] private ClassChoiceDisplay class2;
    [SerializeField] private ClassChoiceDisplay class3;

    public void GenerateClasses(List<ClassDataSO> classes, CharacterData character)
    {
        if (!characterClassChoices.ContainsKey(character))
        {
            List<ClassDataSO> selected = new List<ClassDataSO>(classes);
            for (int i = 0; i < selected.Count; i++)
            {
                int rnd = Random.Range(i, selected.Count);
                (selected[i], selected[rnd]) = (selected[rnd], selected[i]);
            }

            List<ClassDataSO> finalChoices = selected.Take(3).ToList();
            characterClassChoices[character] = finalChoices;
        }

        List<ClassDataSO> choices = characterClassChoices[character];
        int count = choices.Count;

        class1.gameObject.SetActive(count > 0);
        class2.gameObject.SetActive(count > 1);
        class3.gameObject.SetActive(count > 2);

        if (count > 0) class1.Setup(choices[0]);
        if (count > 1) class2.Setup(choices[1]);
        if (count > 2) class3.Setup(choices[2]);
    }


    public void RemoveCharacterChoice(CharacterData character)
    {
        characterClassChoices.Remove(character);
    }
}
