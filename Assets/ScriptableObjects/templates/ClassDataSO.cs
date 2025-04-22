using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "NewClass", menuName = "ClassData")]
public class ClassDataSO : ScriptableObject
{
    public enum Gender { Male, Female }

    [Header("Class Information")]
    [Tooltip("The name of the class (e.g., Warrior, Mage, etc.)")]
    public string className;

    [Tooltip("The gender of the character associated with this class")]
    public Gender gender;

    public GameObject spriteMale;
    public GameObject spriteFemale;

    [Header("Starting Cards")]
    [Tooltip("List of starting deck cards for the class")]
    public List<CardDataSO> startingDeck;
}
