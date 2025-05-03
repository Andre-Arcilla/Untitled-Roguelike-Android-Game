using SerializeReferenceEditor;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "NewCard", menuName = "CardData")]
public class CardDataSO : ScriptableObject
{
    [Header("Card Information")]
    [Tooltip("Name of the card")]
    public string cardName;

    [Tooltip("Flavor text or description of the card")]
    public string description;

    [Tooltip("Who this card targets when played")]
    public Target target;

    [Tooltip("Determines if this card is discarded or consumed on use")]
    public bool singleUse;

    [Tooltip("Cost to play this card (e.g., energy points required)")]
    [Range(0, 99)]
    public int cost = 0;

    [Tooltip("The power of the card, affecting damage or effect strength")]
    [Range(0, 99)]
    public int power = 0;

    [Tooltip("Card effects")]
    [SerializeReference, SR]
    public List<ICardEffect> effects;

    [Header("Sprite")]
    [Tooltip("Artwork or image representing the card")]
    public Sprite cardSprite;
}

public enum Target
{
    Enemy,
    Ally,
    Self,
    AllEnemies,
    AllAllies,
    Card,
    Trigger
}
