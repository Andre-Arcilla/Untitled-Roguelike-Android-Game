using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "NewClass", menuName = "ClassData")]
public class ClassDataSO : ScriptableObject
{
    [Header("Basic Info")]
    public string className;
    [TextArea] public string description;
    public GameObject classSprite;

    [Header("Cards")]
    public List<CardDataSO> startingDeck;
}